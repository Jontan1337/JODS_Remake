using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterPlayerData : BasePlayerData
{
    [Header("Master")]
    [SerializeField] private int unitsPlaced;
    [SerializeField] private int totalUnitUpgrades;

    #region Actions

    public Action<int> onUnitsPlacedChanged;
    public Action<int> onTotalUnitUpgradesChanged;

    #endregion

    public int UnitsPlaced
    {
        get => unitsPlaced;
        set
        {
            Score += 10;
            unitsPlaced = value;
            onUnitsPlacedChanged?.Invoke(value);
        }
    }
    public int TotalUnitUpgrades
    {
        get => totalUnitUpgrades;
        set
        {
            Score += 100;
            totalUnitUpgrades = value;
            onTotalUnitUpgradesChanged?.Invoke(value);
        }
    }

}
