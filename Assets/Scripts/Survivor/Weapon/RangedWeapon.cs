using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RangedWeapon : NetworkBehaviour
{
    [Header("Basic settings")]
    [SerializeField]
    private string weaponName = "Weapon name";
    [SerializeField]
    private int damage = 0;
    [SerializeField]
    private float range = 0f;
    [SerializeField]
    private float fireRate = 0f;

    [Header("Weapon stats")]
    [SerializeField, SyncVar]
    private int ammunition = 10;
    [SerializeField, SyncVar]
    private int extraAmmunition = 20;

    [Header("Game details")]
    [SerializeField, SyncVar]
    private string player;

    [Header("References")]
    [SerializeField]
    private Animator weaponAnimator = null;

    private void Awake()
    {
        // Setup controls
    }

    private void Shoot()
    {

    }
}
