using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using System;

public struct NetworkBufferItem
{
    public MonoBehaviour objectType;
    public string objectMethod;
    public object[] args;
    public NetworkBufferItem(MonoBehaviour objectType, string objectMethod, object[] args)
    {
        this.objectType = objectType;
        this.objectMethod = objectMethod;
        this.args = args;
    }
}

public class Lobby : NetworkManager
{
    [Header("References")]
    public Transform matchListContent;
    [Space]
    public InputField matchNameInputField;
    public InputField ipAddressInputField;
    [Space]
    public GameObject MMPanel;
    public GameObject LobbyPanel;
    public GameObject MainMenuPanel;
    public GameObject mainCamera;
    [Space]
    public LobbySeat[] playerSeats;
    [Space]
    public Button masterToggle; //This is used by other scripts
    public Button readyToggle; //This is used by other scripts
    [SerializeField] private GameObject survivorSelectionButton = null;
    [SerializeField] private GameObject disconnectButton = null;
    [Space]
    [SerializeField] private LobbyCountdown countdown = null;
    [Space]
    [SerializeField] private LobbyFade lobbyFade = null;
    [SerializeField] private float fadeTime = 3f;

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

    [Space]
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

        base.OnServerConnect(conn);
    }

    public static Dictionary<string, object[]> dict = new Dictionary<string, object[]>();

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        foreach (var item in dict)
        {
            if (item.Value[0].ToString() == "NetworkConnection")
            {
                item.Value[0] = conn;
            }
            this.GetType().GetMethod(item.Key).Invoke(null, item.Value);
        }

        if (SceneManager.GetActiveScene().path == gameplayScene) return;

        base.OnServerAddPlayer(conn);

        GameObject newLobbyPlayerInstance = conn.identity.gameObject;

        roomPlayers.Add(newLobbyPlayerInstance.GetComponent<LobbyPlayer>());
        // We don't use the conn.connectionId because the network
        // doesn't increment relative to player count.
        //LobbySync.Instance.Svr_AddPlayerLabel(Instance.roomPlayers.Count - 1);
        LobbySync.Instance.Svr_AddPlayer(Instance.roomPlayers.Count - 1);

        ReadyCheck();
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
        LoadingScreenManager.Instance.ShowLoadingScreen(false);

        base.OnClientSceneChanged(conn);
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
        }
        else
        {
            StopClient();
        }
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        Destroy(LobbySync.Instance.gameObject);
        Destroy(gameObject);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        Shutdown();
    }

    public IEnumerator StartGame()
    {
        PickMaster();

        AssignSurvivors();

        yield return new WaitForSeconds(1);

        lobbyFade.BeginFade(fadeTime);

        yield return new WaitForSeconds(fadeTime + 1);

        SwitchScene();
    }

    void SwitchScene()
    {
        LoadingScreenManager.Instance.Svr_ShowLoadingScreen(true);
        ServerChangeScene(gameplayScene);
    }

    void PickMaster()
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
            GameObject who = masters[UnityEngine.Random.Range(0, masters.Count)];

            //Make player the master
            who.GetComponent<LobbyPlayer>().isMaster = true;

            //Change their name
            who.name = who.name + " (Master)";
        }


    }

    void AssignSurvivors()
    {
        Debug.Log("TODO: AssignSurvivors. \n Make a list of all players in a random order, then assign their survivors.");
        Debug.Log("Players who have not chosen any survivor will be at the bottom of the list, and will be assigned a random non-picked survivor.");
    }

    #region Ready System

    public void ReadyCheck()
    {
        //Check if everyone is ready
        if (IsEveryoneReady())
        {
            countdown.StartCountdown();
        }
        else
        {
            countdown.StopCountdown();
        }
    }
    private bool IsEveryoneReady()
    {
        //Iterate through each player and check if they have readied up.
        foreach (LobbyPlayer lobbyPlayer in roomPlayers)
        {
            print(lobbyPlayer);
            //If a player has not readied up, return false.
            if (!lobbyPlayer.isReady) return false;
        }
        //If all are ready, return true.
        return true;
    }

    public void ServerCountdownCompleted()
    {
        StartCoroutine(StartGame());
    }

    public void ClientCountdownCompleted()
    {
        //Disable the ready button for everyone
        readyToggle.gameObject.SetActive(false);
        //Disable the master toggle for everyone
        masterToggle.gameObject.SetActive(false);
        //Disable the survivor selection button for everyone
        survivorSelectionButton.gameObject.SetActive(false);
        //Disable the disconnect button for everyone
        disconnectButton.SetActive(false);
    }

    #endregion

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
        Debug.Log($"Readying {conn.identity.name} : class = {_class} : isMaster = " + isMaster);
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
