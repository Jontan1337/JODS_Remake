﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using UnityEngine.InputSystem;
using System;
using Sirenix.OdinInspector;


public enum wahta
{
    common = 2,
    stronk = 20,
}

[RequireComponent(typeof(AudioSource))]
public abstract class GamemodeBase : NetworkBehaviour
{
    #region Singleton
    public static GamemodeBase Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    [Header("General Gamemode Settings")]
    [SerializeField] private int gameStartCountdown = 5;
    [SerializeField] private Text gameStartCountdownText = null;
    [SerializeField] private AudioClip countdownAudio = null;
    [SerializeField] private AudioClip countdownEndAudio = null;
    protected AudioSource AS;
    public MapSettingsSO mapSettings;
    [Space]
    [SerializeField] private int survivorsAlive = 0;
    public int SurvivorsAlive
    {
        get { return survivorsAlive; }
        set
        {
            wahta w = wahta.common;
            int point = (int)w;
            survivorsAlive = value; 

            if (survivorsAlive <= 0)
            {
                Svr_EndGame();
            }
        }
    }


    [Header("Endgame Management")]
    [SerializeField] private GameObject endgameCamera = null;
    [SerializeField] private AudioClip endgameSound = null;
    [SerializeField] private Image endgameFade = null;

    private void Start()
    {
        AS = GetComponent<AudioSource>();
        endgameCamera.SetActive(false);

        if (test && isServer)
        {
            Rpc_CountdownEnd();
            InitializeGamemode();
        }
    }

    #region Point System and Player Scores

    //When a player joins the server, a PlayerData gets created with the player's info.
    //This PlayerData then gets added to the list of players.
    //The server then tells all clients to update the visual scoreboard with the new list of players.
    //
    //Only the server has these PlayerDatas, as it is the only one allowed to modify them.
    [Server]
    public void Svr_AddPlayer(uint playerId, bool isMaster = false)
    {
        GameObject player = NetworkIdentity.spawned[playerId].gameObject;

        if (!isMaster)
        {
            player.GetComponent<BaseStatManager>().onDied.AddListener(delegate { survivorsAlive--; });
            survivorsAlive++;
            Scoreboard.Instance.Svr_AddSurvivor(player.GetComponent<SurvivorPlayerData>());
        }
        else
        {
            Scoreboard.Instance.Svr_AddMaster(player.GetComponent<MasterPlayerData>());
        }
    }

    #endregion

    public override void OnStartServer()
    {
        Lobby.OnPlayersLoaded += Initialize;
    }
    private void Initialize()
    {
        StartCoroutine(IEGameStartCountdown());
    }

    #region Countdown

    private IEnumerator IEGameStartCountdown()
    {
        yield return new WaitForSeconds(0.25f);

        while (gameStartCountdown > 0)
        {
            Rpc_Countdown(gameStartCountdown);
            yield return new WaitForSeconds(1);
            gameStartCountdown--;
        }

        InitializeGamemode();
        Rpc_CountdownEnd();
    }

    [ClientRpc]
    private void Rpc_Countdown(int number)
    {
        gameStartCountdownText.enabled = true;
        gameStartCountdownText.text = number.ToString();

        float numberToDecimal = (float)(number * 0.1);

        AS.pitch = 1 - numberToDecimal;
        AS.PlayOneShot(countdownAudio,0.5f + numberToDecimal);

        if (countdownFade)
        {
            StopCoroutine(countdownFadeCo);
        }
        countdownFadeCo = StartCoroutine(IECountdownFade());
    }

    Coroutine countdownFadeCo;
    bool countdownFade = false;
    private IEnumerator IECountdownFade()
    {
        countdownFade = true;

        Color col = gameStartCountdownText.color;

        float alpha = 1;
        col.a = alpha;

        gameStartCountdownText.color = col;

        yield return new WaitForSeconds(0.4f);


        while (alpha > 0)
        {
            yield return null;
            alpha -= Time.deltaTime * 2;
            col.a = alpha;
            gameStartCountdownText.color = col;
        }

        countdownFade = false;
    }

    [ClientRpc]
    private void Rpc_CountdownEnd()
    {
        gameStartCountdownText.enabled = false;

        AS.pitch = 1;
        AS.PlayOneShot(countdownEndAudio, 1f);


        //Enable player controls here.
    }

    #endregion

    public abstract void InitializeGamemode();

    #region Scoreboard


    #endregion

    #region Endgame

    [Server]
    private void Svr_EndGame()
    {
        EndGame();
        Rpc_EndGame(); //Client stuff, like disable controls and cameras, enable end game camera and enable scoreboard.

        Invoke(nameof(StopGame), endgameSound.length);
    }

    [ClientRpc]
    private void Rpc_EndGame()
    {
        JODSInput.Controls.Disable(); //Disable all controls

        foreach (Camera cam in Camera.allCameras) { cam.enabled = false; } //Disable all enabled cameras in the scene

        endgameCamera.SetActive(true); //Enable the endgame camera
        endgameCamera.GetComponent<Camera>().enabled = true; //Enable the endgame camera
        endgameCamera.transform.position = new Vector3(0, 5, 0);
        /*
        if (mapSettings)
        {
            if (mapSettings.endGameCameraPoints.Length > 0)
            {
                PositionAndRotationPoint camPoint = mapSettings.endGameCameraPoints[Random.Range(0, mapSettings.endGameCameraPoints.Length)];
                endgameCamera.transform.position = camPoint.position;
                endgameCamera.transform.rotation = camPoint.rotation;
            }
        }
        else { endgameCamera.transform.position = new Vector3(0, 5, 0); }
        */


        AS.PlayOneShot(endgameSound, 1f);

        StartCoroutine(EndGameIE(endgameSound.length));
    }

    public abstract void EndGame();

    private IEnumerator EndGameIE(float time)
    {
        float timeToFade = time / 5;
        yield return new WaitForSeconds(time - timeToFade);

        endgameFade.gameObject.SetActive(true);
        float curTime = 0;
        Color fadeCol = endgameFade.color;
        while(curTime < timeToFade)
        {
            curTime += Time.deltaTime;
            fadeCol.a = curTime / timeToFade;
            endgameFade.color = fadeCol;
            yield return null;
        }
    }

    private void StopGame()
    {
        if (Lobby.Instance)
        {
            Lobby.Instance.ReturnToLobby();
        }
        else
        {
            //TEST SCENES
            NetworkManager.singleton.StopHost();
            SceneManager.LoadScene(0);
        }
    }


    #endregion

    [BoxGroup("Debug")]
    [SerializeField] private bool test = false;
}
