using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Ammo : NetworkBehaviour
{
    [SyncVar] public int shotgunAmmo;
    [SyncVar] public int rifleAmmo;
    [SyncVar] public int pistolAmmo;

    [SyncVar] public bool shotgun;
    [SyncVar] public bool rifle;
    [SyncVar] public bool pistol;

    public GameObject shotgunModel;
    public GameObject rifleModel;
    public GameObject pistolModel;

    public int tier;

    void Start()
    {
        pistol = true;

        if (tier == 2)
        {
            shotgun = Random.Range(0, 10) < 4;
            rifle = Random.Range(0, 10) < 4;
        }

        if (tier == 3)
        {
            shotgun = Random.Range(0, 5) < 4;
            rifle = Random.Range(0, 5) < 4;
        }

        shotgunModel.SetActive(shotgun);
        rifleModel.SetActive(rifle);
        pistolModel.SetActive(pistol);

        if (shotgun) { shotgunAmmo = Random.Range(10, 20); }
        if (rifle) { rifleAmmo = Random.Range(30, 60); }
        if (pistol) { pistolAmmo = Random.Range(14, 35); }
    }


    /// <summary>
    /// What ammo ? 1 = Shotgun, 2 = Rifle, 3 = Pistol
    /// </summary>
    /// <param name="which"></param>
    [ClientRpc]
    public void RpcPickUpAmmo(int which, GameObject go)
    {
        if (which == 1 && shotgun)
        {
            shotgun = false;
            shotgunModel.SetActive(shotgun);
            go.GetComponent<Shoot>().weaponStats.ammunition += shotgunAmmo;
            if (!pistol && !rifle && !shotgun) { Destroy(gameObject); }
        }
        else if (which == 2 && rifle)
        {
            rifle = false;
            rifleModel.SetActive(rifle);
            go.GetComponent<Shoot>().weaponStats.ammunition += rifleAmmo;
            if (!pistol && !rifle && !shotgun) { Destroy(gameObject); }
        }
        else if (which == 3 && pistol)
        {
            pistol = false;
            pistolModel.SetActive(pistol);
            go.GetComponent<Shoot>().weaponStats.ammunition += pistolAmmo;
            if (!pistol && !rifle && !shotgun) { Destroy(gameObject); }
        }
        else
        {

        }
    }
}
