using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

[System.Serializable]
public class PlayerData
{
    public PlayerData() { }
    public PlayerData(uint playerId, string playerName, int score, bool isMaster)
    {
        this.playerId = playerId;
        this.playerName = playerName;
        this.score = score;
        this.isMaster = isMaster;

        if (isMaster) return;

        alive = true;
        points = score;
    }

    [SyncVar] public string playerName;
    [Header("Shared")]
    [SyncVar] public uint playerId;
    [SyncVar] public int score;
    [SyncVar] public bool isMaster;
    [Header("Survivor")]
    [SyncVar] public int points;
    [SyncVar] public int kills;
    [SyncVar] public bool alive;
    [Header("Master")]
    [SyncVar] public int unitsPlaced;
    [SyncVar] public int totalUpgrades;
    [SyncVar] public int totalUnitUpgrades;
}

public enum PlayerDataStat
{
    Score,
    Points,
    Kills,
    UnitsPlaced,
    TotalUpgrades,
    TotalUnitUpgrades,
    Alive
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

    [Header("Points System Management")]
    [SerializeField] private int defaultStartingPoints = 0;
    [Space]
    [SerializeField] private List<PlayerData> playerList = new List<PlayerData>();

    [Header("Endgame Management")]
    [SerializeField] private GameObject endgameCamera = null;
    [SerializeField] private AudioClip endgameSound = null;
    [SerializeField] private Image endgameFade = null;

    public int PlayerCount
    {
        get => playerList.Count;
    }

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

    [ClientRpc]
    private void Rpc_ChangePlayerList(PlayerData playerData)
    {
        bool playerExists = false;

        int index = 0;
        foreach (PlayerData player in playerList)
        {
            if (player.playerId == playerData.playerId)
            {
                index = playerList.IndexOf(player);
                playerExists = true;
            }
        }

        if (playerExists)
        {
            playerList[index] = playerData;
        }

        else playerList.Add(playerData);

        //Assign the player to a scoreboard row
        foreach (ScoreboardRow row in playerData.isMaster ? masterRows : survivorRows)
        {
            if (row.playerId == 0)
            {
                row.playerId = playerData.playerId;
                row.SetupPlayerScore(playerData);
                break;
            }
        }
    }

    private PlayerData GetPlayer(uint playerId)
    {
        int index = 0;
        foreach(PlayerData player in playerList)
        {
            if(player.playerId == playerId)
            {
                index = playerList.IndexOf(player);
            }
        }
        return playerList[index];
    }

    [Server]
    public int Svr_GetPoints(uint playerId)
    {
        return GetPlayer(playerId).points;
    }

    [Server]
    public void Svr_ModifyStat(uint playerId, int amount, PlayerDataStat stat = PlayerDataStat.Score)
    {
        Rpc_ModifyPlayerData(playerId, amount, stat);

        PlayerData player = GetPlayer(playerId);
        if (!player.isMaster && amount == 0 && stat == PlayerDataStat.Alive) //Did the Alive stat get changed to 0? (Someone died)
        {
            bool everyoneIsDead = true;

            //Iterate through all players and check if they're alive.
            foreach(PlayerData pd in playerList)
            {
                if (pd == player) continue; //Skip the initial player, we know it's dead.

                if (pd.alive) everyoneIsDead = false; //If the player we're checking is alive, then we don't end the game
            }
            if (everyoneIsDead) Svr_EndGame(); //All players are dead, end the game.
        }
    }
    
    [ClientRpc]
    private void Rpc_ModifyPlayerData(uint playerId, int amount, PlayerDataStat stat)
    {
        PlayerData playerToModify = GetPlayer(playerId);

        switch (stat)
        {
            case PlayerDataStat.Points:
                playerToModify.points += amount;
                break;
            case PlayerDataStat.Kills:
                playerToModify.kills += amount;
                break;
            case PlayerDataStat.UnitsPlaced:
                playerToModify.unitsPlaced += amount;
                break;
            case PlayerDataStat.TotalUnitUpgrades:
                playerToModify.totalUnitUpgrades += amount;
                break;
            case PlayerDataStat.TotalUpgrades:
                playerToModify.totalUpgrades += amount;
                break;
            case PlayerDataStat.Alive:
                playerToModify.alive = amount != 0; //False if 0, true if 1
                break;
        }

        if (amount > 0)
        {
            playerToModify.score += amount;
        }

        UpdateScoreboardRow(playerToModify);
    }

    [Server]
    public void Svr_AddPlayer(uint playerId, string playerName, bool isMaster = false)
    {
        PlayerData newPlayer = new PlayerData(playerId, playerName, defaultStartingPoints, isMaster);

        Rpc_ChangePlayerList(newPlayer);
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

    [Header("Scoreboard Management")]
    [SerializeField] private GameObject scoreboard = null;
    [Space]
    [SerializeField] private ScoreboardRow[] survivorRows = null;
    [SerializeField] private ScoreboardRow[] masterRows = null;
    [Space]
    [SerializeField] private bool scoreboardIsOpen = false;

    //TEMPORARY
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            OpenScoreboard();
        }
    }

    private void OpenScoreboard()
    {
        scoreboardIsOpen = !scoreboardIsOpen;
        scoreboard.SetActive(scoreboardIsOpen);

        if (scoreboardIsOpen)
        {
            UpdateScoreboard();
        }
    }

    private void UpdateScoreboard()
    {
        foreach (PlayerData pd in playerList)
        {
            foreach (ScoreboardRow row in pd.isMaster ? masterRows : survivorRows)
            {
                if (row.playerId == pd.playerId)
                {
                    row.ChangeScores(pd);
                    break;
                }
            }
        }
    }

    private void UpdateScoreboardRow(PlayerData pd)
    {
        foreach (ScoreboardRow row in pd.isMaster ? masterRows : survivorRows)
        {
            if (row.playerId == pd.playerId)
            {
                row.ChangeScores(pd);
                break;
            }
        }
    }

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

        if (!scoreboardIsOpen) OpenScoreboard(); //Open the scoreboard

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
        NetworkTest.Instance.StopHost();
        SceneManager.LoadScene(0);
    }

    #endregion

    [Header("Debug")]
    [SerializeField] private bool test = false;
}
