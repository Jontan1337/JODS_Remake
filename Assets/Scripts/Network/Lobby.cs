using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using System;

public class Lobby : NetworkManager
{
    [Header("References")]
    public Transform matchListContent;
    public InputField matchNameInputField;
    public InputField ipAddressInputField;
    public GameObject startButton;
    public GameObject disconnectButton;
    public GameObject MMPanel;
    public GameObject LobbyPanel;
    public GameObject MainMenuPanel;
    public GameObject mainCamera;
    public GameObject lobbyCamera;
    public LobbySeat[] playerSeats;
    [Header("Prefabs")]
    public GameObject playerSpawner;
    public GameObject MatchManager;
    public GameObject lobbyPlayer;
    public GameObject GOLobbySync;
    public GameObject listMatch;
    public GameObject survivorPrefab;
    public GameObject masterPrefab;
    public GameObject lobbyCharacters;

    [Header("Room Data")]
    public List<LobbyPlayer> roomPlayers = new List<LobbyPlayer>();
    public MatchListing MatchListing;
    [Scene] public string gameplayScene;

    //private bool quickJoin = false;
    public bool mustHaveMaster;

    private bool isInitialized = false;

    #region Singleton
    public static Lobby Instance;
    #endregion

    private void OnEnable()
    {
        if (isInitialized) return;

        Start();
        isInitialized = true;

        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        MMPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void MMJoinMatch(System.Uri uri)
    {
        LobbyPanel.SetActive(true);
        MMPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        mainCamera.SetActive(false);
        lobbyCamera.SetActive(true);
        singleton.StartClient(uri);
    }
    public void MMJoinMatch()
    {
        string ipAddress = ipAddressInputField.text;
        LobbyPanel.SetActive(true);
        MMPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        mainCamera.SetActive(false);
        lobbyCamera.SetActive(true);
        UriBuilder uriBuilder = new UriBuilder("tcp4", ipAddress);
        singleton.StartClient(uriBuilder.Uri);
    }

    public void MMCreateMatch()
    {
        singleton.StartHost();

        MatchListing.AdvertiseServer();


        //GameObject newLobbySync = Instantiate(GOLobbySync);
        //newLobbySync.GetComponent<LobbySync>().lobbySeats = playerSeats;
        //NetworkServer.Spawn(newLobbySync);

        //string matchName = "Default Match";
        //if (!string.IsNullOrEmpty(matchNameInputField.text))
        //{
        //    matchName = matchNameInputField.text;
        //}

        //LobbyPanel.SetActive(true);
        //MMPanel.SetActive(false);
        //MainMenuPanel.SetActive(false);
    }


    public override void OnServerConnect(NetworkConnection conn)
    {
        // Is the current active scene the same as the gameplay scene.
        if (SceneManager.GetActiveScene().path == gameplayScene)
        {
            conn.Disconnect();
            return;
        }

        // Is the maximum number of players already connected.
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        // Is the connection id 0 (host).
        if (conn.connectionId == 0)
        {
            startButton.SetActive(true);
        }

        base.OnServerConnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == gameplayScene) return;

        base.OnServerAddPlayer(conn);
        print("OnServerAddPlayer");
        GameObject newLobbyPlayerInstance = conn.identity.gameObject;

        roomPlayers.Add(newLobbyPlayerInstance.GetComponent<LobbyPlayer>());
        // We don't use the conn.connectionId because the network
        // doesn't increment relative to player count.
        //LobbySync.Instance.Svr_AddPlayerLabel(Instance.roomPlayers.Count - 1);
        LobbySync.Instance.Svr_AddPlayer(Instance.roomPlayers.Count - 1);
    }

    // Called on server when anyone disconnects.
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Instance.roomPlayers.Remove(conn.identity.GetComponent<LobbyPlayer>());
        NetworkServer.DestroyPlayerForConnection(conn);
        //LobbySync.Instance.Svr_RemovePlayerLabel(Instance.roomPlayers.Count-1);
        //LobbySync.Instance.Svr_RemovePlayer(Instance.roomPlayers.Count-1);


        base.OnServerDisconnect(conn);
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);


        // If server is in gameplay scene
        // then spawn a player spawner for each player.
        if (SceneManager.GetActiveScene().path != gameplayScene) return;

        GameObject oldPlayerObject = conn.identity.gameObject;
        var gamePlayerInstance = Instantiate(playerSpawner);
        if (NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance))
        {
            NetworkServer.Destroy(oldPlayerObject);
        }

        //if (playersReady == roomPlayers.Count)
        //{
        //    GameObject GOMatchManager = Instantiate(MatchManager);
        //    NetworkServer.Spawn(GOMatchManager);
        //    StartCoroutine(GOMatchManager.GetComponent<MatchManager>().Svr_StartInitialization());
        //}
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        print("OnServerSceneChanged");
        
    }

    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        base.OnServerError(conn, errorCode);

        print($"Error: {conn} \n {errorCode}");
    }

    public void Disconnect()
    {
        // If host/server then stop server.
        if (LobbySync.Instance.isServer)
        {
            NetworkServer.DisconnectAllConnections();
            //for (int i = numPlayers-1; i >= 0; i--)
            //{
            //    Debug.Log($"Destroying player connection {roomPlayers[i].connectionToServer}");
            //    NetworkServer.DestroyPlayerForConnection(roomPlayers[i].connectionToServer);
            //}
            MatchListing.StopDiscovery();
            Shutdown();
            print("Shutdown");
        }
        else
        {
            StopClient();
        }
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        print("OnStopServer");

        Destroy(LobbySync.Instance.gameObject);
        Destroy(gameObject);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        Shutdown();
    }

    public void StartGame()
    {
        List<GameObject> masters = new List<GameObject>();
        //Pick a random player (who wants to be master) to become master
        foreach (var p in Instance.roomPlayers)
        {
            if (p != null)
            {
                if (p.GetComponent<LobbyPlayer>().wantsToBeMaster)
                {
                    masters.Add(p.gameObject);
                }
            }
        }
        //If no one wants to be master, pick a random player
        if (mustHaveMaster)
        {
            if (masters.Count == 0)
            {
                foreach (var p in Instance.roomPlayers)
                {
                    if (p != null)
                    {
                        masters.Add(p.gameObject);
                    }
                }
            }
        }
        // PICK RANDOM PLAYER TO BECOME MASTER (FROM THE MASTERS LIST)
        if (masters.Count != 0)
        {
            PickMaster(masters[UnityEngine.Random.Range(0, masters.Count)]);
        }
        startButton.SetActive(false);
        Invoke(nameof(SwitchScene), 1f);
    }

    void SwitchScene()
    {
        foreach (var p in Instance.roomPlayers)
        {
            if (p != null)
            {
                p.GetComponent<LobbyPlayer>().ChangePrefab();
            }
        }
        ServerChangeScene(gameplayScene);
    }

    void PickMaster(GameObject who)
    {
        //Make player the master
        who.GetComponent<LobbyPlayer>().isMaster = true;
        Debug.Log(who.name + " has been chosen as the master");
        //Change his name
        who.name = who.name + " (Master)";
    }

    // TODO: Implement.
    //public void QuickJoin()
    //{
    //    quickJoin = true;
    //    MatchList();
    //}
}
