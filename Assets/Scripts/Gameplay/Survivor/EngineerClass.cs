using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EngineerClass : NetworkBehaviour
{
    private Stats stats;
    public GameObject turret;
    private PlaceItem placer;
    public GameObject wrench;
    private Shoot shoot;
    private float cooldownRemaining = 0;

    void Start()
    {
        if (!hasAuthority) { return; }
        shoot = GetComponent<Shoot>();
        stats = GetComponent<Stats>();
        cooldownRemaining = stats.maxCooldown;
        placer = GetComponent<PlaceItem>();
    }

    void Update()
    {
        if (!hasAuthority) { return; }
        if (!shoot.hasWeapon)
        {
            StarterWeapon();
        }
        stats.SpecialCooldown = cooldownRemaining;
        if (Input.GetKey(KeyCode.C) && cooldownRemaining >= stats.maxCooldown)
        {
            placer.item = turret;
            placer.Place(true);
        }
        else if (Input.GetKeyUp(KeyCode.C) && cooldownRemaining >= stats.maxCooldown)
        {
            if (placer.Place(false))
            {
                cooldownRemaining = 0;
                Debug.Log("Place turret");
            }
            //placer.Place(false);
        }
        if (cooldownRemaining < stats.maxCooldown)
        {
            cooldownRemaining += Time.deltaTime;
        }
    }
    void StarterWeapon()
    {
        //shoot.hasWeapon = false;
        shoot.StarterWeapon(wrench.name);
    }

    void SpawnTurret()
    {
        //GameObject currentTurret = Instantiate(turret, new Vector3(turretSpawnPosition.transform.position.x, turretSpawnPosition.transform.position.y, turretSpawnPosition.transform.position.z), transform.rotation);
        //currentTurret.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * 10);
    }
}
