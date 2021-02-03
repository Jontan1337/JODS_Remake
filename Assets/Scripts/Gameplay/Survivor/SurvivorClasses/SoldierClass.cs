using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierClass : SurvivorClass
{
    public GameObject rocket;
    int rocketSpeed;


    public override void ActiveAbility()
    {
        CmdRocketLaunch();
        print("BANG");
    }

    [Command]
    void CmdRocketLaunch()
    {
        GameObject currentRocket = Instantiate(rocket, (new Vector3(transform.localPosition.x, transform.localPosition.y + 2, transform.localPosition.z + 1)), transform.rotation);
        NetworkServer.Spawn(currentRocket);
        RpcRocketLaunch(currentRocket);
    }

    [ClientRpc]
    void RpcRocketLaunch(GameObject currentRocket)
    {
        Rigidbody rb = currentRocket.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * rocketSpeed);
    }
}
