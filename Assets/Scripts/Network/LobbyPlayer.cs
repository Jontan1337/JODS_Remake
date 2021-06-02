﻿using System;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Collections;

public class LobbyPlayer : NetworkBehaviour
{
    public bool isMe;
    [Space]
    [SyncVar] public bool isMaster;
    [SyncVar] public bool wantsToBeMaster;
    [SyncVar] public bool isHost = false;

    [SyncVar(hook = nameof(ChangeName))] private string playerName;
    public string PlayerName => playerName;


    [SyncVar] public bool gameOn;
    [SyncVar] public bool setupInGameScene;
    [Header("Character")]
    public SurvivorSO survivorSO = null;
    [SyncVar] public bool hasSelectedACharacter;
    [Space]
    public Color playerColor = Color.red;
    [Header("Data")]
    [SyncVar] public int playerID;
    public int playerIndexOnServer;
    [Header("References")]
    public GameObject lobbyCamera;
    public Camera _lobbyCam;
    public LobbySeat playerSeat;
    private LobbyCharacters lobbyCharacters;

    private bool launcherLogin = false;
    private string userName = null;

    private void Awake()
    {
        DontDestroyOnLoad(this);


        #region Environment setup
        if (Environment.GetCommandLineArgs().Length > 1)
        {
            if (!String.IsNullOrEmpty(Environment.GetCommandLineArgs()[1]))
            {
                launcherLogin = Environment.GetCommandLineArgs()[1].ToString() == "LoggedIn";
            }
            if (!String.IsNullOrEmpty(Environment.GetCommandLineArgs()[2]))
            {
                userName = Environment.GetCommandLineArgs()[2].ToString();
            }
        }
        #endregion
    }


    public override void OnStartClient()
    {
        lobbyCharacters = GetComponent<LobbyCharacters>();

        // Is this the Host
        if (isServer && isLocalPlayer)
        {
            isHost = true;
        }

        if (!hasAuthority) return;

        // Is this me
        if (isLocalPlayer)
        {
            isMe = true;
            // Is the player logged in via the launcher
            string newName;

            if (!launcherLogin)
            {
                newName = "Player_" + UnityEngine.Random.Range(1, 1000).ToString();
            }
            else
            {
                newName = userName;
            }

            Cmd_ChangeName(newName);

            SurvivorSelection.instance.LoadSelection();

            GetMasterToggle();

        }
    }

    [Command]
    public void Cmd_SetNewObject()
    {
        GameObject newPlayerObject = Instantiate(Lobby.Instance.playerSpawner);

        bool isReplaced = NetworkServer.ReplacePlayerForConnection(connectionToClient, newPlayerObject, true);
        if (isReplaced)
        {
            print($"{name} got replaced");
            NetworkServer.Destroy(gameObject);
        }
    }

    [Server]
    public void Svr_PlayerChangeCharacter(GameObject player, string survivorName)
    {
        LobbyPlayer currentPlayer = player.GetComponent<LobbyPlayer>();

        LobbySync.Instance.playersInLobby[currentPlayer.playerID].GetComponent<LobbyCharacters>().Rpc_ChangeCharacter(survivorName);
    }

    [Command]
    public void Cmd_ChangePreference()
    {
        Debug.LogWarning("Cmd_ChangePreference needs to change");

        lobbyCharacters.Svr_GetChoice(wantsToBeMaster);
    }

    /// <summary>
    /// Returns true if both gameobjects have the same LobbyPlayer data.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="otherGameObject"></param>
    [Server]
    private bool CompareCharacterComponent(GameObject player, GameObject otherGameObject)
    {
        // Check if the LobbyPlayer component on the hit object
        // is the same as the own player's LobbyPlayer component.
        LobbyPlayer tempPlayer = player.GetComponent<LobbyPlayer>();
        LobbyPlayer tempOther = otherGameObject.GetComponentInParent<LobbyPlayer>();
        return tempPlayer == tempOther;
    }
    [Command]

    #region ChangeName


    public void ChangeName(string old,string newName)
    {
        if (!isServer)
        {
            playerName = newName;
        }
        gameObject.name = newName;
    }
    [Command]
    public void Cmd_ChangeName(string newName)
    {
        playerName = newName;
        ChangeName("",newName);
    }

    #endregion

    #region Character

    public void SetSurvivorSO(SurvivorSO so)
    {
        survivorSO = so;
        hasSelectedACharacter = so != null;
    }

    private void GetMasterToggle()
    {
        GameObject.Find("Master Toggle").GetComponent<Button>().onClick.AddListener(TogglePreference);
        Debug.LogWarning("GetMasterToggle needs to die");
    }

    public void TogglePreference()
    {
        PlayerPrefs.SetString("Master", "Zombie Master");
        Debug.LogWarning("TogglePreference needs to be reworked");

        wantsToBeMaster = !wantsToBeMaster;

        PlayerPrefs.SetInt("IsMaster", wantsToBeMaster ? 1 : 0);
        print(name + " wants to be master: " + wantsToBeMaster);

        Cmd_ChangePreference();
    }

    #endregion
}
