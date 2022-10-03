using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerData : MonoBehaviour
{
    [Header("Shared")]
    public string playerName;
    public uint playerId;

    private int score;

    private int exp;
    public int expRequired;
    private int level;


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
