﻿using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using DG.Tweening;

public abstract class RangedWeapon : EquipmentItem, IImpacter
{
    [Header("Weapon stats")]
    [SerializeField] protected int damage = 10;
    [SerializeField] private AmmunitionTypes ammunitionType = AmmunitionTypes.Small;
    [SerializeField] private FireModes fireMode = FireModes.Single;
    [SerializeField] private FireModes[] fireModes = null;
    [SerializeField] private int burstBulletAmount = 3;
    [SerializeField] private float fireRate = 600f;
    [SerializeField] private float fireInterval = 0f;
    [SerializeField] private int bulletsPerShot = 1;
    [SerializeField, SyncVar] protected int currentAmmunition = 10;
    [SerializeField] protected int maxCurrentAmmunition = 10;
    [SerializeField, SyncVar] protected int extraAmmunition = 20;
    [Space]
    [SerializeField] protected bool highPower = false;
    //[SerializeField] protected int maxExtraAmmunition = 20;

    [Header("Game details")]
    //[SerializeField, SyncVar] private string player = "Player name";
    [SerializeField] protected float recoil = 0.1f;
    [SerializeField] protected float currentAccuracy = 0f;
    [SerializeField] protected float currentCurveAccuracy = 0f;
    [SerializeField] protected AnimationCurve recoilCurve;

    [Header("References")]
    [SerializeField] protected Transform shootOrigin = null;
    [SerializeField, SyncVar] protected Transform playerHead;
    [SerializeField] protected Camera playerCamera;
    [SerializeField] private GameObject muzzleFlash = null;
    [SerializeField] private SFXPlayer sfxPlayer = null;


    [Header("Audio Settings")]
    [SerializeField] private AudioClip shootSound = null;
    [SerializeField] private AudioClip emptySound = null;

    private Coroutine COShootLoop;
    private Coroutine COStopShootLoop;
    private Coroutine COAccuracyStabilizer;

    [SerializeField] private ParticleSystem muzzleParticle;

    private bool canShoot = true;

    private int fireModeIndex = 0;

    public Action<float> OnImpact { get; set; }

    public int Damage { get => damage; }
    public AmmunitionTypes AmmunitionType { get => ammunitionType; set => ammunitionType = value; }
    public FireModes FireMode { get => fireMode; }
    public FireModes[] AllFireModes { get => fireModes; }
    protected float CurrentAccuracy
    {
        get => currentAccuracy;
        set
        {
            currentAccuracy = Mathf.Clamp01(value);
        }
    }

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

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnImpact += ImpactShake;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        OnImpact -= ImpactShake;
    }

    [Server]
    public override void Svr_Interact(GameObject interacter)
    {
        base.Svr_Interact(interacter);
        playerHead = interacter.GetComponent<LookController>().RotateVertical;
        playerCamera = playerHead.GetChild(0).GetComponent<Camera>();
        Rpc_GetPlayerHead(connectionToClient, playerHead);
    }

    [TargetRpc]
    private void Rpc_GetPlayerHead(NetworkConnection target, Transform head)
    {
        playerHead = head;
        playerCamera = playerHead.GetChild(0).GetComponent<Camera>();
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

    // Why is Y axis (Sin) dumb pls help.
    //protected Vector2 GetRandomPointInCircle(float radius)
    //{
    //    float r = 2 * Mathf.PI * radius;
    //    float u = UnityEngine.Random.Range(-1f, 1f) + UnityEngine.Random.Range(-1f, 1f);
    //    float f = u > 1 ? u - 2 : u;
    //    print(Mathf.Cos(r));
    //    print(Mathf.Sin(r));
    //    return new Vector2(f * Mathf.Cos(r), f * Mathf.Sin(r));
    //}

    #region Server

    [Command]
    private void Cmd_Shoot()
    {
        if (!canShoot) return;
        StartAccuracyStabilizer();

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
        // Scatter shot ammo is a single shell with multiple bullets/pellets.
        What();
        Vector2 aimPoint = UnityEngine.Random.insideUnitCircle * currentAccuracy * 10;
        while (currentAmmunition > 0 && firedRounds < bulletsPerShot)
        {
            firedRounds++;
            Shoot(aimPoint);
            if (firedRounds == bulletsPerShot)
            {
                StartCooldown();
                if (currentAmmunition == 0)
                {
                    Rpc_EmptySFX();
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
        // NOTE: Consider making the bullet shot interval in a burst very fast, since that's how they actually work.
        while (currentAmmunition > 0 && firedRounds < burstBulletAmount)
        {
            if (canShoot)
            {
                Shoot(Vector2.zero);
                What();
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
                Shoot(Vector2.zero);
                What();
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
        Shoot(Vector2.zero);
        What();
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
    private void StartAccuracyStabilizer()
    {
        if (COAccuracyStabilizer != null) return;

        COAccuracyStabilizer = StartCoroutine(IEAccuracyStabilizer());
    }
    // A cooldown to simulate the time it takes
    // for a bullet to get chambered.
    private IEnumerator IEShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireInterval);
        canShoot = true;
    }
    private IEnumerator IEAccuracyStabilizer()
    {
        while (CurrentAccuracy > 0f)
        {
            CurrentAccuracy -= 0.4f * Time.deltaTime;
            currentCurveAccuracy = recoilCurve.Evaluate(CurrentAccuracy);
            yield return null;
        }

        COAccuracyStabilizer = null;
    }

    protected virtual void Shoot(Vector2 aimPoint)
    {
    }

    private void What()
    {
        CurrentAccuracy += recoil;
        StartAccuracyStabilizer();
        currentAmmunition -= 1;
    }

    // Later this should be called by a reload animation event.
    [Command]
    private void Cmd_Reload()
    {
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
        transform.DOComplete();
        transform.DOPunchRotation(new Vector3(0f, 2f, -5f), 0.2f, 0, 0.5f);
    }

    #endregion

    #region Clients

    [ClientRpc]
    protected virtual void Rpc_Shoot(Vector2 recoil)
    {
        ShootFX();
    }

    protected void ShootFX()
    {
        sfxPlayer.PlaySFX(shootSound);
        muzzleParticle.Emit(10);
        if (hasAuthority)
        {
        }
        OnImpact?.Invoke(recoil);
    }
    protected void ImpactShake(float amount)
    {
        transform.DOComplete();
        transform.DOPunchPosition(new Vector3(0f, 0f, -0.1f), 0.15f, 12, 1f);
        transform.DOPunchRotation(new Vector3(-2f, 0f, UnityEngine.Random.Range(-5f, 5f)), 0.28f, 12, 1f);
    }

    [ClientRpc]
    protected void Rpc_EmptySFX()
    {
        sfxPlayer.PlaySFX(emptySound);
    }
    [ClientRpc]
    protected void Rpc_ChangeFireModeSFX()
    {
        sfxPlayer.PlaySFX(emptySound);
    }
    [ClientRpc]
    protected void Rpc_Reload()
    {
        transform.DOComplete();
        transform.DOPunchRotation(new Vector3(0f, 2f, -5f), 0.2f, 0, 0.5f);
    }

    #endregion
}
