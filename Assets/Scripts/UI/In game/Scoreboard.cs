using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class Scoreboard : NetworkBehaviour
{
    #region Singleton
    public static Scoreboard Instance;
    private void Awake()
    {
        Instance = this;
        scoreboard.SetActive(false);
    }

    #endregion

    public override void OnStartClient()
    {
        JODSInput.Controls.General.Scoreboard.performed += OpenScoreboard;
    }

    [Header("Scoreboard Management")]
    [SerializeField] private GameObject scoreboard = null;
    [Space]
    [SerializeField] private ScoreboardRow[] survivorRows = null;
    [SerializeField] private ScoreboardRow[] masterRows = null;
    [Space]
    [SerializeField] private bool scoreboardIsOpen = false;

    [Server]
    public void Svr_AddSurvivor(SurvivorPlayerData data)
    {
        if (data == null)
        {
            Debug.LogError("New Player (survivor) had no SurvivorPlayerData!");
            return;
        }

        //SurvivorScoreboardRow newPlayerRow = null;
        //foreach(SurvivorScoreboardRow scoreboardRow in survivorRows)
        //{
        //    if (scoreboardRow.playerId == 0)
        //    {
        //        newPlayerRow = scoreboardRow;
        //        break;
        //    }
        //}

        //if (newPlayerRow == null)
        //{
        //    Debug.LogError("New Player (survivor) was not assigned to a scoreboard row!");
        //    return;
        //}


        //newPlayerRow.SetPlayerAliveStatus(true);

        //newPlayerRow.SetPlayerNameText(data.playerName);

        Rpc_AddSurvivor(data);
    }

    [ClientRpc]
    private void Rpc_AddSurvivor(SurvivorPlayerData data)
    {
        SurvivorScoreboardRow newPlayerRow = null;
        foreach (SurvivorScoreboardRow scoreboardRow in survivorRows)
        {
            if (scoreboardRow.playerId == 0)
            {
                newPlayerRow = scoreboardRow;
                break;
            }
        }

        if (newPlayerRow == null)
        {
            Debug.LogError("New Player (survivor) was not assigned to a scoreboard row!");
            return;
        }

        data.onKillsChanged += newPlayerRow.SetKillsText;
        data.onLevelChanged += newPlayerRow.SetPlayerLevelText;
        data.onScoreChanged += newPlayerRow.SetPlayerScoreText;

        newPlayerRow.SetPlayerAliveStatus(true);

        newPlayerRow.SetPlayerNameText(data.playerName);
    }

    [Server]
    public void Svr_AddMaster(MasterPlayerData data)
    {
        if (data == null)
        {
            Debug.LogError("New Player (master) had no MasterPlayerData!");
            return;
        }

        //MasterScoreboardRow newPlayerRow = null;
        //foreach (MasterScoreboardRow scoreboardRow in masterRows)
        //{
        //    if (scoreboardRow.playerId == 0)
        //    {
        //        newPlayerRow = scoreboardRow;
        //        break;
        //    }
        //}

        //if (newPlayerRow == null)
        //{
        //    Debug.LogError("New Player (master) was not assigned to a scoreboard row!");
        //    return;
        //}

        //data.onLevelChanged += newPlayerRow.SetPlayerLevelText;
        //data.onScoreChanged += newPlayerRow.SetPlayerScoreText;
        //data.onTotalUnitUpgradesChanged += newPlayerRow.SetTotalUnitUpgradesText;
        //data.onUnitsPlacedChanged += newPlayerRow.SetUnitsPlacedText;
        //newPlayerRow.SetPlayerAliveStatus(true);

        //newPlayerRow.SetPlayerNameText(data.playerName);

        Rpc_AddMaster(data);
    }

    [ClientRpc]
    private void Rpc_AddMaster(MasterPlayerData data)
    {
        MasterScoreboardRow newPlayerRow = null;
        foreach (MasterScoreboardRow scoreboardRow in masterRows)
        {
            if (scoreboardRow.playerId == 0)
            {
                newPlayerRow = scoreboardRow;
                break;
            }
        }

        if (newPlayerRow == null)
        {
            Debug.LogError("New Player (master) was not assigned to a scoreboard row!");
            return;
        }

        data.onLevelChanged += newPlayerRow.SetPlayerLevelText;
        data.onScoreChanged += newPlayerRow.SetPlayerScoreText;
        data.onTotalUnitUpgradesChanged += newPlayerRow.SetTotalUnitUpgradesText;
        data.onUnitsPlacedChanged += newPlayerRow.SetUnitsPlacedText;
        newPlayerRow.SetPlayerAliveStatus(true);

        newPlayerRow.SetPlayerNameText(data.playerName);
    }

    private void OpenScoreboard(InputAction.CallbackContext context)
    {
        scoreboardIsOpen = !scoreboardIsOpen;
        scoreboard.SetActive(scoreboardIsOpen);
    }

    public void OpenScoreboard(bool enable)
    {
        scoreboard.SetActive(enable);
    }
}
