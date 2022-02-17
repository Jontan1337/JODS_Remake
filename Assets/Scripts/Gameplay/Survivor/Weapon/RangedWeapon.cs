using UnityEngine;
using TMPro;
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
    [SerializeField, SyncVar(hook = nameof(UpdateFireModeText))] private FireModes fireMode = FireModes.Single;
    [SerializeField] private FireModes[] fireModes = null;
    [SerializeField] private int burstBulletAmount = 3;
    [SerializeField] private float fireRate = 600f;
    [SerializeField] private float fireInterval = 0f;
    [SerializeField] private int bulletsPerShot = 1;
    [Space]
    [SerializeField, SyncVar(hook = nameof(UpdateMagazineText))] protected int magazine = 10;
    [SerializeField] protected int magazineSize = 10;
    [SerializeField, SyncVar(hook = nameof(UpdateExtraAmmunitionText))] protected int extraAmmunition = 20;
    [Space]
    [SerializeField] protected bool highPower = false;
    //[SerializeField] protected int maxExtraAmmunition = 20;

    [Header("Game details")]
    //[SerializeField, SyncVar] private string player = "Player name";
    [SerializeField, Range(0f, 1f), Tooltip("Affects weapon accuracy")] protected float recoil = 0.1f;
    [SerializeField, Tooltip("Affects weapon and camera shake")] protected float visualPunchback = 0.2f;
    [SerializeField] protected float stabilization = 1f;
    [SerializeField] protected float currentAccuracy = 0f;
    [SerializeField] protected float currentCurveAccuracy = 0f;
    [SerializeField] protected AnimationCurve recoilCurve;

    [Header("References")]
    [SerializeField] protected Transform shootOrigin = null;
    [SerializeField, SyncVar] protected Transform playerHead;
    [SerializeField] protected Camera playerCamera;
    [SerializeField] private GameObject muzzleFlash = null;
    [SerializeField] private ParticleSystem muzzleParticle;
    [SerializeField] private SFXPlayer sfxPlayer = null;

    [Header("UI References")]
    [SerializeField] private Transform crosshairUIParent;
    [SerializeField] private Crosshair crosshairUI;
    [SerializeField] private Transform currentAmmunitionUI;
    [SerializeField] private TextMeshProUGUI currentAmmunitionUIText;
    [SerializeField] private Transform extraAmmunitionUI;
    [SerializeField] private TextMeshProUGUI extraAmmunitionUIText;
    [SerializeField] private Transform fireModeUI;
    [SerializeField] private TextMeshProUGUI fireModeUIText;

    [Header("Prefabs")]
    [SerializeField] private GameObject crosshairPrefab;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip shootSound = null;
    [SerializeField] private AudioClip emptySound = null;

    private Coroutine COShootLoop;
    private Coroutine COStopShootLoop;
    private Coroutine COAccuracyStabilizer;

    private const string playerUIPath = "UI/Canvas - In Game/";

    private bool canShoot = true;

    private int fireModeIndex = 0;

    public Action<float> OnImpact { get; set; }

    public int Magazine { 
        get => magazine;
        private set
        {
            magazine = value;
        }
    }
    public int MagazineSize { 
        get => magazineSize;
        private set
        {
            magazineSize = value;
        }
    }
    public int ExtraAmmunition { 
        get => extraAmmunition;
        private set
        {
            extraAmmunition = value;
        }
    }
    public int Damage { get => damage; }
    public AmmunitionTypes AmmunitionType { get => ammunitionType; set => ammunitionType = value; }
    public FireModes FireMode {
        get => fireMode;
        private set
        {
            fireMode = value;
            TeaseFireMode();
        }
    }
    public FireModes[] AllFireModes { get => fireModes; }
    protected float CurrentAccuracy
    {
        get => currentAccuracy;
        set
        {
            currentAccuracy = Mathf.Clamp01(value);
        }
    }

    #region Update UI

    private void UpdateMagazineText(int oldValue, int newValue)
    {
        if (!hasAuthority) return;

        currentAmmunitionUIText.text = newValue.ToString();
    }
    private void UpdateExtraAmmunitionText(int oldValue, int newValue)
    {
        if (!hasAuthority) return;

        extraAmmunitionUIText.text = newValue.ToString();
    }
    private void UpdateFireModeText(FireModes oldValue, FireModes newValue)
    {
        if (!hasAuthority) return;

        fireModeUIText.text = newValue.ToString();
    }

    #endregion

    private void OnValidate()
    {
        equipmentType = EquipmentType.Weapon;
        currentCurveAccuracy = recoilCurve.Evaluate(0f);
        fireRate = Mathf.Clamp(fireRate, 0.01f, float.MaxValue);
        fireInterval = 60 / fireRate;
        fireModeIndex = 0;
        foreach (int modeIndex in fireModes)
        {
            if ((int)FireMode == modeIndex) break;
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

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
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
        //magazine = Magazine;
        //extraAmmunition = ExtraAmmunition;
        //fireMode = FireMode;
        Rpc_GetPlayerHead(connectionToClient, playerHead);
    }

    [TargetRpc]
    public override void Rpc_Interact(NetworkConnection target, GameObject interacter)
    {
        base.Rpc_Interact(target, interacter);
        GetUIElements(interacter.transform);
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
        CreateCrosshair();
        currentAmmunitionUIText.enabled = true;
        extraAmmunitionUIText.enabled = true;
        fireModeUIText.enabled = true;
        // Update UI.
        UpdateMagazineText(0, magazine);
        UpdateExtraAmmunitionText(0, extraAmmunition);
        UpdateFireModeText(0, fireMode);
        TeaseFireMode();
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
        RemoveCrosshair();
        currentAmmunitionUIText.enabled = false;
        extraAmmunitionUIText.enabled = false;
        fireModeUIText.enabled = false;
        FadeFireMode(0f);
    }

    private void GetUIElements(Transform root)
    {
        crosshairUIParent = root.Find($"{playerUIPath}Crosshair");
        currentAmmunitionUI = root.Find($"{playerUIPath}Weapon ammunition");
        extraAmmunitionUI = root.Find($"{playerUIPath}Weapon extra ammunition");
        fireModeUI = root.Find($"{playerUIPath}Weapon fire mode");
        currentAmmunitionUIText = currentAmmunitionUI.GetComponent<TextMeshProUGUI>();
        extraAmmunitionUIText = extraAmmunitionUI.GetComponent<TextMeshProUGUI>();
        fireModeUIText = fireModeUI.GetComponent<TextMeshProUGUI>();
    }

    private void TeaseFireMode()
    {
        fireModeUIText.DOFade(1f, 0.5f).OnComplete(() => { fireModeUIText.DOFade(0f, 0.5f); });
    }
    private void FadeFireMode(float value)
    {
        fireModeUIText.DOFade(value, 0.5f);
    }

    private void CreateCrosshair()
    {
        if (crosshairUI != null) return;
        if (crosshairUIParent != null)
        {
            crosshairUI = Instantiate(crosshairPrefab, crosshairUIParent).GetComponent<Crosshair>();
        }
        // Set the crosshair min and max size to the weapon recoil min and max values.
        crosshairUI.minSize = recoilCurve.keys[0].value;
        crosshairUI.maxSize = recoilCurve.keys[1].value;
        crosshairUI.SetSize(currentCurveAccuracy);
    }
    private void RemoveCrosshair()
    {
        if (crosshairUI != null)
        {
            Destroy(crosshairUI.gameObject);
            crosshairUI = null;
        }
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
        Svr_StartAccuracyStabilizer();
        if (!hasAuthority)
            Rpc_StartAccuracyStabilizer(connectionToClient);

        switch (FireMode)
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
        PreShoot();
        Vector2 aimPoint = UnityEngine.Random.insideUnitCircle * currentAccuracy * 10;
        while (Magazine > 0 && firedRounds < bulletsPerShot)
        {
            firedRounds++;
            Shoot(aimPoint);
            if (firedRounds == bulletsPerShot)
            {
                Svr_StartCooldown();
                if (!hasAuthority)
                    Rpc_StartCooldown(connectionToClient);
                if (Magazine == 0)
                {
                    Rpc_EmptySFX();
                }
                break;
            }
        }
        PostShoot();
    }

    private IEnumerator IEBurstShootLoop()
    {
        int firedRounds = 0;
        // The loop will keep going, as long as there
        // are bullets in the magazine and burst hasn't finished.
        // NOTE: Consider making the bullet shot interval in a burst very fast, since that's how they actually work.
        while (Magazine > 0 && firedRounds < burstBulletAmount)
        {
            if (canShoot)
            {
                PreShoot();
                Shoot(Vector2.zero);
                Svr_StartCooldown();
                if (!hasAuthority)
                    Rpc_StartCooldown(connectionToClient);
                firedRounds++;
            }
            yield return null;
        }
        PostShoot();
        if (Magazine == 0)
        {
            Rpc_EmptySFX();
        }
        COShootLoop = null;
    }
    private IEnumerator IEFullAutoShootLoop()
    {
        // The loop will keep going, as long as there
        // are bullets in the magazine.
        while (Magazine > 0)
        {
            if (canShoot)
            {
                PreShoot();
                Shoot(Vector2.zero);
                PostShoot();
                Svr_StartCooldown();
                if (!hasAuthority)
                    Rpc_StartCooldown(connectionToClient);
            }
            yield return null;
        }
        // If the magazine runs out of bullets, this will be called.
        Rpc_EmptySFX();
    }

    private void ShootSingle()
    {
        if (Magazine == 0)
        {
            Rpc_EmptySFX();
            return;
        }
        PreShoot();
        Shoot(Vector2.zero);
        PostShoot();
        Svr_StartCooldown();
        if (!hasAuthority)
            Rpc_StartCooldown(connectionToClient);
    }

    [Command]
    // Stop any shoot coroutine.
    private void Cmd_StopShoot()
    {
        // Stop shoot loops from firing except burst mode because
        // it needs to shoot all burst rounds before stopping.
        if (COShootLoop != null && FireMode != FireModes.Burst)
        {
            StopCoroutine(COShootLoop);
            COShootLoop = null;
        }
    }

    [Server]
    private void Svr_StartCooldown()
    {
        StartCoroutine(IEShootCooldown());
    }

    [Server]
    private void Svr_StartAccuracyStabilizer()
    {
        if (COAccuracyStabilizer != null) return;

        COAccuracyStabilizer = StartCoroutine(IEAccuracyStabilizer());
    }

    protected virtual void Shoot(Vector2 aimPoint)
    {
        Debug.LogError("You broke this. Rocket launcher didn't work because it never lost ammunition. I temporarily fixed by removing an If statement in rocket launcher script. Fix.");
    }

    private void PreShoot()
    {
        Magazine -= 1;
    }
    private void PostShoot()
    {
        CurrentAccuracy += recoil;
        Svr_StartAccuracyStabilizer();
    }

    // Later this should be called by a reload animation event.
    [Command]
    private void Cmd_Reload()
    {
        if (Magazine == MagazineSize) return;

        Rpc_Reload();
        if (ExtraAmmunition > (MagazineSize - Magazine))
        {
            ExtraAmmunition = ExtraAmmunition - (MagazineSize - Magazine);
            Magazine = MagazineSize;
        }
        else
        {
            Magazine += ExtraAmmunition;
            ExtraAmmunition = 0;
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
            FireMode = fireModes[fireModeIndex];
        }
        else
        {
            FireMode = fireModes[++fireModeIndex];
        }
        Rpc_ChangeFireModeSFX();
        transform.DOComplete();
        transform.DOPunchRotation(new Vector3(0f, 2f, -5f), 0.2f, 0, 0.5f);
    }

    #endregion

    #region Clients

    [TargetRpc]
    private void Rpc_StartCooldown(NetworkConnection target)
    {
        StartCoroutine(IEShootCooldown());
    }

    [TargetRpc]
    private void Rpc_StartAccuracyStabilizer(NetworkConnection target)
    {
        if (COAccuracyStabilizer != null) return;

        COAccuracyStabilizer = StartCoroutine(IEAccuracyStabilizer());
    }

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
            CurrentAccuracy -= stabilization * Time.deltaTime;
            currentCurveAccuracy = recoilCurve.Evaluate(CurrentAccuracy);
            if (crosshairUI)
            {
                crosshairUI.SetSize(currentCurveAccuracy);
            }
            yield return null;
        }
        COAccuracyStabilizer = null;
    }

    [ClientRpc]
    protected virtual void Rpc_Shoot(Vector2 recoil)
    {
        ShootFX();
    }

    protected void ShootFX()
    {
        sfxPlayer.PlaySFX(shootSound);
        muzzleParticle.Emit(10);
        OnImpact?.Invoke(visualPunchback);
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
        transform.DOPunchRotation(new Vector3(20f, 5f, -10f), 1f, 2, 1f);
        transform.DOPunchPosition(new Vector3(0.05f, -0.04f, 0f), 1f, 2, 0.1f);
    }

    #endregion
}
