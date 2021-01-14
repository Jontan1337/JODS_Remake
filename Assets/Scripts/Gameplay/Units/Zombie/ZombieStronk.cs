﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieStronk : UnitBase
{
    [Header("Stronk Weapons")]
    [SerializeField] private Mesh[] weaponMeshes = null;
    [SerializeField] private GameObject weapon = null;
    public ParticleSystem slamFX;

    public override void Start()
    {
        base.Start();
        weapon.GetComponent<MeshFilter>().mesh = weaponMeshes[Random.Range(0, weaponMeshes.Length)];
    }

    public override void Attack()
    {
        if (CanMeleeAttack)
        {
            MeleeAttack();
        }
    }
}
