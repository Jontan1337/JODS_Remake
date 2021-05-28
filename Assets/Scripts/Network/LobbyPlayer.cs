﻿using System;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Collections;

public class LobbyPlayer : NetworkBehaviour
{
    [SyncVar] public bool isMaster;
    [SyncVar] public bool wantsToBeMaster;
    [SyncVar] public bool isHost = false;
    [SyncVar] public string playerName;
    [SyncVar] public bool gameOn;
    [SyncVar] public bool isMe;
    [SyncVar] public bool setupInGameScene;

    [Header("Data")]
    [SyncVar] public int playerID;
    public int playerIndexOnServer;
    [Header("References")]
    public Toggle masterToggle;
    public Dropdown survivorDropdown;
    public GameObject _nameLabel;
    public Image _nameLabelMasterColor;
    public GameObject lobbyCharacter;
    public GameObject lobbyCamera;
    public Camera _lobbyCam;
    public LobbySeat playerSeat;

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

    private void Start()
    {

        // Is this the Host
        if (isLocalPlayer && isServer)
        {
            isHost = true;
        }

        // Is this me
        if (isLocalPlayer)
        {
            isMe = true;
            // Is the player logged in via the launcher
            if (!launcherLogin)
            {
                playerName = "Player_" + UnityEngine.Random.Range(1, 1000).ToString();
            }
            else
            {
                playerName = userName;
            }
            Cmd_ChangeName(playerName);
            PlayerPrefs.SetString("PlayerName", playerName);
            PlayerPrefs.SetInt("Character", 1);
            PlayerPrefs.SetInt("Survivor", 0);
            PlayerPrefs.SetInt("Master", 0);
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
    public void Svr_PlayerChangeCharacter(GameObject player, int characterIndex)
    {
        LobbyPlayer currentPlayer = player.GetComponent<LobbyPlayer>();

        LobbySync.Instance.playersInLobby[currentPlayer.playerID].GetComponent<LobbyCharacters>().Svr_ChangeCharacter(characterIndex);
    }
    [Command]
    public void Cmd_ChangePreference(bool wantToBe)
    {
        wantsToBeMaster = wantToBe;

        if (_nameLabelMasterColor)
        {
            _nameLabelMasterColor.enabled = wantsToBeMaster;
            Rpc_ChangePreference(_nameLabel.gameObject, wantsToBeMaster);
        }
    }
    [ClientRpc]
    public void Rpc_ChangePreference(GameObject playerLabel, bool wantsToBe)
    {
        if (playerLabel)
            playerLabel.GetComponentInChildren<Image>().enabled = wantsToBe;
    }

    [Server]
    private void Svr_RequestMasterToggle(GameObject player, GameObject character)
    {
        if (CompareCharacterComponent(player, character))
        {
            Cmd_TogglePreference(character);
        }
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
    private void Cmd_TogglePreference(GameObject lobbyChar)
    {
        wantsToBeMaster = !wantsToBeMaster;
        LobbySync.Instance.Svr_PlayerSmoke(lobbyChar.gameObject);
    }
    [Command]
    private void Cmd_CheckHit(int characterID)
    {
        GameObject tempCharacter = LobbySync.Instance.playersInLobby[characterID];
        Svr_RequestMasterToggle(gameObject, tempCharacter);
    }

    #region ChangeName
    [Command]
    public void Cmd_ChangeName(string newName)
    {
        Svr_ChangeName(newName);
    }
    [Server] 
    private void Svr_ChangeName(string newName)
    {
        gameObject.GetComponent<LobbyPlayer>().playerName = newName;
        gameObject.name = newName;
    }

    [ClientRpc]
    private void Rpc_ChangeName(GameObject player, string newName)
    {
        player.name = newName;
    }
    #endregion

    // Change character
    public void ChangePrefab()
    {
        // Master - 0
        // Survivor - 1
        if (isHost)
        {
            //If host, call others and tell them to change prefab
            CallOthers();
        }
        if (hasAuthority)
        {
            gameOn = true;
            if (isMaster)
            {
                PlayerPrefs.SetInt("Character", 0);
            }
            else
            {
                PlayerPrefs.SetInt("Character", 1);
            }
        }
    }
    void CallOthers()
    {
        GameObject[] others = GameObject.FindGameObjectsWithTag("LobbyPlayer");
        foreach (GameObject o in others)
        {
            if (!o.GetComponent<LobbyPlayer>().isHost)
            {
                //Debug.Log("Found : " + o.name);
                CmdCallOthers(o);
            }
        }
    }

    #region ChangeSurvivor
    void ChangeSurvivor(int num)
    {
        PlayerPrefs.SetInt("Survivor", num);
        Cmd_ChangeSurvivor(gameObject, num);
        LobbySync.Instance.sound.PlaySound("Change");
    }
    [Command]
    void Cmd_ChangeSurvivor(GameObject player, int num)
    {
        Svr_PlayerChangeCharacter(player, num);
    }
    #endregion

    void ChangeMaster(int num)
    {
        PlayerPrefs.SetInt("Master", num);
    }

    [Command]
    void CmdCallOthers(GameObject other)
    {
        RpcCallOthers(other);
    }
    [ClientRpc]
    void RpcCallOthers(GameObject other)
    {
        other.GetComponent<LobbyPlayer>().ChangePrefab();
    }
}
