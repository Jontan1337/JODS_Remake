using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterPlayerData : BasePlayerData
{
    [Header("Master")]
    public int unitsPlaced;
    public int totalUpgrades;
    public int totalUnitUpgrades;

    #region Actions

    public Action<int> onUnitsPlacedChanged;
    public Action<int> onTotalUpgradesChanged;
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
    public int TotalUpgrades
    {
        get => totalUpgrades;
        set
        {
            totalUpgrades = value;
            onTotalUpgradesChanged?.Invoke(value);
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
