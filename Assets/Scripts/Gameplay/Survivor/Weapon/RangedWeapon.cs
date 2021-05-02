﻿using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(PhysicsToggler), typeof(Rigidbody), typeof(BoxCollider))]
public class RangedWeapon : NetworkBehaviour, IInteractable, IEquippable, IBindable
{
    [Header("Settings")]
    [SerializeField]
    private string weaponName = "Weapon name";
    [SerializeField]
    private EquipmentType equipmentType = EquipmentType.Weapon;
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
    private string player = "Player name";

    [Header("References")]
    [SerializeField]
    private Animator weaponAnimator = null;
    [SerializeField]
    private AudioSource audioSource = null;
    [SerializeField]
    private Transform bulletRayOrigin = null;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioClip shootSound = null;
    [SerializeField]
    private AudioClip emptySound = null;
    [SerializeField, Range(0f, 1f)]
    private float volume = 1f;

    [Space]
    [SerializeField, SyncVar]
    private bool isInteractable = true;

    private Coroutine COShootingLoop;

    public bool IsInteractable {
        get => isInteractable;
        set => isInteractable = value;
    }

    public string ObjectName => gameObject.name;

    public string Name => weaponName;
    public GameObject Item => gameObject;
    public EquipmentType EquipmentType => equipmentType;

    private void OnValidate()
    {
        fireRate = Mathf.Clamp(fireRate, 0.1f, float.MaxValue);
    }

    public void Bind()
    {
        JODSInput.Controls.Survivor.LMB.performed += OnShoot;
        JODSInput.Controls.Survivor.LMB.canceled += OnStopShoot;
        JODSInput.Controls.Survivor.Reload.performed += OnReload;
    }
    public void UnBind()
    {
        JODSInput.Controls.Survivor.LMB.performed -= OnShoot;
        JODSInput.Controls.Survivor.LMB.canceled -= OnStopShoot;
        JODSInput.Controls.Survivor.Reload.performed -= OnReload;
    }

    private void OnShoot(InputAction.CallbackContext context) => Cmd_Shoot();
    private void OnStopShoot(InputAction.CallbackContext context) => StopShootingLoop();
    private void OnReload(InputAction.CallbackContext context) => Cmd_Reload();

    [Command]
    private void Cmd_Shoot()
    {
        if (currentAmmunition == 0)
        {
            return;
        }
        audioSource.PlayOneShot(shootSound, volume);
        Ray shootRay = new Ray(bulletRayOrigin.position, transform.forward);
        RaycastHit rayHit;
        if (Physics.Raycast(shootRay, out rayHit, range, ~ignoreLayer))
        {
            rayHit.collider.GetComponent<IDamagable>()?.Svr_Damage(damage);
        }
        currentAmmunition -= bulletsPerShot;
        //COShootingLoop = StartCoroutine(ShootingLoop());
    }

    private IEnumerator ShootingLoop()
    {
        float fireInterval = Mathf.Clamp(fireRate / 1000, 0.1f, float.MaxValue);
        while (true)
        {
            if (currentAmmunition == 0)
            {
                //audioSource.PlayOneShot(emptySound, volume);

                yield return new WaitForSeconds(fireInterval);
            }
            else
            {
                audioSource.PlayOneShot(shootSound, volume);
                Ray shootRay = new Ray(bulletRayOrigin.position, transform.forward);
                RaycastHit rayHit;
                if (Physics.Raycast(shootRay, out rayHit, range, ~ignoreLayer))
                {
                    rayHit.collider.GetComponent<IDamagable>()?.Svr_Damage(damage);
                }
                currentAmmunition -= bulletsPerShot;

                yield return new WaitForSeconds(fireInterval);
            }
        }
    }

    private void StopShootingLoop()
    {
        Debug.Log("Stopped shooting", this);
        if (COShootingLoop != null)
        {
            StopCoroutine(COShootingLoop);
        }
    }

    // Later this should be called by a reload animation event.
    [Command]
    private void Cmd_Reload()
    {
        Debug.Log("Reload!", this);
        int neededAmmunition = 0;

        neededAmmunition = extraAmmunition < maxCurrentAmmunition
                                ? extraAmmunition
                                : maxCurrentAmmunition - this.currentAmmunition;

        this.currentAmmunition =+ neededAmmunition;

        //this.currentAmmunition += extraAmmunition < maxCurrentAmmunition 
        //                        ? neededAmmunition
        //                        : maxCurrentAmmunition - currentAmmunition;

        extraAmmunition -= neededAmmunition;

        Debug.Log(neededAmmunition);
        Debug.Log(extraAmmunition);
    }

    [Server]
    public void Svr_Interact(GameObject interacter)
    {
        if (!IsInteractable) return;

        // Equipment should be on a child object of the player.
        Equipment equipment = interacter.GetComponentInChildren<Equipment>();

        if (equipment != null)
        {
            Svr_GiveAuthority(interacter.GetComponent<NetworkIdentity>().connectionToClient);
            equipment?.Svr_Equip(gameObject, equipmentType);
            IsInteractable = false;
        }
        else
        {
            // This should not be possible, but just to be absolutely sure.
            Debug.LogWarning($"{interacter} does not have an Equipment component", this);
        }
    }

    [Server]
    public void Svr_GiveAuthority(NetworkConnection conn)
    {
        netIdentity.AssignClientAuthority(conn);
    }
    [Server]
    public void Svr_RemoveAuthority()
    {
        netIdentity.RemoveClientAuthority();
    }
}
