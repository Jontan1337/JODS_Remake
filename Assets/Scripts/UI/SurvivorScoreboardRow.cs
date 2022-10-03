using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivorScoreboardRow : ScoreboardRow
{
    [Header("Survivor Stats")]
    [SerializeField] private Text pointsText = null; private const string pointsTextDefault = "Points: ";
    [SerializeField] private Text killsText = null; private const string killsTextDefault = "Kills: ";

    public void PointsText(int value) => pointsText.text = pointsTextDefault + value;
}
