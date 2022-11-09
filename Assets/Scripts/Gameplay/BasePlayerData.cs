using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BasePlayerData : NetworkBehaviour
{
    [Header("Shared")]
    [SerializeField] private string playerName = "Player 1";
    public uint playerId;

    [SerializeField, SyncVar] private int score;

    [SerializeField, SyncVar] private int exp;
    [SerializeField, SyncVar] private int expRequired;
    [SerializeField, SyncVar] private int level;

    private int baseExpRequired = 100;
    public int previousExpRequired = 0;


    public static BasePlayerData Instance;
    public override void OnStartAuthority()
    {
        Instance = this;
    }


    #region Actions

    public Action<string> onPlayerNameChanged;
    public Action<int> onScoreChanged;
    public Action<int> onExpChanged;
    public Action<int> onExpRequirementChanged;
    public Action<int> onLevelChanged;

    #endregion
    public string PlayerName
    {
        get => playerName;
        set
        {
            playerName = value;
            Rpc_PlayerName(value);
        }
    }
    [ClientRpc]
    private void Rpc_PlayerName(string value)
    {
        onPlayerNameChanged?.Invoke(value);
    }
    public int Score
    {
        get => score;
        set
        {
            score = value;
            Rpc_Score(value);
        }
    }

    [ClientRpc]
    private void Rpc_Score(int value)
    {
        onScoreChanged?.Invoke(value);
    }

    public int Level
    {
        get => level;
        set
        {
            level = value;
            Rpc_Level(value);
            int newXpReq = Mathf.RoundToInt(previousExpRequired > 0 ? Mathf.RoundToInt(previousExpRequired * 0.25f) : Mathf.RoundToInt(baseExpRequired * 0.25f));
            previousExpRequired = ExpRequired;
            ExpRequired += baseExpRequired + newXpReq;
        }
    }

    [ClientRpc]
    private void Rpc_Level(int value)
    {
        onLevelChanged?.Invoke(value);
    }

    public virtual int Exp
    {
        get => exp;
        set
        {
            exp = value;
            onExpChanged?.Invoke(value);
            if (exp >= ExpRequired) Level++;
        }
    }
    public int ExpRequired
    {
        get => expRequired;
        set
        {
            expRequired = value;
            onExpRequirementChanged?.Invoke(value);
        }
    }
    public int BaseExpRequired
    {
        get => baseExpRequired;
        set
        {
            baseExpRequired = value;
            ExpRequired = baseExpRequired;
        }
    }
}
