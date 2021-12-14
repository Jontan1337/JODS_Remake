using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

[System.Serializable]
public struct PlayerData
{
    public PlayerData(uint playerId, string playerName, int score) : this()
    {
        this.playerId = playerId;
        this.playerName = playerName;
        this.score = score;
    }

    //Shared stats
    public uint playerId;
    public string playerName;
    public int score;

    //Survivor Stats
    public int points;
    public int kills;

    //Master Stats
    public int unitsPlaced;
    public int totalUpgrades;
    public int totalUnitUpgrades;
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

    [Server]
    public int Svr_GetPoints(uint playerId)
    {
        return 0;
        //return playerList[player];
    }

    [Server]
    public void Svr_ModifyPoints(uint playerId, int amount)
    {
        //playerList[player] += amount;
    }

    [Server]
    public void Svr_AddPlayer(uint playerId, string playerName)
    {
        playerList.Add(new PlayerData(playerId, playerName, defaultStartingPoints));
    }

    [Header("Scoreboard")]
    [SerializeField] private GameObject scoreboard = null;

    //TEMPORARY
    private void Update()
    {

        scoreboard.SetActive(Input.GetKey(KeyCode.Tab));

    }

    public override void OnStartServer()
    {
        Lobby.OnPlayersLoaded += Initialize;
    }

    private void Start()
    {
        AS = GetComponent<AudioSource>();
    }

    private void Initialize()
    {
        StartCoroutine(IEGameStartCountdown());
    }

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

    public abstract void InitializeGamemode();
}
