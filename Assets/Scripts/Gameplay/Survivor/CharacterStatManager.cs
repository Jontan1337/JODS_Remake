using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Sirenix.OdinInspector;
using RootMotion.FinalIK;
using UnityEngine.Events;

public class CharacterStatManager : NetworkBehaviour, IDamagable, IInteractable
{
    private SurvivorController sController;
    [SerializeField] private Animator animatorController;


    [Title("Stats")]
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int armor = 0;
    [SerializeField] private float movementSpeed = 0;
    [SerializeField] private float reviveTime = 5;
    [SerializeField] private float downTime = 30;

    [Title("UI References")]
    [SerializeField] private Slider healthBar = null;
    [SerializeField] private Slider healthLossBar = null;
    [SerializeField] private Slider armorBar = null;
    [SerializeField] private Image lowHealthImage = null;
    [SerializeField] private Image reviveTimerImageUI = null;
    [SerializeField] private GameObject reviveTimerObjectUI = null;
    [SerializeField] private Image downImage = null;
    [SerializeField] private Image damagedImage = null;
    [SerializeField] private GameObject downCanvas = null;
    [SerializeField] private GameObject inGameCanvas = null;
    [Space]
    [SerializeField] private PlayerEquipment playerEquipment;
    [SerializeField] private FullBodyBipedIK fullBodyBipedIK;
    [SerializeField] private SurvivorSetup survivorSetup;


    private const string inGameUIPath = "UI/Canvas - In Game";

    private Transform cameraTransform;
    private Transform originalCameraTransformParent;

    [Header("Events")]
    public UnityEvent onDied = null;

    private float reviveTimeCount = 0;
    private float downImageOpacity = 0;
    private bool beingRevived = false;
    private NetworkConnection connectionToClientInteractor;

    [SerializeField] private Text pointsText = null;
    [SerializeField] private GameObject pointGainPrefab = null;
    public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
    [SerializeField, SyncVar] private bool isInteractable = false;

    [SyncVar(hook = nameof(pointsHook))] public int points;
    private void pointsHook(int oldVal, int newVal)
    {
        pointsText.text = "Points: " + newVal;
        StartCoroutine(PointsIE(newVal - oldVal));
    }

    public float MovementSpeed => movementSpeed;

    public override void OnStartAuthority()
    {
        sController = GetComponent<SurvivorController>();
    }

    public void SetStats(int maxHealth, int armor, float movementSpeed)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        this.armor = armor;
        this.movementSpeed = movementSpeed;
        GetComponent<ModifierManagerSurvivor>().MovementSpeed = movementSpeed;
        GetComponent<SurvivorAnimationIKManager>().anim.speed = movementSpeed;


        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        healthLossBar.maxValue = maxHealth;
        armorBar.value = armor;
    }


    private bool isDead;
    public bool IsDead
    {
        get { return isDead; }
        set
        {
            //Update scoreboard stat
            GamemodeBase.Instance.Svr_ModifyStat(GetComponent<NetworkIdentity>().netId, 0, PlayerDataStat.Alive);

            isDead = value;
            onDied?.Invoke();
            NetworkServer.Destroy(gameObject);
        }
    }

    public int GetHealth => currentHealth;

    public int Health
    {
        get => currentHealth;
        private set
        {
            int prevHealth = currentHealth;
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            healthBar.value = currentHealth;


            lowHealthImage.color = new Color(1, 1, 1, (maxHealth / 2 - (float)currentHealth) / 100 * 2);

            if (!healthLossBool)
            {
                StartCoroutine(HealthLossCo(prevHealth));
            }
            if (isServer)
            {
                if (currentHealth <= 0)
                {
                    IsDown = true;
                }
                Rpc_SyncStats(connectionToClient, currentHealth, armor);
            }
        }
    }

    public int Armor
    {
        get => armor;
        private set
        {
            armor = value;
            armorBar.value = armor;
            if (isServer)
            {
                Rpc_SyncStats(connectionToClient, currentHealth, armor);
            }
        }
    }

    public override void OnStartServer()
    {
        FindComponents();
    }

    private async void FindComponents()
    {
        await JODSTime.WaitTime(0.1f);
        playerEquipment = GetComponentInChildren<PlayerEquipment>();
        fullBodyBipedIK = GetComponent<FullBodyBipedIK>();
        survivorSetup = GetComponent<SurvivorSetup>();
        cameraTransform = transform.Find("Virtual Head(Clone)/PlayerCamera(Clone)");
        originalCameraTransformParent = transform.Find("Virtual Head(Clone)");
    }

    private async void FindCamera()
    {
        await JODSTime.WaitTime(0.2f);
        fullBodyBipedIK = GetComponent<FullBodyBipedIK>();
        cameraTransform = transform.Find("Virtual Head(Clone)/PlayerCamera(Clone)");
        originalCameraTransformParent = transform.Find("Virtual Head(Clone)");
        inGameCanvas = transform.Find($"{inGameUIPath}").gameObject;
    }

    [SyncVar] private bool isDown;
    public bool IsDown
    {
        get { return isDown; }
        set
        {
            isDown = value;
            if (isDown)
            {
                DownCo = Down();
                StartCoroutine(DownCo);
                var equip = playerEquipment.EquipmentItem;
                if (equip)
                {
                    equip.Svr_Unequip();
                }
                fullBodyBipedIK.enabled = false;
                //survivorSetup.Rpc_ToggleHead(connectionToClient);
                Rpc_SetCameraForDownedState(connectionToClient);
            }
            else
            {
                playerEquipment.EquipmentItem?.Svr_Equip();
                fullBodyBipedIK.enabled = true;
                //survivorSetup.Rpc_ToggleHead(connectionToClient);
                Rpc_SetCameraForRevivedState(connectionToClient);
            }
        }
    }

    private bool healthLossBool = false;

    private IEnumerator HealthLossCo(int prevHealth)
    {
        healthLossBool = true;
        healthLossBar.value = prevHealth;
        yield return new WaitForSeconds(1);
        while (healthLossBar.value > Health)
        {
            healthLossBar.value -= Time.deltaTime * 15;

            yield return null;
        }
        healthLossBool = false;
    }

    [TargetRpc]
    private void Rpc_DamageImage(NetworkConnection target, int damageTaken)
    {
        float dmgTaken = damageTaken;
        if ((dmgTaken / 100) > damagedImage.color.a)
        {
            if (damagedImageBool)
            {
                StopCoroutine(DamangedImageCo);
                damagedImageBool = false;
            }
            DamangedImageCo = DamagedImage(dmgTaken);
            StartCoroutine(DamangedImageCo);
        }
    }
    IEnumerator DamangedImageCo;
    private bool damagedImageBool = false;
    private IEnumerator DamagedImage(float damageTaken)
    {
        float time = 1;
        float roundedValue = Mathf.Ceil(damageTaken / 20) * 20;
        float baseValue = roundedValue / 100;
        damagedImageBool = true;
        damagedImage.color = new Color(1, 1, 1, baseValue);
        yield return new WaitForSeconds(1f);
        while (time > 0)
        {
            damagedImage.color = new Color(1, 1, 1, (time * baseValue));

            time -= Time.deltaTime;
            yield return null;
        }

        damagedImageBool = false;
    }


    public Teams Team => Teams.Player;



    [Server]
    public void Svr_Damage(int damage, Transform target = null)
    {
        Damage(damage, target);
    }

    [Command]
    public void Cmd_Damage(int damage)
    {
        Damage(damage);
    }

    void Damage(int damage, Transform source = null)
    {
        if (source)
        {
            if (source.GetComponent<IDamagable>().Team == Teams.Player)
            {
                float parsedDmg = damage;
                damage = Mathf.RoundToInt(parsedDmg /= 2);
            }
        }

        if (IsDown)
        {
            downTime -= 0.1f;
            downImageOpacity += (1f / 30f) * 0.1f;
        }
        else
        {
            if (armor > 0)
            {
                float armorPercent = (float)armor / 100;

                int armorLoss = Mathf.Clamp(Mathf.RoundToInt(damage * armorPercent), 0, 100);
                int healthLoss = damage - armorLoss;

                Health -= healthLoss;
                Armor = Mathf.Clamp(Armor -= armorLoss, 0, 100);
            }
            else
            {
                Health -= damage;
            }
            Rpc_DamageImage(connectionToClient, damage);
        }
    }

    // Why are we using an Rpc to update the player's armor instead of just making it a SyncVar?????
    [TargetRpc]
    public void Rpc_SyncStats(NetworkConnection target, int newHealth, int newArmor)
    {
        if (isServer) return;
        Health = newHealth;
        Armor = newArmor;
    }

    private IEnumerator PointsIE(int pointGain)
    {
        GameObject pText = Instantiate(pointGainPrefab, inGameCanvas.transform);
        Text text = pText.GetComponent<Text>();
        text.text = "+ " + pointGain;
        float time = 1;
        while (time > 0)
        {
            yield return null;
            time -= Time.deltaTime;
            text.color = new Color(1, 1, 1, time);
            text.transform.Translate(new Vector3(0, 0.5f, 0));
        }
        Destroy(pText);
    }

    #region Revive Stuff

    IEnumerator DownCo;
    private IEnumerator Down()
    {
        Rpc_Down(connectionToClient);
        animatorController.SetBool("IsDown", true);
        IsInteractable = true;
        downImageOpacity = 0;
        downImage.color = new Color(1f, 1f, 1f, 0f);

        while (downTime > 0)
        {
            if (!beingRevived)
            {
                Rpc_UpdateDownImage(connectionToClient, downImageOpacity);
                downImageOpacity += (1f / 30f);
                downTime -= 1;
            }
            yield return new WaitForSeconds(1f);
        }
        IsDead = true;
    }

    [TargetRpc]
    private void Rpc_Down(NetworkConnection target)
    {
        JODSInput.DisableCamera();
        JODSInput.DisableHotbarControl();
        animatorController.SetBool("IsDown", true);
        inGameCanvas.SetActive(false);
        downCanvas.SetActive(true);
        sController.enabled = false;
    }

    [TargetRpc]
    private void Rpc_UpdateDownImage(NetworkConnection target, float downImageOpacity)
    {
        downImage.color = new Color(1f, 1f, 1f, downImageOpacity);
    }

    IEnumerator BeingRevivedCo;
    public IEnumerator BeingRevived()
    {
        beingRevived = true;
        while (reviveTime > 0)
        {
            reviveTime -= 1;
            yield return new WaitForSeconds(1f);
        }
        Revived();
    }

    private void Revived()
    {
        Health = 50;
        IsDown = false;
        animatorController.SetBool("IsDown", false);
        downTime = 30;
        reviveTime = 5;
        StopCoroutine(DownCo);
        IsInteractable = false;
        Rpc_Revived(connectionToClient);
        beingRevived = false;
    }

    [TargetRpc]
    private void Rpc_Revived(NetworkConnection target)
    {
        inGameCanvas.SetActive(true);
        downCanvas.SetActive(false);
        sController.enabled = true;
        JODSInput.EnableCamera();
        JODSInput.EnableHotbarControl();
        animatorController.SetBool("IsDown", false);
    }


    IEnumerator ReviveTimerCo;
    private IEnumerator ReviveTimer()
    {
        reviveTimeCount = 0;
        reviveTimerObjectUI.SetActive(true);
        reviveTimerImageUI.fillAmount = 0;
        while (reviveTimeCount < 5)
        {
            reviveTimeCount += (Time.deltaTime);
            reviveTimerImageUI.fillAmount = reviveTimeCount / 5;
            yield return null;
        }
        reviveTimerObjectUI.SetActive(false);
    }

    [TargetRpc]
    private void Rpc_StartReviveTimer(NetworkConnection target)
    {
        ReviveTimerCo = ReviveTimer();
        StartCoroutine(ReviveTimerCo);
    }

    [TargetRpc]
    private void Rpc_ReviveTimerCancelled(NetworkConnection target)
    {
        StopCoroutine(ReviveTimerCo);
        reviveTimerObjectUI.SetActive(false);
    }

    private void ReviveCancelled()
    {
        StopCoroutine(BeingRevivedCo);

        beingRevived = false;
        reviveTime = 5;
    }

    [Server]
    public void Svr_PerformInteract(GameObject interacter)
    {
        if (!beingRevived)
        {
            connectionToClientInteractor = interacter.GetComponent<NetworkIdentity>().connectionToClient;
            Rpc_DisableMovement(connectionToClientInteractor);
            interacter.GetComponent<CharacterStatManager>().Rpc_StartReviveTimer(connectionToClientInteractor);
            BeingRevivedCo = BeingRevived();
            StartCoroutine(BeingRevivedCo);
        }
    }
    [Server]
    public void Svr_CancelInteract(GameObject interacter)
    {
        Rpc_EnableMovement(connectionToClientInteractor);
        interacter.GetComponent<CharacterStatManager>().Rpc_ReviveTimerCancelled(connectionToClientInteractor);
        ReviveCancelled();
    }
    [TargetRpc]
    private void Rpc_DisableMovement(NetworkConnection target)
    {
        JODSInput.DisableMovement();
    }

    [TargetRpc]
    private void Rpc_EnableMovement(NetworkConnection target)
    {
        JODSInput.EnableMovement();
    }
    #endregion

    #region ViewModel
    [TargetRpc]
    private void Rpc_SetCameraForDownedState(NetworkConnection target)
    {
        cameraTransform.SetParent(fullBodyBipedIK.references.head.GetChild(0));
    }
    [TargetRpc]
    private void Rpc_SetCameraForRevivedState(NetworkConnection target)
    {
        cameraTransform.SetParent(originalCameraTransformParent);
        cameraTransform.localPosition = new Vector3(0f, 0.1f, 0f);
        cameraTransform.rotation = originalCameraTransformParent.rotation;
    }

    #endregion
}
