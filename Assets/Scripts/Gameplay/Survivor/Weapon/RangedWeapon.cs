using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using DG.Tweening;

public abstract class RangedWeapon : EquipmentItem, IImpacter
{
    [Header("RANGED WEAPON")]
    [SerializeField, TextArea(2,2)] private string header2 = "";
    [Space]
    [Header("Weapon stats")]
    [SerializeField] protected int damage = 10;
    [SerializeField, Tooltip("The number of targets the weapon will penetrate.")] protected int penetrationAmount = 0;
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
    [SerializeField, SyncVar(hook = nameof(SetPlayerCamera))] protected Transform playerHead;
    [SerializeField] protected Camera playerCamera;
    [SerializeField] private GameObject muzzleFlash = null;
    [SerializeField] private ParticleSystem muzzleParticle;
    [SerializeField] private SFXPlayer sfxPlayer = null;

    [Header("Prefabs")]
    [SerializeField] private GameObject crosshairPrefab;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip shootSound = null;
    [SerializeField] private AudioClip emptySound = null;

    protected float damageFallOff = 0;

    private Transform crosshairUIParent;
    private Crosshair crosshairUI;
    private Transform currentAmmunitionUI;
    private TextMeshProUGUI currentAmmunitionUIText;
    private Transform extraAmmunitionUI;
    private TextMeshProUGUI extraAmmunitionUIText;
    private Transform fireModeUI;
    private TextMeshProUGUI fireModeUIText;

    private Coroutine COShootLoop;
    private Coroutine COStopShootLoop;
    private Coroutine COAccuracyStabilizer;

    private const string inGameUIPath = "UI/Canvas - In Game";

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
            FadeFireMode(1f, 0.2f).OnComplete(delegate () { FadeFireMode(0f, 0.5f); });
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
        currentCurveAccuracy = recoilCurve.Evaluate(0f);
        fireRate = Mathf.Clamp(fireRate, 0.01f, float.MaxValue);
        fireInterval = 60 / fireRate;
        fireModeIndex = 0;
        switch (ammunitionType)
        {
            case AmmunitionTypes.Small:
                damageFallOff = 0.5f;
                break;
            case AmmunitionTypes.Medium:
                damageFallOff = 0.3f;
                break;
            case AmmunitionTypes.Large:
                damageFallOff = 0.15f;
                break;
        }
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
        //Rpc_GetPlayerHead(connectionToClient, playerHead);
    }

    [TargetRpc]
    public override void Rpc_Interact(NetworkConnection target, GameObject interacter)
    {
        base.Rpc_Interact(target, interacter);
        playerHead = interacter.GetComponent<LookController>().RotateVertical;
        playerCamera = playerHead.GetChild(0).GetComponent<Camera>();
        GetUIElements(interacter.transform);
    }

    private void SetPlayerCamera(Transform oldValue, Transform newValue)
    {
        if (!newValue) return;

        playerCamera = newValue.GetChild(0).GetComponent<Camera>();
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
        FadeFireMode(1f, 0.2f).OnComplete(delegate () { FadeFireMode(0f, 0.5f); });
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
        FadeFireMode(0f, 0.5f);
    }

    private void GetUIElements(Transform root)
    {
        crosshairUIParent = root.Find($"{inGameUIPath}/Crosshair");
        currentAmmunitionUI = root.Find($"{inGameUIPath}/Weapon ammunition");
        extraAmmunitionUI = root.Find($"{inGameUIPath}/Weapon extra ammunition");
        fireModeUI = root.Find($"{inGameUIPath}/Weapon fire mode");
        currentAmmunitionUIText = currentAmmunitionUI.GetComponent<TextMeshProUGUI>();
        extraAmmunitionUIText = extraAmmunitionUI.GetComponent<TextMeshProUGUI>();
        fireModeUIText = fireModeUI.GetComponent<TextMeshProUGUI>();
    }
    private Tweener FadeFireMode(float value, float duration)
    {
        return fireModeUIText.DOFade(value, duration);
    }

    private void CreateCrosshair()
    {
        if (crosshairUI != null) return;
        if (crosshairUIParent != null)
        {
            crosshairUI = Instantiate(crosshairPrefab, crosshairUIParent).GetComponent<Crosshair>();
        }
        // Set the crosshair min and max size to the weapon recoil min and max values.
        crosshairUI.Init();
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
        JODSInput.Controls.Survivor.Hotbarselecting.Disable();

        if (hasAuthority)
        {
            Cmd_Shoot();
        }
    }

    protected override void OnLMBCanceled(InputAction.CallbackContext obj)
    {
        JODSInput.Controls.Survivor.Drop.Enable();
        JODSInput.Controls.Survivor.Interact.Enable();
        JODSInput.Controls.Survivor.Hotbarselecting.Enable();

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
        Svr_PreShoot();
        Vector2 aimPoint = UnityEngine.Random.insideUnitCircle * currentAccuracy * 10;
        while (Magazine > 0 && firedRounds < bulletsPerShot)
        {
            firedRounds++;
            Svr_Shoot(aimPoint);
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
        if (!isLocalPlayer)
            Rpc_PostShoot(connectionToClient);
        Svr_PostShoot();
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
                Svr_PreShoot();
                Svr_Shoot(Vector2.zero);
                Svr_StartCooldown();
                if (!hasAuthority)
                    Rpc_StartCooldown(connectionToClient);
                firedRounds++;
            }
            yield return null;
        }
        if (!isLocalPlayer)
            Rpc_PostShoot(connectionToClient);
        Svr_PostShoot();
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
                Svr_PreShoot();
                Svr_Shoot(Vector2.zero);
                if (!isLocalPlayer)
                    Rpc_PostShoot(connectionToClient);
                Svr_StartCooldown();
                if (!hasAuthority)
                    Rpc_StartCooldown(connectionToClient);
                Svr_PostShoot();
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
        Svr_PreShoot();
        Svr_Shoot(Vector2.zero);
        if (!isLocalPlayer)
            Rpc_PostShoot(connectionToClient);
        Svr_StartCooldown();
        if (!isLocalPlayer)
            Rpc_StartCooldown(connectionToClient);
        Svr_PostShoot();
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

    //[Server]
    private void StartAccuracyStabilizer()
    {
        if (COAccuracyStabilizer != null) return;

        COAccuracyStabilizer = StartCoroutine(IEAccuracyStabilizer());
    }

    [Server]
    protected virtual void Svr_Shoot(Vector2 aimPoint)
    {
        Debug.LogError("You broke this. Rocket launcher didn't work because it never lost ammunition. I temporarily fixed by removing an If statement in rocket launcher script. Fix.");
    }
    [Server]
    protected virtual void Svr_PreShoot()
    {
        Magazine -= 1;
    }
    [Server]
    protected virtual void Svr_PostShoot()
    {
        PostShoot();
    }

    private void PostShoot()
    {
        CurrentAccuracy += recoil;
        StartAccuracyStabilizer();
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
        Rpc_ChangeFireMode();
    }

    #endregion

    #region Clients

    [TargetRpc]
    private void Rpc_PostShoot(NetworkConnection target)
    {
        PostShoot();
    }

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
    protected void Rpc_ChangeFireMode()
    {
        sfxPlayer.PlaySFX(emptySound);
        transform.DOComplete();
        transform.DOPunchRotation(new Vector3(0f, 2f, -5f), 0.2f, 0, 0.5f);
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
