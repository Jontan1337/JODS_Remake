using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public abstract class RangedWeapon : EquipmentItem, IImpacter
{
    [Title("RANGED WEAPON", "", TitleAlignments.Centered), Space(30f)]
    [Header("Weapon stats")]
    [SerializeField] protected int damage = 10;
    [SerializeField, Tooltip("The number of targets the weapon will penetrate.")] protected int penetrationAmount = 0;
    [SerializeField] private AmmunitionTypes ammunitionType = AmmunitionTypes.Small;
    [SerializeField, SyncVar(hook = nameof(UpdateFireModeText))] private FireModes fireMode = FireModes.Single;
    [SerializeField] private FireModes[] fireModes = null;
    [SerializeField, ShowIf("fireMode", FireModes.Scatter)] private int burstBulletAmount = 3;
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
    [SerializeField, Range(0f, 1f), Tooltip("Affects weapon accuracy")] protected float recoil = 0.1f;
    [SerializeField, Range(0f, 1f), Tooltip("Affects weapon accuracy")] protected float aimingRecoil = 0.05f;
    [SerializeField, Tooltip("Affects weapon and camera shake")] protected float visualPunchback = 0.2f;
    [SerializeField, Tooltip("Affects weapon and camera shake")] protected float aimingVisualPunchback = 0.1f;
    [SerializeField, Tooltip("Affects rigidbodies hit")] protected float rigidbodyPunchback = 0.2f;
    [SerializeField] private float aimingSpeed = 0.1f;
    [SerializeField] private float reloadSpeed = 1f;
    [SerializeField, Tooltip("If stabilization is the same as the recoil, recoil won't decrease")] protected float stabilization = 1f;
    [SerializeField] protected float currentAccuracy = 0f;
    [SerializeField] protected float currentCurveAccuracy = 0f;
    [SerializeField] protected AnimationCurve recoilCurve;
    [SerializeField, Range(0f, 10f)] private float aimBaseAccuracy = 1;
    [SerializeField] private int muzzleParticleEmitAmount = 10;
    [Title("FOV Settings")]
    [SerializeField] protected float hipFOV = 80f;
    [SerializeField] protected float ADSFOV = 50f;

    [Header("References")]
    [SerializeField, Required] protected Transform shootOrigin = null;
    [SerializeField, SyncVar(hook = nameof(SetPlayerHeadAndCamera))] protected Transform playerHead;
    [SerializeField] protected Camera playerCamera = null;
    [SerializeField] private GameObject muzzleFlash = null;
    [SerializeField] protected Transform aimSight = null;
    [SerializeField] private ParticleSystem muzzleParticle;
    [SerializeField] private SFXPlayer sfxPlayer = null;

    [Header("Prefabs")]
    [SerializeField] private GameObject crosshairPrefab = null;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip shootSound = null;
    [SerializeField] private AudioClip emptySound = null;

    //[SerializeField] private bool debugWeapon = false;

    protected Vector3 hipAimPosition;
    protected float damageFallOff = 0;

    private Transform emptyHandsCosshair;
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
    [SyncVar] private bool isAiming = false;
    private bool canShoot = true;
    private float currentRecoil = 0f;
    private int fireModeIndex = 0;
    private CameraSettings cameraSettings = null;

    protected ImpactData impactData;
    protected ImpactData aimingImpactData;

    public Action<ImpactData> OnImpact { get; set; }

    public int Magazine { 
        get
        {
            if (magazine <= 0)
            {
                Rpc_EmptySFX();
            }
            return magazine;
        }
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
        set
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
    public bool IsAiming
    {
        get => isAiming;
        private set => isAiming = value;
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

    // This is not called when object is instantiated. (This is only for editor purposes)
    private void OnValidate()
    {
        InitVariables();
    }

    private void InitVariables()
    {
        // Initialize variables.
        currentRecoil = recoil;
        currentCurveAccuracy = recoilCurve.Evaluate(0f);
        foreach (int modeIndex in fireModes)
        {
            if ((int)FireMode == modeIndex) break;
            fireModeIndex++;
        }
        // Calculate weapon stats.
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
        InitVariables();
        base.OnStartClient();
        OnImpact += ImpactShake;
        impactData = new ImpactData(visualPunchback, ImpactSourceType.Ranged);
        aimingImpactData = new ImpactData(aimingVisualPunchback, ImpactSourceType.Ranged);
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
    public override void Svr_PerformInteract(GameObject interacter)
    {
        base.Svr_PerformInteract(interacter);
        playerHead = interacter.GetComponent<LookController>().RotateVertical;
        playerCamera = playerHead.Find("PlayerCamera(Clone)").GetComponent<Camera>();
        cameraSettings = playerHead.Find("PlayerCamera(Clone)").GetComponent<CameraSettings>();
    }

    [TargetRpc]
    public override void Rpc_PerformInteract(NetworkConnection target, GameObject interacter)
    {
        base.Rpc_PerformInteract(target, interacter);
        playerHead = interacter.GetComponent<LookController>().RotateVertical;
        playerCamera = playerHead.Find("PlayerCamera(Clone)").GetComponent<Camera>();
        cameraSettings = playerHead.Find("PlayerCamera(Clone)").GetComponent<CameraSettings>();
        hipFOV = playerCamera.fieldOfView;
        ADSFOV = hipFOV - 10f;
        GetUIElements(interacter.transform);
        transform.DOComplete();
    }

    [Server]
    public override void Svr_ParentChanged()
    {
        hipAimPosition = transform.parent.localPosition;
        //hipAimPosition = Vector3.zero;
    }
    [Client]
    public override void ParentChanged()
    {
        hipAimPosition = transform.parent.localPosition;
        //hipAimPosition = Vector3.zero;
    }

    private void SetPlayerHeadAndCamera(Transform oldValue, Transform newValue)
    {
        if (!newValue) return;

        playerHead = newValue;
        playerCamera = newValue.Find("PlayerCamera(Clone)").GetComponent<Camera>();
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
        fireModeUIText.DOComplete();
        FadeFireMode(1f, 0.2f).OnComplete(delegate () { FadeFireMode(0f, 0.5f); });
    }
    public override void Unbind()
    {
        //if (hasAuthority)
        //{
        //    OnLMBCanceled(default);
        //}
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
        emptyHandsCosshair = root.Find($"{inGameUIPath}/Crosshair/Empty hands crosshair");
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
    private Tweener ScaleCrosshair(float value, float duration)
    {
        return crosshairUIParent.DOScale(value, duration);
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
        emptyHandsCosshair.gameObject.SetActive(false);
    }
    private void RemoveCrosshair()
    {
        if (crosshairUI != null)
        {
            Destroy(crosshairUI.gameObject);
            crosshairUI = null;
            emptyHandsCosshair.gameObject.SetActive(true);
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

    protected override void OnRMBPerformed(InputAction.CallbackContext obj)
    {
        Aim(true);
    }
    protected override void OnRMBCanceled(InputAction.CallbackContext obj)
    {
        Aim(false);
    }

    private void Aim(bool aim)
    {
        IsAiming = aim;
        Cmd_Aim(aim);
        ScaleCrosshair(IsAiming ? 0 : 1, 0.1f);
        cameraSettings.SetFOV(IsAiming ? ADSFOV : hipFOV, 0.1f);
        Vector3 targetAimPosition = new Vector3(-0.14f, 0.1f - aimSight.localPosition.y + 0.14f, -aimSight.localPosition.z - 0.4f);

        transform.parent.DOComplete();
        transform.parent.DOLocalJump(IsAiming ? targetAimPosition : hipAimPosition, -0.05f, 1, aimingSpeed);
    }

    private void OnReload(InputAction.CallbackContext context) => Cmd_Reload();
    private void OnChangeFireMode(InputAction.CallbackContext context) => Cmd_ChangeFireMode();

    #region Server

    [Command]
    private void Cmd_Aim(bool aim)
    {
        IsAiming = aim;
        currentRecoil = IsAiming ? aimingRecoil : recoil;
        EvaluateAccuracy();
    }

    [Command]
    private void Cmd_Shoot()
    {
        if (!canShoot) return;

        switch (FireMode)
        {
            case FireModes.Single:
                Svr_ShootSingle();
                break;
            case FireModes.SemiAuto:
                Svr_ShootSemiAuto();
                break;
            case FireModes.Scatter:
                Svr_ScatterShot();
                break;
            case FireModes.Burst:
                Svr_BurstShootLoop();
                break;
            case FireModes.FullAuto:
                Svr_FullAutoShootLoop();
                break;
        }
    }

    [Server]
    private void Svr_ScatterShot()
    {
        if (!canShoot) return;
        if (Magazine <= 0) return;
        int firedRounds = 0;
        // Scatter shot ammo is a single shell with multiple bullets/pellets.
        Svr_PreShoot();
        Vector2 aimPoint = UnityEngine.Random.insideUnitCircle * currentAccuracy * 10;
        while (firedRounds < bulletsPerShot)
        {
            firedRounds++;
            Svr_Shoot(aimPoint);
            if (firedRounds == bulletsPerShot)
            {
                Svr_StartCooldown();
                if (!hasAuthority)
                    Rpc_StartCooldown(connectionToClient);
                break;
            }
        }
        if (!isLocalPlayer)
            Rpc_PostShoot(connectionToClient);
        Svr_PostShoot();
    }

    [Server]
    private void Svr_BurstShootLoop()
    {
        if (COShootLoop == null)
        {
            COShootLoop = StartCoroutine(IEBurstShootLoop());
        }
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
        COShootLoop = null;
    }

    [Server]
    private void Svr_FullAutoShootLoop()
    {
        if (COShootLoop == null)
        {
            COShootLoop = StartCoroutine(IEFullAutoShootLoop());
        }
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
    }

    [Server]
    private void Svr_ShootSemiAuto()
    {
        if (Magazine == 0) return;
        Svr_PreShoot();
        Svr_Shoot(Vector2.zero);
        if (!isLocalPlayer)
            Rpc_PostShoot(connectionToClient);
        Svr_StartCooldown();
        if (!isLocalPlayer)
            Rpc_StartCooldown(connectionToClient);
        Svr_PostShoot();
    }

    [Server]
    private void Svr_ShootSingle()
    {
        if (Magazine == 0) return;
        Svr_PreShoot();
        Svr_Shoot(Vector2.zero);
        if (!isLocalPlayer)
            Rpc_PostShoot(connectionToClient);
        Svr_StartCooldown();
        Rpc_ReloadAnimation(reloadSpeed);
        if (!isLocalPlayer)
            Rpc_StartCooldown(connectionToClient);
        Rpc_Reload(connectionToClient);
        Svr_PostShoot();
    }

    // Stop any shoot coroutine.
    [Command]
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

    private void StartAccuracyStabilizer()
    {
        if (COAccuracyStabilizer != null) return;

        COAccuracyStabilizer = StartCoroutine(IEAccuracyStabilizer());
    }

    [Server]
    protected virtual void Svr_Shoot(Vector2 aimPoint)
    {
    }
    [Server]
    protected virtual void Svr_PreShoot()
    {
        Magazine -= 1;
        Rpc_ShootFX();
    }
    [Server]
    protected virtual void Svr_PostShoot()
    {
        PostShoot();
    }

    private void PostShoot()
    {
        CurrentAccuracy += currentRecoil;
        StartAccuracyStabilizer();
    }

    // Later this should be called by a reload animation event.
    [Command]
    private async void Cmd_Reload()
    {
        if (Magazine == MagazineSize) return;

        Rpc_Reload(connectionToClient);
        Rpc_ReloadAnimation(reloadSpeed);
        await JODSTime.WaitTime(reloadSpeed);
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

    [TargetRpc]
    private async void Rpc_Reload(NetworkConnection target)
    {
        JODSInput.DisableLMB();
        JODSInput.DisableRMB();
        JODSInput.DisableReload();
        JODSInput.DisableHotbarControl();
        JODSInput.DisableInteract();
        JODSInput.DisableDrop();
        await JODSTime.WaitTime(reloadSpeed);
        JODSInput.EnableLMB();
        JODSInput.EnableRMB();
        JODSInput.EnableReload();
        JODSInput.EnableHotbarControl();
        JODSInput.EnableInteract();
        JODSInput.EnableDrop();
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
            EvaluateAccuracy();
            if (crosshairUI)
            {
                crosshairUI.SetSize(currentCurveAccuracy);
            }
            yield return null;
        }
        COAccuracyStabilizer = null;
    }

    private void EvaluateAccuracy()
    {
        float tempCA = recoilCurve.Evaluate(CurrentAccuracy);
        if (IsAiming)
        {
            tempCA -= recoilCurve[0].value - aimBaseAccuracy;
        }
        currentCurveAccuracy = tempCA;
    }

    [ClientRpc]
    protected virtual void Rpc_Shoot(Vector2 recoil)
    {
    }

    [ClientRpc]
    protected void Rpc_ShootFX()
    {
        sfxPlayer.PlaySFX(shootSound);
        muzzleParticle.Emit(muzzleParticleEmitAmount);
        OnImpact?.Invoke(IsAiming ? aimingImpactData : impactData);
    }
    protected void ImpactShake(ImpactData impactData)
    {
        transform.DOComplete();
        transform.DOPunchPosition(new Vector3(0f, 0f, -0.1f) * impactData.Amount, 0.15f, 12, 1f);
        transform.DOPunchRotation(new Vector3(-2f, 0f, UnityEngine.Random.Range(-5f, 5f)) * impactData.Amount, 0.28f, 12, 1f);
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
    protected void Rpc_ReloadAnimation(float reloadSpeed)
    {
        transform.DOComplete();
        //Vector3 initialRotation = transform.rotation.eulerAngles;
        //Vector3 initialPosition = transform.rotation.eulerAngles;
        transform.DOLocalRotate(new Vector3(-80f, 5f, 0f), 0.2f, RotateMode.Fast).SetEase(Ease.Flash);
        transform.DOLocalMove(new Vector3(0.2f, 0.1f, -0.05f), 0.2f).SetEase(Ease.Flash).OnComplete(Do);
        //transform.DOPunchRotation(new Vector3(-80f, 5f, 0f), 1f, 0, 0f).SetEase(Ease.InOutQuint);
        //transform.DOPunchPosition(new Vector3(0.1f, 0.1f, -0.05f), 1f, 0, 0f).SetEase(Ease.InOutQuint);
        async void Do()
        {
            //transform.DOShakePosition(0.1f, 0.02f, 50, 90f).SetEase(Ease.Flash);
            transform.DOShakeRotation(0.1f, 5f, 15, 90f).SetEase(Ease.InOutBounce);
            await JODSTime.WaitTime(reloadSpeed - 0.4f);
            transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.2f, RotateMode.Fast).SetEase(Ease.OutBack);
            transform.DOLocalMove(new Vector3(0f, 0f, 0f), 0.2f).SetEase(Ease.OutBack);
        }
    }

    #endregion
}
