using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterScoreboardRow : ScoreboardRow
{
    [Header("Master UI References")]
    [SerializeField] private Text unitsPlacedText = null; private const string unitsPlacedTextDefault = "Units Placed: ";
    [SerializeField] private Text totalUnitUpgradesText = null; private const string totalUnitUpgradesTextDefault = "Total Unit Upgrades: ";

    public void SetUnitsPlacedText(int value) => unitsPlacedText.text = unitsPlacedTextDefault + value;
    public void SetTotalUnitUpgradesText(int value) => totalUnitUpgradesText.text = totalUnitUpgradesTextDefault + value;
}
