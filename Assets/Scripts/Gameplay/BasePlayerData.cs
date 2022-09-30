using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerData : MonoBehaviour
{
    [Header("Shared")]
    public string playerName;
    public uint playerId;

    public int score;
    
    public int exp;
    public int expRequired;
    public int level;
}
