using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorPlayerData : BasePlayerData
{
    [Header("Survivor")]
    public int points;
    public int kills;

    #region Actions

    public Action<int> onPointsChanged;
    public Action<int> onKillsChanged;

    #endregion

    public int Points
    {
        get => points;
        set
        {
            points = value;
            onPointsChanged?.Invoke(value);
        }
    }
    public int Kills
    {
        get => kills;
        set
        {
            kills = value;
            onKillsChanged?.Invoke(value);
        }
    }
}
