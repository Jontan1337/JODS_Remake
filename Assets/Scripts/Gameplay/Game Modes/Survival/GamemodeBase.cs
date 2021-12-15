using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        points = score;
    }

    //Shared stats
    [SyncVar] public uint playerId;
    [SyncVar] public string playerName;
    [SyncVar] public int score;
    [SyncVar] public bool isMaster;

    //Survivor Stats
    [SyncVar] public int points;
    [SyncVar] public int kills;

    //Master Stats
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
    TotalUnitUpgrades
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
    [SerializeField] private List<PlayerData> playerList = new List<PlayerData>();

    #region Point System and Player Scores

    [ClientRpc]
    private void Rpc_ChangePlayerList(PlayerData playerData)
    {
        playerList.Add(playerData);
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

        // playerList.Add(newPlayer);
        Rpc_ChangePlayerList(newPlayer);

        //Assign the player to a scoreboard row
        foreach (ScoreboardRow row in isMaster ? masterRows : survivorRows)
        {
            if (row.playerId == 0)
            {
                row.playerId = playerId;
                row.SetupPlayerScore(newPlayer);
                break;
            }
        }
    }

    #endregion

    public override void OnStartServer()
    {
        Lobby.OnPlayersLoaded += Initialize;
    }

    private void Start()
    {
        AS = GetComponent<AudioSource>();

        if (test)
        {
            Initialize();
        }
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

    [Header("Scoreboard")]
    [SerializeField] private GameObject scoreboard = null;
    [Space]
    [SerializeField] private ScoreboardRow[] survivorRows = null;
    [SerializeField] private ScoreboardRow[] masterRows = null;
    [Space]
    [SerializeField] private bool scoreboardIsOpen = false;

    //TEMPORARY
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
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

    [Header("Debug")]
    [SerializeField] private bool test = false;
}
