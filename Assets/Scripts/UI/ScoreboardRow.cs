using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScoreboardRow : MonoBehaviour
{
    public uint playerId;
    [Space]
    [SerializeField] private Image overlayImage = null;
    [SerializeField] private Image foregroundImage = null;

    [Header("Shared Stats")]
    [SerializeField] private Text playerNameText = null;
    [SerializeField] private Text playerScoreText = null; private const string playerScoreTextDefault = "Score: ";



    public void ChangeScores(PlayerData playerData)
    {
        playerScoreText.text = playerScoreTextDefault + playerData.score;

        //Survivor
        
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
