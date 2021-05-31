using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Collections;
using System;

[RequireComponent(typeof(PhysicsToggler), typeof(Rigidbody), typeof(BoxCollider)),
 RequireComponent(typeof(AuthorityController))]
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
    private int damage = 10;
    [SerializeField]
    private float range = 1000f;
    [SerializeField]
    private FireModes fireMode = FireModes.Single;
    [SerializeField]
    private FireModes[] fireModes;
    [SerializeField]
    private int burstBulletAmount = 3;
    [SerializeField]
    private float fireRate = 600f;
    [SerializeField]
    private float fireInterval = 0f;
    [SerializeField]
    private float fireCooldownInterval = 0f;
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
    [SerializeField]
    private GameObject muzzleFlash = null;
    [SerializeField]
    private SFXPlayer sfxPlayer = null;
    [SerializeField]
    private AuthorityController authController = null;

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

    private Coroutine COShootLoop;
    private Coroutine COStopShootLoop;

    private ParticleSystem muzzleParticle;

    private bool canShoot = true;

    private int fireModeIndex = 0;

    public bool IsInteractable
    {
        get => isInteractable;
        set => isInteractable = value;
    }

    public string ObjectName => gameObject.name;
    public string Name => weaponName;
    public GameObject Item => gameObject;
    public EquipmentType EquipmentType => equipmentType;

    private void OnValidate()
    {
        fireRate = Mathf.Clamp(fireRate, 0.01f, float.MaxValue);
        fireInterval = 60 / fireRate;
        fireModeIndex = 0;
        foreach (int modeIndex in fireModes)
        {
            if ((int)fireMode == modeIndex) break;
            fireModeIndex++;
        }
        if (muzzleFlash != null)
        {
            if (muzzleFlash.TryGetComponent(out ParticleSystem particle))
            {
                muzzleParticle = particle;
            }
        }
    }

    public void Bind()
    {
        JODSInput.Controls.Survivor.LMB.performed += OnShoot;
        JODSInput.Controls.Survivor.LMB.canceled += OnStopShoot;
        JODSInput.Controls.Survivor.Reload.performed += OnReload;
        JODSInput.Controls.Survivor.Changefiremode.performed += OnChangeFireMode;
    }
    public void UnBind()
    {
        if (hasAuthority)
        {
            OnStopShoot(default);
        }

        JODSInput.Controls.Survivor.LMB.performed -= OnShoot;
        JODSInput.Controls.Survivor.LMB.canceled -= OnStopShoot;
        JODSInput.Controls.Survivor.Reload.performed -= OnReload;
        JODSInput.Controls.Survivor.Changefiremode.performed -= OnChangeFireMode;
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        JODSInput.Controls.Survivor.Drop.Disable();
        JODSInput.Controls.Survivor.Interact.Disable();

        Cmd_Shoot();
    }
    private void OnStopShoot(InputAction.CallbackContext context)
    {
        JODSInput.Controls.Survivor.Drop.Enable();
        JODSInput.Controls.Survivor.Interact.Enable();

        if (hasAuthority)
        {
            Cmd_StopShoot();
        }
    }
    private void OnReload(InputAction.CallbackContext context) => Cmd_Reload();
    private void OnChangeFireMode(InputAction.CallbackContext context) => Cmd_ChangeFireMode();


    #region Server

    [Command]
    private void Cmd_Shoot()
    {
        if (!canShoot) return;

        switch (fireMode)
        {
            case FireModes.Single:
            case FireModes.SemiAuto:
                ShootSingle();
                break;
            case FireModes.Burst:
                if (COShootLoop == null)
                {
                    COShootLoop = StartCoroutine(IEBurstShootLoop());
                }
                break;
            case FireModes.FullAuto:
                if (COShootLoop == null)
                {
                    COShootLoop = StartCoroutine(IEFullAutoShootLoop());
                }
                break;
        }
    }

    private IEnumerator IEBurstShootLoop()
    {
        int firedRounds = 0;
        // The loop will keep going, as long as there
        // are bullets in the magazine and burst hasn't finished.
        while (currentAmmunition > 0 && firedRounds < burstBulletAmount)
        {
            if (canShoot)
            {
                Shoot();
                firedRounds++;
            }
            yield return null;
        }
        if (currentAmmunition == 0)
        {
            Rpc_EmptySFX();
        }
        COShootLoop = null;
    }
    private IEnumerator IEFullAutoShootLoop()
    {
        // The loop will keep going, as long as there
        // are bullets in the magazine.
        while (currentAmmunition > 0)
        {
            if (canShoot)
            {
                Shoot();
            }
            yield return null;
        }
        // If the magazine runs out of bullets, this will be called.
        Rpc_EmptySFX();
    }

    private void ShootSingle()
    {
        if (currentAmmunition == 0)
        {
            Rpc_EmptySFX();
            return;
        }
        Shoot();
    }

    [Command]
    // Stop any shoot coroutine.
    private void Cmd_StopShoot()
    {
        // Stop shoot loops from firing except burst mode because
        // it needs to shoot all burst rounds before stopping.
        if (COShootLoop != null && fireMode != FireModes.Burst)
        {
            StopCoroutine(COShootLoop);
            COShootLoop = null;
        }
    }

    // A cooldown to simulate the time it takes
    // for a bullet to get chambered.
    private IEnumerator IEShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireInterval);
        canShoot = true;
    }


    // Main shoot method.
    private void Shoot()
    {
        StartCoroutine(IEShootCooldown());
        Rpc_ShootSFX();
        Ray shootRay = new Ray(bulletRayOrigin.position, transform.forward);
        RaycastHit rayHit;
        if (Physics.Raycast(shootRay, out rayHit, range, ~ignoreLayer))
        {
            rayHit.collider.GetComponent<IDamagable>()?.Svr_Damage(damage);
        }


        currentAmmunition -= bulletsPerShot;
    }


    // Later this should be called by a reload animation event.
    [Command]
    private void Cmd_Reload()
    {
        Debug.Log("Reload!", this);
        if (extraAmmunition > (maxCurrentAmmunition - currentAmmunition))
        {
            extraAmmunition = extraAmmunition - (maxCurrentAmmunition - currentAmmunition);
            currentAmmunition = maxCurrentAmmunition;
        }
        else
        {
            currentAmmunition = currentAmmunition + extraAmmunition;
            extraAmmunition = 0;
        }
    }

    [Command]
    private void Cmd_ChangeFireMode()
    {
        Cmd_StopShoot();
        if (fireModeIndex == fireModes.Length - 1)
        {
            fireModeIndex = 0;
            fireMode = fireModes[fireModeIndex];
        }
        else
        {
            fireMode = fireModes[++fireModeIndex];
        }
        Rpc_ChangeFireModeSFX();
    }

    [Server]
    public void Svr_Interact(GameObject interacter)
    {
        if (!IsInteractable) return;

        // Equipment should be on a child object of the player.
        Equipment equipment = interacter.GetComponentInChildren<Equipment>();

        if (equipment != null)
        {
            authController.Svr_GiveAuthority(interacter.GetComponent<NetworkIdentity>().connectionToClient);
            equipment?.Svr_Equip(gameObject, equipmentType);
            IsInteractable = false;
        }
        else
        {
            // This should not be possible, but just to be absolutely sure.
            Debug.LogWarning($"{interacter} does not have an Equipment component", this);
        }
    }

    #endregion

    #region Clients

    [ClientRpc]
    private void Rpc_ShootSFX()
    {
        sfxPlayer.PlaySFX(shootSound);
        muzzleParticle.Emit(10);
        // Implement partyicle efect.
    }
    [ClientRpc]
    private void Rpc_EmptySFX()
    {
        sfxPlayer.PlaySFX(emptySound);
        // Implement partyicle efect.
    }
    [ClientRpc]
    private void Rpc_ChangeFireModeSFX()
    {
        sfxPlayer.PlaySFX(emptySound);
        // Implement partyicle efect.
    }

    #endregion
}
