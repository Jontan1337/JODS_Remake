using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Scoreboard : NetworkBehaviour
{
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
        ScoreboardRow newPlayerRow = null;
        foreach(ScoreboardRow scoreboardRow in survivorRows)
        {
            if (scoreboardRow.playerId == 0)
            {
                newPlayerRow = scoreboardRow;
            }
        }

        if (newPlayerRow == null)
        {
            Debug.LogError("New Player was not assigned to a scoreboard row!");
            return;
        }

        data.onKillsChanged += surv
        data.onLevelChanged
        data.onPointsChanged
        data.onScoreChanged
    }

    [Server]
    public void Svr_AddMaster(MasterPlayerData data)
    {

    }
}
