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
        } 
    }
    public int Exp { get => exp; set => exp = value; }

}
