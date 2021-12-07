using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class Gamemode_Survival : GamemodeBase
{
    [Header("Survival Timer Settings")]
    [SerializeField] private int timeSurvived = 0;
    [SerializeField] private Text timeSurvivedText = null;
    private string timeSurvivedTextDefault = "Time Survived: \n ";

    [Header("Points System Management")]
    [SerializeField] private int numberOfPlayers = 0;
    private Dictionary<GameObject, int> playersAndPoints = new Dictionary<GameObject, int>();

    [Header("Shop Period Management")]
    [SerializeField] private int timeTillNextShopPeriod = 0;
    [Space]
    [SerializeField] private int timeBetweenShopPeriods = 120; //2 minutes between each shop period
    [SerializeField] private int shopPeriodDuration = 30; //each shop period lasts 30 seconds
    private List<Vector3> shopSpawnpoints = new List<Vector3>();

    public override void InitializeGamemode()
    {
        timeTillNextShopPeriod = timeBetweenShopPeriods;
        timeSurvivedText.enabled = true;
        timeSurvivedText.text = timeSurvivedTextDefault + "00:00";

        survivalTimerCo = StartCoroutine(SurvivalTimer());
    }

    Coroutine survivalTimerCo;
    private IEnumerator SurvivalTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            
            timeSurvived++;
            UpdateSurvivalTimer(timeSurvived);

            timeTillNextShopPeriod--;
            if (timeTillNextShopPeriod >= 0) NewShopPeriod();
        }
    }

    [ClientRpc]
    private void UpdateSurvivalTimer(int time)
    {
        float minutes = Mathf.Floor(time / 60);
        float seconds = Mathf.RoundToInt(time % 60);

        timeSurvivedText.text = timeSurvivedTextDefault + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void NewShopPeriod()
    {

    }


}
