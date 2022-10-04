using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BasePlayerData : NetworkBehaviour
{
    [Header("Shared")]
    public string playerName = "Player 1";
    public uint playerId;

    [SerializeField] private int score;

    [SerializeField] private int exp;
    [SerializeField] private int expRequired;
    [SerializeField] private int level;

    private int baseExpRequired = 100;
    public int previousExpRequired = 0;


    #region Actions

    public Action<int> onScoreChanged;
    public Action<int> onExpChanged;
    public Action<int> onExpRequirementChanged;
    public Action<int> onLevelChanged;

    #endregion

    public int Score
    {
        get => score;
        set
        {
            score = value;
            onScoreChanged?.Invoke(value);
        }
    }
    public int Level
    {
        get => level;
        set
        {
            level = value;
            onLevelChanged?.Invoke(value);
            int newXpReq = Mathf.RoundToInt(previousExpRequired > 0 ? Mathf.RoundToInt(previousExpRequired * 0.25f) : Mathf.RoundToInt(baseExpRequired * 0.25f));
            previousExpRequired = ExpRequired;
            ExpRequired += newXpReq;
        }
    }
    public int Exp
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
