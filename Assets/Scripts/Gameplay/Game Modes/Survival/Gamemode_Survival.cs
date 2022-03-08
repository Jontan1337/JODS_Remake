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
    private readonly string timeSurvivedTextDefault = "Time Survived: \n ";

    [Header("Shop Period Management")]
    [SerializeField] private int timeTillNextShopPeriod = 0;
    [Space]
    [SerializeField] private int timeBetweenShopPeriods = 120; //2 minutes between each shop period
    [SerializeField] private int shopPeriodDuration = 30; //each shop period lasts 30 seconds
    [Space]
    [SerializeField] private GameObject shopPrefab = null;
    [Space]
    [SerializeField] private AudioClip shopPeriodSpawnAudio = null;

    public override void InitializeGamemode()
    {
        timeTillNextShopPeriod = timeBetweenShopPeriods;
        timeSurvivedText.enabled = true;
        timeSurvivedText.text = timeSurvivedTextDefault + "00:00";

        survivalTimerCo = StartCoroutine(SurvivalTimer());
    }

    Coroutine survivalTimerCo; //To stop the coroutine eventually
    private IEnumerator SurvivalTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            
            timeSurvived++;
            UpdateSurvivalTimer(timeSurvived);

            timeTillNextShopPeriod--;
            if (timeTillNextShopPeriod <= 0)
            {
                timeTillNextShopPeriod = timeBetweenShopPeriods + shopPeriodDuration;
                NewShopPeriod();
            }
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
        if (!mapSettings) return;

        print("SHOP IS OPEN GO GO GO!");

        AS.PlayOneShot(shopPeriodSpawnAudio, 1f);

        PositionAndRotationPoint spawnpoint = mapSettings.shopSpawnPoints[Random.Range(0, mapSettings.shopSpawnPoints.Length)];

        GameObject newShop = Instantiate(shopPrefab, spawnpoint.position, spawnpoint.rotation);

        NetworkServer.Spawn(newShop);

        newShop.GetComponent<WeaponShop>().Svr_GenerateNewSelection();

        StartCoroutine(IE_DestroyShop(newShop));
    }
    private IEnumerator IE_DestroyShop(GameObject shop)
    {
        yield return new WaitForSeconds(shopPeriodDuration);

        NetworkServer.Destroy(shop);
    }

    public override void EndGame()
    {
        StopCoroutine(survivalTimerCo);
    }
}
