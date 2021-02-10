using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierClass : SurvivorClass
{
    SurvivorStats sStats;
    GameObject rocketSpawnPos;
    int rocketSpeed = 5000;

    private void Awake()
    {
        sStats = GetComponent<SurvivorStats>();
        sStats.armor = 80;
        rocketSpawnPos = new GameObject("rocketSpawnPos");
        rocketSpawnPos.transform.SetParent(transform);
        rocketSpawnPos.transform.position = new Vector3(0.5f, 2, 0.5f);
    }


    public override void ActiveAbility()
    {        
        //CmdRocketLaunch();
    }



    [Command]
    void CmdRocketLaunch()
    {
        GameObject currentRocket = Instantiate(abilityObject, new Vector3(rocketSpawnPos.transform.position.x , rocketSpawnPos.transform.position.y, rocketSpawnPos.transform.position.z), transform.GetComponent<LookController>().playerCamera.transform.rotation);
        NetworkServer.Spawn(currentRocket);
        RpcRocketLaunch(currentRocket);
    }

    [ClientRpc]
    void RpcRocketLaunch(GameObject currentRocket)
    {
        Rigidbody rb = currentRocket.GetComponent<Rigidbody>();
        rb.AddForce(transform.GetComponent<LookController>().playerCamera.transform.forward * rocketSpeed);
    }
}
