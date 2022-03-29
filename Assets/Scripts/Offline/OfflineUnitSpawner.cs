using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class EnemyUnit
{
    public UnitSO unit;
    public int spawnChance = 0;
    public int level = 1;
}

public class OfflineUnitSpawner : NetworkBehaviour
{ 
    [Header("Master and Unit settings")]
    [SerializeField] private UnitMasterSO masterSO = null;
    [SerializeField] private List<EnemyUnit> unitList = new List<EnemyUnit>();

    [Header("Unit Numbers")]
    [SerializeField] private int unitsLeftToSpawn = 0;
    //[SerializeField] private int maxUnitsAlive = 0;
    //[SerializeField] private int aliveUnits = 0;

    [Header("References")]
    [SerializeField] private Transform[] spawnPoints = null;
    [SerializeField] private Transform player = null;

    [Header("Other")]
    [SerializeField] private int spawnRadius = 8;

    private int round;


    public void SetupUnitSpawner()
    {
        Invoke(nameof(SetupEnemy),1f);
    }

    private void SetupEnemy()
    {
        if (masterSO == null)
        {
            Debug.LogError("MasterSO was null, round could not start");
            return;
        }

        //This is temporary, to find the player
        player = GameObject.FindGameObjectWithTag("Player").transform;

        //Assign all the possible units to spawn
        foreach (UnitSO newUnit in masterSO.units)
        {
            unitList.Add(new EnemyUnit { unit = newUnit });
        }

        //Set the default unit (first in the list) to have a spawn chance of 100
        unitList[0].spawnChance = 100; //The first unit on the list is always the default unit of the class

        //Invert the list of units. This is done because when the units get spawned, it iterates through each of their spawn chance.
        //If default was first, it would get chosen every time, because of the 100% spawn chance.
        //If no other unit gets chosen, then the default unit will always be chosen.
        unitList.Reverse();
    }


    public void RoundStart(int newRound)
    {
        round = newRound;

        //Units in this round
        unitsLeftToSpawn = round * 10; //Temporary number. Need a good way to determine how many

        //Max amount of units that can be alive on the map this round
        //maxUnitsAlive = (unitsLeftToSpawn / 3) * 2;

        //aliveUnits = 0; //This keeps track of how many units are alive on the map. Reset to 0 each round.

        //Debug.Log($"Spawning {unitsLeftToSpawn / 2} units.");
        //Debug.Log($"{unitsLeftToSpawn / 3} units left to spawn this round.");

        //Spawn a bunch of units at the beginning of the round.
        SpawnUnit(unitsLeftToSpawn / 2);

        //Then start the coroutine which continually spawns units until enough units have been spawned.
        StartCoroutine(SpawnCoroutine());
    }

    public void SpawnUnit(int amount)
    {
        //Spawn the amount of units specified
        for (int i = 0; i < amount; i++)
        {
            foreach(EnemyUnit enemyUnit in unitList)
            {
                //Iterate through all units. Choose which unit to spawn by comparing their spawn chance with a random number.
                if (enemyUnit.spawnChance > Random.Range(0, 100))
                {
                    //Choose the spawn point
                    Vector3 spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
                    //Set the units spawn position to be random within a radius of the spawn point
                    Vector3 spawnPosition = new Vector3(spawnPoint.x + Random.Range(-spawnRadius, spawnRadius), 0, spawnPoint.z + Random.Range(-spawnRadius, spawnRadius));

                    //Set the units rotation to be random
                    Quaternion spawnRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));

                    //Choose this unit to spawn.
                    GameObject newUnit = Instantiate(enemyUnit.unit.unitPrefab, spawnPosition, spawnRotation);

                    UnitBase unit = newUnit.GetComponent<UnitBase>();

                    unit.SetUnitSO(enemyUnit.unit);

                    unitsLeftToSpawn -= 1;
                    Debug.Log("Units left to spawn : " + unitsLeftToSpawn);

                    //Set the target of the unit
                    StartCoroutine(SetUnitTarget(unit));
                }
            }
        }
    }

    private IEnumerator SetUnitTarget(UnitBase unit)
    {
        //The delay is because the units needs to initialize itself before a target can be set.
        yield return new WaitForSeconds(1);

        unit.SetPermanentTarget(player);
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            //Wait one second between spawning each unit
            yield return new WaitForSeconds(1);

            //Check if there are any units left to spawn
            if(unitsLeftToSpawn == 0)
            {
                //Stop the coroutine
                yield break;
            }

            //Spawn the unit
            SpawnUnit(1);
        }
    }

}
