﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : RangedWeapon
{
    [Header("Projectile Settings")]
    [SerializeField] private int projectileSpeed = 30;
    [SerializeField] private float timeToLive = 5f;
    [SerializeField] private string projectileTag = "Bullet";

    protected override void Shoot()
    {
        base.Shoot();
        Rpc_ShootFX();
        GameObject projectile = ObjectPool.Instance.SpawnFromPool(projectileTag, shootOrigin.position, shootOrigin.rotation, timeToLive);
        projectile.GetComponent<Rigidbody>().AddForce(shootOrigin.forward * projectileSpeed, ForceMode.Impulse);
    }
}