using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RangedWeapon : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private string weaponName = "Weapon name";
    [SerializeField]
    private LayerMask ignoreLayer;

    [Header("Weapon stats")]
    [SerializeField]
    private int damage = 0;
    [SerializeField]
    private float range = 0f;
    [SerializeField]
    private float fireRate = 0f;
    [SerializeField]
    private int bulletsPerShot = 1;
    [SerializeField, SyncVar]
    private int currentAmmunition = 10;
    [SerializeField]
    private int maxCurrentAmmunition = 10;
    [SerializeField, SyncVar]
    private int extraAmmunition = 20;
    [SerializeField]
    private int maxExtraAmmunition = 20;

    [Header("Game details")]
    [SerializeField, SyncVar]
    private string player;

    [Header("References")]
    [SerializeField]
    private Animator weaponAnimator = null;
    [SerializeField]
    private Transform rayPosition = null;

    private void Awake()
    {
        // Setup controls
    }

    private void Update()
    {
        if (!hasAuthority) return;

        if (Input.GetMouseButtonDown(0))
        {
            Cmd_Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Cmd_Reload();
        }
    }

    [Command]
    private void Cmd_Shoot()
    {
        if (currentAmmunition == 0) return;

        Ray shootRay = new Ray(rayPosition.position, transform.forward);
        RaycastHit rayHit;
        if (Physics.Raycast(shootRay, out rayHit, range, ~ignoreLayer))
        {
            rayHit.collider.GetComponent<IDamagable>()?.Svr_Damage(damage);
        }
        currentAmmunition -= bulletsPerShot;
    }

    [Command]
    private void Cmd_Reload()
    {
        int currentAmmunition = this.currentAmmunition;
        int neededAmmunition = maxCurrentAmmunition % currentAmmunition;

        this.currentAmmunition += (extraAmmunition < maxCurrentAmmunition 
                                ? neededAmmunition
                                : maxCurrentAmmunition - currentAmmunition);

        extraAmmunition -= neededAmmunition;

        Debug.Log(neededAmmunition);
        Debug.Log(extraAmmunition);
    }
}
