using System.Collections;
using UnityEngine;
using Mirror;

public class PlayerSpawner : NetworkBehaviour
{
    public GameObject[] survivorPrefab;
    public GameObject[] masterPrefab;
    public int characterNum;
    public int survivorNum;
    public int masterNum;
    private GameObject[] spawns;
    private int spawn;
    public GameObject cam;
    private bool spawned;

    public override void OnStartAuthority()
    {
        // Check if this is currently the server and if the object is is not the same as the host.
        if (!hasAuthority) return;

        //spawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        spawns = new GameObject[0];
        if (spawns.Length != 0 && !spawned)
        {
            spawned = true;
            characterNum = PlayerPrefs.GetInt("Character");
            spawn = Random.Range(0, spawns.Length);

            print($"Survivor Int: {PlayerPrefs.GetInt("Survivor")}");
            print($"Master Int: {PlayerPrefs.GetInt("Master")}");
            // Survivor - 1
            if (characterNum == 1)
            {
                // Soldier - 0
                // Taekwondo - 1
                // Engineer - 2

                survivorNum = PlayerPrefs.GetInt("Survivor");
                Cmd_ReplacePlayer(gameObject, survivorNum, true, spawns, spawn);
                return;
            }

            // Master - 0
            if (characterNum == 0)
            {
                // Zombie - 0
                masterNum = PlayerPrefs.GetInt("Master");
                Cmd_ReplacePlayer(gameObject, masterNum, false, spawns, spawn);
                return;
            }
        }
        if (spawns.Length == 0 && !spawned)
        {
            spawned = true;
            survivorNum = PlayerPrefs.GetInt("Survivor");
            Cmd_ReplacePlayer(gameObject, survivorNum, true, null, 0);
            return;
        }
    }

    [Command]
    void Cmd_ReplacePlayer(GameObject playerOwner, int num, bool survivor, GameObject[] spawnPoints, int spawnNum)
    {
        GameObject go = null;
        if (survivor)
        {
            if (spawnPoints != null)
            {
                go = Instantiate(survivorPrefab[num], spawnPoints[spawnNum].transform.position, spawnPoints[spawnNum].transform.rotation);
            }
            else
            {
                go = Instantiate(survivorPrefab[num], transform.position, transform.rotation);
            }
        }
        else
        {
            go = Instantiate(masterPrefab[num], transform.position, transform.rotation);
        }

        print($"New Player: {go.name} For {playerOwner.name}");

        NetworkServer.ReplacePlayerForConnection(connectionToClient, go);

        NetworkServer.Destroy(gameObject);

        //if (NetworkServer.ReplacePlayerForConnection(connectionToClient, go))
        //{
        //    if (spawnPoints.Length != 0)
        //    {
        //        Destroy(spawnPoints[spawnNum]);
        //    }
        //    NetworkServer.Destroy(gameObject);
        //}
    }
}
