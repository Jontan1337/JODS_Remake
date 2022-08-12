using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Molotov : Projectile
{
    [SerializeField] private GameObject fireSpreadPrefab = null;
    public override void OnHit(Collision objectHit)
    {
        GameObject fire = Instantiate(fireSpreadPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(fire);

        Destroy();
    }
}
