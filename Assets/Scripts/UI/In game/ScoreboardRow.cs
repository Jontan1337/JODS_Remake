using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScoreboardRow : MonoBehaviour
{
    public uint playerId;

    [Header("Shared UI References")]
    [SerializeField] private Image overlayImage = null;
    [SerializeField] private Image foregroundImage = null;
    [Space]
    [SerializeField] private Text playerNameText = null;
    [SerializeField] private Text playerScoreText = null; private const string playerScoreTextDefault = "Score: ";
    [SerializeField] private Text playerLevelText = null; private const string playerLevelTextDefault = "Level: ";

    public void SetPlayerAliveStatus(bool alive)
    {
        if (alive) foregroundImage.enabled = false;

        //UI overlay
        Color col = overlayImage.color;
        col.a = alive ? 0 : 0.2f;
        overlayImage.color = col;
    }
    public void SetPlayerNameText(string value) => playerNameText.text = value;
    public void SetPlayerScoreText(int value) => playerScoreText.text = playerScoreTextDefault + value;
    public void SetPlayerLevelText(int value) => playerLevelText.text = playerLevelTextDefault + value;
}
