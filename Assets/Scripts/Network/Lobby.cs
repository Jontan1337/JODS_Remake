﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using System;
using UnityEngine.Events;

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
    [Header("Lobby Settings")]
    [SerializeField, Range(0,5)] private int requiredAmountOfPlayers = 1;
    [SerializeField] private bool mustHaveMaster = true;

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
    [Space]
    [SerializeField] private SurvivorSelection survivorSelection = null;

    [Header("Prefabs")]
    public GameObject playerSpawner;
    public GameObject preGameRoom;
    public GameObject objectPool;
    public GameObject MatchManager;
    public GameObject lobbyPlayer;
    public GameObject GOLobbySync;
    public GameObject listMatch;
    public GameObject[] gamemodePrefabs;

    [Header("Room Data")]
    public List<LobbyPlayer> roomPlayers = new List<LobbyPlayer>();
    public MatchListing MatchListing;

    [Header("Map Settings (Runtime)")] 
    public MapSettingsSO currentMapSettings;
    [SerializeField, Scene] private string gameplayScene;
    [SerializeField, Scene] private string lobbyScene;
    [Space]
    public int gamemodeInt;

    [Header("Debug")]
    private bool isInitialized = false;

    public static Action OnServerGameStarted;
    public static Action OnClientGameStarted;
    public UnityEvent OnClientDisconnectEvent;

    #region Singleton
    public static Lobby Instance;
    public override void Awake()
    {
        base.Awake();
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            OnServerGameStarted += delegate { DontDestroyOnLoad(this); };
            OnClientGameStarted += delegate { DontDestroyOnLoad(this); };
        }
    }
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


        MMPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void FindReferencesInLobby()
    {
        matchListContent = GameObject.Find("Match List Content Panel").transform;
        matchNameInputField = GameObject.Find("MatchNameInputField").GetComponent<InputField>();
        ipAddressInputField = GameObject.Find("IpAddressInputField").GetComponent<InputField>();
        MMPanel = GameObject.Find("Match Making Panel");
        LobbyPanel = GameObject.Find("Lobby Panel");
        MainMenuPanel = GameObject.Find("Main Menu Panel");
        mainCamera = GameObject.Find("Main Camera");
        masterToggle = GameObject.Find("Master Toggle").GetComponent<Button>();
        readyToggle = GameObject.Find("Ready Toggle").GetComponent<Button>();
        survivorSelectionButton = GameObject.Find("Survivor Selection Button");
        disconnectButton = GameObject.Find("Exit Button");
        survivorSelection = GameObject.Find("Survivor Selection").GetComponent<SurvivorSelection>();
        countdown = GameObject.Find("Lobby Countdown").GetComponent<LobbyCountdown>();
        lobbyFade = GameObject.Find("Lobby Fade").GetComponent<LobbyFade>();
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

        //Apply the masters associasted map settings as the current map settings
        List<UnitMasterSO> masterSOList = PlayableCharactersManager.Instance.GetAllMasters();

        foreach (UnitMasterSO master in masterSOList)
        {
            if (master.name == MasterSelection.instance.GetMasterName)
            {
                currentMapSettings = master.associatedMapSettings;
                gameplayScene = currentMapSettings.gameplayScene;
                break;
            }
        }

        EnterLobby();
    }
    #endregion

       
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

        LobbyPlayer lobbyPlayer = newLobbyPlayerInstance.GetComponent<LobbyPlayer>();

        roomPlayers.Add(lobbyPlayer);
        // We don't use the conn.connectionId because the network
        // doesn't increment relative to player count.
        //LobbySync.Instance.Svr_AddPlayerLabel(Instance.roomPlayers.Count - 1);
        LobbySync.Instance.Svr_AddPlayer(Instance.roomPlayers.Count - 1);

        if(conn.connectionId != 0) lobbyPlayer.Rpc_ClientChatStart(conn, WebsocketManager.Instance.playerProfile.data[0].guid);

        ReadyCheck();
    }

    // Called on server when anyone disconnects.
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        PlayerEquipment playerEquipment = conn.identity.gameObject.GetComponentInChildren<PlayerEquipment>();
        if (playerEquipment != null)
        {
            conn.identity.gameObject.GetComponentInChildren<PlayerEquipment>().Svr_DropAllItems();
        }
        SurvivorSelection.instance.Svr_OnPlayerDisconnect(conn.identity.netId);
        LobbySync.Instance.Svr_RemovePlayer(Instance.roomPlayers.Count - 1);
        Instance.roomPlayers.Remove(conn.identity.GetComponent<LobbyPlayer>());
        NetworkServer.DestroyPlayerForConnection(conn);

        if (conn.connectionId == 0)
        {
            OnClientDisconnectEvent?.Invoke();
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        OnClientDisconnectEvent?.Invoke();

        base.OnClientDisconnect(conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        //This happens on the server

        if (SceneManager.GetActiveScene().path == gameplayScene)
        {
            //Spawn the Pre Game Waiting Room
            GameObject preGameInstance = Instantiate(preGameRoom);
            preGameInstance.GetComponent<PreGameWaitingRoom>().playersToWaitFor = roomPlayers.Count;
            NetworkServer.Spawn(preGameInstance);

            //Spawn the Player Spawner
            GameObject playerSpawnerInstance = Instantiate(playerSpawner);
            playerSpawnerInstance.GetComponent<PlayerSpawner>().mapSettings = currentMapSettings;
            NetworkServer.Spawn(playerSpawnerInstance);

            //Spawn the Object Pool
            GameObject objectPoolInstance = Instantiate(objectPool);
            NetworkServer.Spawn(objectPoolInstance);
            objectPool = objectPoolInstance;

            GameObject gamemodeManager = Instantiate(gamemodePrefabs[gamemodeInt]);
            gamemodeManager.GetComponent<GamemodeBase>().mapSettings = currentMapSettings;
            NetworkServer.Spawn(gamemodeManager);
        }
        if (SceneManager.GetActiveScene().path == lobbyScene)
        {
            GameObject lobbyPlayer;
            GameObject oldPlayerInstance;
            foreach (var player in NetworkServer.connections.Values)
            {
                lobbyPlayer = Instantiate(this.lobbyPlayer);
                oldPlayerInstance = player.identity.gameObject;
                NetworkServer.Spawn(lobbyPlayer);
                NetworkServer.ReplacePlayerForConnection(player, lobbyPlayer);
                NetworkServer.Destroy(oldPlayerInstance);
            }
        }
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        //This happens locally on the client

        //Disable the loading screen for this client
        LoadingScreenManager.Instance.ShowLoadingScreen(false);

        base.OnClientSceneChanged(conn);

        if (SceneManager.GetActiveScene().path == lobbyScene)
        {
            FindReferencesInLobby();
        }
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
            //NetworkServer.DisconnectAllConnections();
            //for (int i = numPlayers-1; i >= 0; i--)
            //{
            //    Debug.Log($"Destroying player connection {roomPlayers[i].connectionToServer}");
            //    NetworkServer.DestroyPlayerForConnection(roomPlayers[i].connectionToServer);
            //}
            MatchListing.StopDiscovery();
            StopHost();
        }
        else
        {
            StopClient();
        }
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        //Destroy(LobbySync.Instance.gameObject);
        //Destroy(gameObject);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        Shutdown();
    }

    #region Gameplay Scene specific

    public void AllPlayersHaveLoaded()
    {
        StartCoroutine(PreGameCo());
    }

    public static event Action OnPlayersLoaded;

    private IEnumerator PreGameCo()
    {
        yield return new WaitForSeconds(0.1f); //Network delay

        //Disable player controls, because the players have been spawned at this point. But the pre-game waiting room is still active.
        LobbySync.Instance.Rpc_DisableControls();

        //Invoke this, which tells other objects that all players have loaded and allows them to do their thing.
        OnPlayersLoaded?.Invoke();

        //Object Pool

        //TODO: Give object pools more objects to spawn 
        //      Master units
        //      Survivor specific objects

        //Then we tell the object pool to initialize it's pools, which will spawn all of the objects.
        objectPool.GetComponent<ObjectPool>().Svr_InitializePools();

        yield return new WaitForSeconds(1.5f); //This delay is mostly to hide all of the game initialization going on.
        
        //Destroy the pre game waiting room.
        NetworkServer.Destroy(PreGameWaitingRoom.Instance.gameObject);
        //Enable player controls.
        LobbySync.Instance.Rpc_EnableControls();
    }


    #endregion

    #region Lobby specific
    private void EnterLobby()
    {
        MenuCamera.instance.ChangePosition("Lobby");
    }

    public IEnumerator StartGame()
    {
        //First pick a master
        bool pickMaster = PickMaster();

        //Do not go further if no master was chosen.
        if (!pickMaster) yield break;

        yield return new WaitForSeconds(0.5f);

        //Then assign survivors to the players who have not selected a character
        bool assignSurvivors = AssignSurvivors();

        //Do not go further if AssignSurvivors failed.
        if (!assignSurvivors) yield break;

        yield return new WaitForSeconds(0.5f);

        //Begin a fade to black
        lobbyFade.BeginFade(fadeTime);

        yield return new WaitForSeconds(fadeTime + 1);

        //Tell the server to switch scene to the game scene
        SwitchScene();
    }

    void SwitchScene()
    {
        //Tell the server to enable the loading screen for everyone
        LoadingScreenManager.Instance.Svr_ShowLoadingScreen(true);

        //Switch to the gameplay scene
        ServerChangeScene(gameplayScene);
    }

    public void ReturnToLobby()
    {
        ServerChangeScene(offlineScene);
        //This doesn't actually work properly
    }

    private bool PickMaster()
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

            LobbyPlayer player = who.GetComponent<LobbyPlayer>();

            //Make player the master
            player.isMaster = true;

            //Now we need to make sure that whatever the player has chosen as survivor (if any) will be reset, making it available for others to use.
            if (player.hasSelectedACharacter)
            {
                //First iterate through all the Available Survivors and find the one that the player has chosen
                foreach(SurvivorSelect survivor in survivorSelection.availableSurvivors)
                {
                    if (survivor.survivor == player.survivorSO)
                    {
                        //Override that survivor, making it available again.
                        survivor.Svr_OverrideSelection();
                    }
                }
            }

            //Change their name (Only in the editor inspector. Won't affect gameplay or players)
            who.name += " (Master)";            

            //Master has been successfully chosen 
            return true;
        }
        else
        {
            if (mustHaveMaster)
            {
                //If a master is required, but no master was chosen
                //Something went wrong
                Debug.LogError("No master was chosen! Something went wrong.");
                return false;
            }
            else return true; //If no master is required, then there is no issue if no master has been chosen.
        }
    }

    //This method assigns all remaining survivors to non-master players (who have not chosen a survivor already)
    private bool AssignSurvivors()
    {
        List<LobbyPlayer> eligiblePlayers = new List<LobbyPlayer>();

        //Iterate through all players in the lobby
        foreach(LobbyPlayer lobbyPlayer in roomPlayers)
        {
            //Only add the player to the eligiblePlayers list if they're not the master and have not chosen a survivor
            if (!lobbyPlayer.isMaster && !lobbyPlayer.hasSelectedACharacter)
            {
                eligiblePlayers.Add(lobbyPlayer);
            }
        }

        //Now that we have the list of eligible players, we need to assign the remaining survivors to each player
        
        //First we make a list of the remaining survivors
        List<SurvivorSO> remainingSurvivors = new List<SurvivorSO>();

        //Then iterate through all the Available Survivors and find those that have not been chosen
        foreach (SurvivorSelect survivor in survivorSelection.availableSurvivors)
        {
            //If the survivor has not been chosen
            if (!survivor.Selected)
            {
                //Add it to the list
                remainingSurvivors.Add(survivor.survivor);
            }
        }

        //Now do a check to see if the amount of remaining survivors matches the amount of eligible players
        //If not, then something went wrong
        if (remainingSurvivors.Count < eligiblePlayers.Count)
        {
            Debug.LogError("Amount of Remaining Survivors is less than amount of Eligible Players. Something went wrong.");
            return false;
        }

        foreach(LobbyPlayer playerToAssignSurvivor in eligiblePlayers)
        {
            //First we get the index of a random survivor from the list of remaining survivors
            int randomSurvivorIndex = UnityEngine.Random.Range(0, remainingSurvivors.Count);
            SurvivorSO randomSurvivor = remainingSurvivors[randomSurvivorIndex]; //Then a reference to that randomly chosen survivor
            //Then we remove the survivor from the list of remaining survivors, as it is no longer available
            remainingSurvivors.RemoveAt(randomSurvivorIndex);

            //Assign the survivor to the player
            playerToAssignSurvivor.SetSurvivorSO(randomSurvivor);
        }

        return true;
    }

    public void ChangeGamemode(int value)
    {
        gamemodeInt = value;
    }

    #endregion

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
        if (roomPlayers.Count < requiredAmountOfPlayers) return false;

        //Iterate through each player and check if they have readied up.
        foreach (LobbyPlayer lobbyPlayer in roomPlayers)
        {
            //If a player has not readied up, return false.
            if (!lobbyPlayer.isReady) return false;
        }
        //If all are ready, return true.
        return true;
    }

    public void ServerCountdownCompleted()
    {
        // Set objects in DontDestroyOnLoad.
        OnServerGameStarted?.Invoke();
        StartCoroutine(StartGame());
    }

    public void ClientCountdownCompleted()
    {
        OnClientGameStarted?.Invoke();
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
        yield return new WaitForSeconds(0.2f); //Delay because server slow
        OnServerReadied?.Invoke(conn, _class, isMaster);
    }

    public static void InvokeRelayOnServerSynchronize(NetworkConnection conn)
    {
        // Don't synchronize the host player since they have all data already.
        if (conn.connectionId != 0)
        {
            RelayOnServerSynchronize?.Invoke(conn);
        }
    }

    #endregion

    private void OnGUI()
    {
        if (!mustHaveMaster) GUI.TextField(new Rect(20, 60, 150, 20), "Must Have Master: " + (mustHaveMaster ? "On" : "Off"));
    }
}
