using System.Collections;
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
        Debug.LogError("MY UNIT BASE'S START DOES NOT RUN BECAUSE I AM OVERRIDING START MYSELF. FIX PLS");
        weapon.GetComponent<MeshFilter>().mesh = weaponMeshes[Random.Range(0, weaponMeshes.Length)];
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }
}
