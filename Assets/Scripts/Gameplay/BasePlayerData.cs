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
    private int previousExpRequired = 0;


    #region Actions

    public Action<int> onScoreChanged;
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
            previousExpRequired = baseExpRequired + Mathf.RoundToInt(previousExpRequired * 0.25f);
            expRequired += previousExpRequired;
        }
    }
    public int Exp
    {
        get => exp;
        set
        {
            exp = value;
            if (exp >= expRequired) Level++;
        }
    }

    public int BaseExpRequired
    {
        get => baseExpRequired;
        set
        {
            baseExpRequired = value;
            expRequired = baseExpRequired;
            previousExpRequired = expRequired;
        }
    }
}
