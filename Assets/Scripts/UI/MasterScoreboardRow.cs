using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterScoreboardRow : ScoreboardRow
{
    [Header("Master Stats")]
    [SerializeField] private Text unitsPlacedText = null; private const string unitsPlacedTextDefault = "Units Placed: ";
    [SerializeField] private Text totalUpgradesText = null; private const string totalUpgradesTextDefault = "Total Upgrades: ";
    [SerializeField] private Text totalUnitUpgradesText = null; private const string totalUnitUpgradesTextDefault = "Total Unit Upgrades: ";
}
