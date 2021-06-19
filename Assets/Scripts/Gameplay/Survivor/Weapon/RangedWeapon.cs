using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Collections;
using System;


public class RangedWeapon : EquipmentItem
{
    [Header("Settings")]
    [SerializeField] private LayerMask ignoreLayer;

    [Header("Weapon stats")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float range = 1000f;
    [SerializeField] private FireModes fireMode = FireModes.Single;
    [SerializeField] private FireModes[] fireModes;
    [SerializeField] private int burstBulletAmount = 3;
    [SerializeField] private float fireRate = 600f;
    [SerializeField] private float fireInterval = 0f;
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField, SyncVar] private int currentAmmunition = 10;
    [SerializeField] private int maxCurrentAmmunition = 10;
    [SerializeField, SyncVar] private int extraAmmunition = 20;
    [SerializeField] private int maxExtraAmmunition = 20;

    [Header("Game details")]
    [SerializeField, SyncVar] private string player = "Player name";

    [Header("References")]
    [SerializeField] private Animator weaponAnimator = null;
    [SerializeField] private Transform bulletRayOrigin = null;
    [SerializeField] private GameObject muzzleFlash = null;
    [SerializeField] private SFXPlayer sfxPlayer = null;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip shootSound = null;
    [SerializeField] private AudioClip emptySound = null;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    private Coroutine COShootLoop;
    private Coroutine COStopShootLoop;

    private ParticleSystem muzzleParticle;

    private bool canShoot = true;

    private int fireModeIndex = 0;

    private void OnValidate()
    {
        equipmentType = EquipmentType.Weapon;
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

    public override void Bind()
    {
        base.Bind();
        JODSInput.Controls.Survivor.Reload.performed += OnReload;
        JODSInput.Controls.Survivor.Changefiremode.performed += OnChangeFireMode;
    }
    public override void Unbind()
    {
        if (hasAuthority)
        {
            OnLMBCanceled(default);
        }

        base.Unbind();
        JODSInput.Controls.Survivor.Reload.performed -= OnReload;
        JODSInput.Controls.Survivor.Changefiremode.performed -= OnChangeFireMode;
    }

    protected override void OnLMBPerformed(InputAction.CallbackContext obj)
    {
        JODSInput.Controls.Survivor.Drop.Disable();
        JODSInput.Controls.Survivor.Interact.Disable();

        if (hasAuthority)
        {
            Cmd_Shoot();
        }
    }

    protected override void OnLMBCanceled(InputAction.CallbackContext obj)
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
            case FireModes.Scatter:
                ScatterShot();
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

    private void ScatterShot()
    {
        if (!canShoot) return;

        int firedRounds = 0;
        while (currentAmmunition > 0 && firedRounds < bulletsPerShot)
        {
            Shoot();
            firedRounds++;
            if (firedRounds == bulletsPerShot)
            {
                StartCooldown();
                if (currentAmmunition == 0)
                {
                    Rpc_EmptySFX();
                    StartCooldown();
                }
                return;
            }
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
                StartCooldown();
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
                StartCooldown();
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
        StartCooldown();
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


    private void StartCooldown()
    {
        StartCoroutine(IEShootCooldown());
    }
    // A cooldown to simulate the time it takes
    // for a bullet to get chambered.
    private IEnumerator IEShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireInterval);
        canShoot = true;
    }

    private void ShotShell()
    {

    }

    // Main shoot method.
    private void Shoot()
    {

        Rpc_ShootSFX();
        Ray shootRay = new Ray(bulletRayOrigin.position, transform.forward);
        RaycastHit rayHit;
        if (Physics.Raycast(shootRay, out rayHit, range, ~ignoreLayer))
        {
            rayHit.collider.GetComponent<IDamagable>()?.Svr_Damage(damage);
        }

        currentAmmunition -= 1;
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
        if (fireModes.Length <= 1) return;

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
