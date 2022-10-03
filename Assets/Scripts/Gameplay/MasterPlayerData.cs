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
            unitsPlaced = value;
            onUnitsPlacedChanged?.Invoke(value);
        }
    }
    public int TotalUnitUpgrades
    {
        get => totalUnitUpgrades;
        set
        {
            totalUnitUpgrades = value;
            onTotalUnitUpgradesChanged?.Invoke(value);
        }
    }

}
