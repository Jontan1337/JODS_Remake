﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardRow : MonoBehaviour
{
    public uint playerId;
    [Space]
    [SerializeField] private Image overlayImage = null;
    [SerializeField] private Image foregroundImage = null;

    [Header("Shared Stats")]
    [SerializeField] private Text playerNameText = null;
    [SerializeField] private Text playerScoreText = null; private const string playerScoreTextDefault = "Score: ";
    [Header("Survivor Stats")]
    [SerializeField] private Text pointsText = null; private const string pointsTextDefault = "Points: ";
    [SerializeField] private Text killsText = null; private const string killsTextDefault = "Kills: ";
    [Header("Master Stats")]
    [SerializeField] private Text unitsPlacedText = null; private const string unitsPlacedTextDefault = "Units Placed: ";
    [SerializeField] private Text totalUpgradesText = null; private const string totalUpgradesTextDefault = "Total Upgrades: ";
    [SerializeField] private Text totalUnitUpgradesText = null; private const string totalUnitUpgradesTextDefault = "Total Unit Upgrades: ";

    public void ChangeScores(PlayerData playerData)
    {
        playerScoreText.text = playerScoreTextDefault + playerData.score;

        //Survivor
        if (pointsText) pointsText.text = pointsTextDefault + playerData.points;
        if (killsText) killsText.text = killsTextDefault + playerData.kills;

        //Master
        if (unitsPlacedText) unitsPlacedText.text = unitsPlacedTextDefault + playerData.unitsPlaced;
        if (totalUpgradesText) totalUpgradesText.text = totalUpgradesTextDefault + playerData.totalUpgrades;
        if (totalUnitUpgradesText) totalUnitUpgradesText.text = totalUnitUpgradesTextDefault + playerData.totalUnitUpgrades;

        //UI overlay
        Color col = overlayImage.color;
        col.a = playerData.alive ? 0 : 0.2f;
        overlayImage.color = col;
    }

    public void SetupPlayerScore(PlayerData playerData)
    {
        foregroundImage.enabled = false;

        playerNameText.text = playerData.playerName;
        playerId = playerData.playerId;

        ChangeScores(playerData);
    }
}
