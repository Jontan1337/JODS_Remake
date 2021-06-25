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
    public LobbySeat[] playerSeats;
    public GameObject loadingScreen;
    public Button masterToggle;
    [Header("Prefabs")]
    public GameObject playerSpawner;
    public GameObject MatchManager;
    public GameObject lobbyPlayer;
    public GameObject GOLobbySync;
    public GameObject listMatch;

    [Header("Room Data")]
    public List<LobbyPlayer> roomPlayers = new List<LobbyPlayer>();
    public MatchListing MatchListing;
    [Scene] public string gameplayScene;

    //private bool quickJoin = false;
    public bool mustHaveMaster;

    private bool isInitialized = false;

    private static Action<NetworkConnection> RelayOnServerPlayerReady;

    #region Singleton
    public static Lobby Instance;
    #endregion

    #region Match Making
    private void OnEnable()
    {
        if (isInitialized) return;

        Start();
#if UNITY_SERVER
        MatchListing.AdvertiseServer();
#endif
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
        DontDestroyOnLoad(loadingScreen);
        loadingScreen.SetActive(false);
    }
    //Match list join
    public void MMJoinMatch(Uri uri)
    {
        LobbyPanel.SetActive(true);
        MMPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        singleton.StartClient(uri);

        EnterLobby();
    }

    //Manual IP join
    public void MMJoinMatch()
    {
        string ipAddress = ipAddressInputField.text;
        LobbyPanel.SetActive(true);
        MMPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        UriBuilder uriBuilder = new UriBuilder("tcp4", ipAddress);
        singleton.StartClient(uriBuilder.Uri);

        EnterLobby();
    }

    public void MMCreateMatch()
    {
        singleton.StartHost();

        MatchListing.AdvertiseServer();

        EnterLobby();
    }
    #endregion
    private void EnterLobby()
    {
        MenuCamera.instance.ChangePosition("Lobby");
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
        conn.identity.gameObject.GetComponentInChildren<PlayerEquipment>().Svr_DropAllItems();
        SurvivorSelection.instance.Svr_OnPlayerDisconnect(conn.identity.netId);
        Instance.roomPlayers.Remove(conn.identity.GetComponent<LobbyPlayer>());
        NetworkServer.DestroyPlayerForConnection(conn);
        //LobbySync.Instance.Svr_RemovePlayerLabel(Instance.roomPlayers.Count-1);
        //LobbySync.Instance.Svr_RemovePlayer(Instance.roomPlayers.Count-1);



        base.OnServerDisconnect(conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().path == gameplayScene)
        {
            GameObject playerSpawnerInstance = Instantiate(playerSpawner);
            NetworkServer.Spawn(playerSpawnerInstance);
        }
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        loadingScreen.SetActive(false);

        base.OnClientSceneChanged(conn);
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        loadingScreen.SetActive(true);

        base.OnServerChangeScene(newSceneName);
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
        foreach(var m in masters) { print(m); }
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

    #region New

    public static event Action<NetworkConnection, string, bool> OnServerReadied;
    public static event Action<NetworkConnection> RelayOnServerSynchronize;

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        if (SceneManager.GetActiveScene().path != gameplayScene) return;

        LobbyPlayer player = null;

        if (conn.identity)
        {
            if (conn.identity.gameObject.TryGetComponent(out LobbyPlayer _player))
            {
                player = _player;
            }
        }

        StartCoroutine(InvokeOnServerReady(conn, player.isMaster ? MasterSelection.instance.GetMasterName : player.survivorSO.name, player.isMaster));
    }

    private IEnumerator InvokeOnServerReady(NetworkConnection conn, string _class, bool isMaster)
    {
        yield return new WaitForSeconds(0.2f);
        OnServerReadied?.Invoke(conn, _class, isMaster);
        yield return new WaitForSeconds(0.3f);
        if (conn.connectionId != 0)
        {
            RelayOnServerSynchronize?.Invoke(conn);
        }
    }

    #endregion
}
