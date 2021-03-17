using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineRoundManager : MonoBehaviour
{
    [SerializeField] private int round = 0;
    [Space]
    [SerializeField] private OfflineUnitSpawner unitSpawner = null;

    #region Round Methods

    public void StartGame()
    {
        if (unitSpawner == null)
        {
            Debug.LogError("Unit Spawner reference not set. Cannot start.");
            return;
        }

        Invoke(nameof(NextRound), 2f);
    }

    private void NextRound()
    {
        round += 1;
        Debug.Log($"Round {round} has begun.");

        unitSpawner.RoundStart(round);
    }

    #endregion
}
