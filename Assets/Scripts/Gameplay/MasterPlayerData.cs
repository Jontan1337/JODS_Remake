using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MasterPlayerData : BasePlayerData
{
    [Header("Master")]
    [SerializeField] private int unitsPlaced;
    [SerializeField] private int totalUnitUpgrades;
    [SerializeField] private int unitUpgradePoints;

    #region Actions

    public Action<int> onUnitsPlacedChanged;
    public Action<int> onTotalUnitUpgradesChanged;
    public Action<int> onUnitUpgradePointsChanged;

    #endregion

    public override int Exp 
    { 
        get => base.Exp;
        set
        {
            if (value > Exp) // Only gain points when gaining exp.
            {
                int diff = value - Exp;
                UnitUpgradePoints += diff;
            }
            base.Exp = value;
        }
    }



    public int UnitUpgradePoints
    {
        get => unitUpgradePoints;
        set
        {
            unitUpgradePoints = value;
            onUnitUpgradePointsChanged?.Invoke(value);
        }
    }


    public int UnitsPlaced
    {
        get => unitsPlaced;
        set
        {
            Score += 10;
            unitsPlaced = value;
            Rpc_UnitsPlaced(value);
        }
    }
    [ClientRpc]
    private void Rpc_UnitsPlaced(int value)
    {
        onUnitsPlacedChanged?.Invoke(value);
    }
    public int TotalUnitUpgrades
    {
        get => totalUnitUpgrades;
        set
        {
            Score += 100;
            totalUnitUpgrades = value;
            Rpc_TotalUnitUpgrades(value);
        }
    }
    [ClientRpc]
    private void Rpc_TotalUnitUpgrades(int value)
    {
        onTotalUnitUpgradesChanged?.Invoke(value);
    }
}
