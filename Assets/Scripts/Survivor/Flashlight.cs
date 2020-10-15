using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Flashlight : NetworkBehaviour
{
    //public Transform cam;
    //public GameObject networkFlashlight;
    [SyncVar] public bool hasFlashlight;
    public GameObject myFlashlight;
    private void Start()
    {
        myFlashlight.SetActive(hasFlashlight);
        if (hasAuthority)
        {
            //CmdNewFlashlight();
        }
        else
        {
            myFlashlight.transform.parent = transform;
            myFlashlight.GetComponent<Light>().spotAngle = 75f;
        }
    }
    void Update()
    {
        if (hasAuthority)
        {
            if (myFlashlight)
            {
                //currentFlashlight.transform.parent = null;
                //currentFlashlight.transform.rotation = Quaternion.Slerp(currentFlashlight.transform.rotation, cam.rotation, 10 * Time.deltaTime);
                //currentFlashlight.transform.position = cam.position;
            }
            if (Input.GetKeyDown(KeyCode.F) && hasFlashlight)
            {
                CmdToggleFlashlight();   
                //else { CmdNewFlashlight(); }
            }
        }
    }

    [Command]
    void CmdToggleFlashlight()
    {
        RpcToggleFlashlight();
    }

    [ClientRpc]
    void RpcToggleFlashlight()
    {
        myFlashlight.SetActive(!myFlashlight.activeSelf);
    }


    [Command]
    void CmdNewFlashlight()
    {
        /*
        GameObject newFlash = (GameObject)Resources.Load("Flashlight");
        var newLight = Instantiate(newFlash, cam.position, cam.rotation, transform);
        Debug.Log("Spawning Flashlight for others");
        networkFlashlight = newFlash;
        NetworkServer.Spawn(networkFlashlight);
        */
    }

    [ClientRpc]
    void RpcNewFlashlight()
    {

        //CmdSpawnFlashlight(newLight);
    }

    [Command]
    void CmdSpawnFlashlight(GameObject newLight)
    {
       // NetworkServer.Spawn(newLight);
    }
}
