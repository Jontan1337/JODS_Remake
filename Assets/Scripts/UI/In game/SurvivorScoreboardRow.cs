using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivorScoreboardRow : ScoreboardRow
{
    [Header("Survivor UI References")]
    [SerializeField] private Text killsText = null; private const string killsTextDefault = "Kills: ";

    public void SetKillsText(int value) => killsText.text = killsTextDefault + value;
}
