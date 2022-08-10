using Mirror;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActiveSClass : NetworkBehaviour, IDamagable, IInteractable
{
    //what is going on in this script??? - dave

    [SyncVar(hook = nameof(SetSurvivorClassSettings))] public SurvivorClass sClass;
    private SurvivorController sController;

    [SerializeField] private SurvivorSO survivorSO;
    [SerializeField] private PlayerEquipment playerEquipment;
    [SerializeField] private FullBodyBipedIK fullBodyBipedIK;
    [SerializeField] private SurvivorSetup survivorSetup;
    [SerializeField] private Animator animatorController;
    [SerializeField] private SkinnedMeshRenderer bodyRenderer = null;
    [SerializeField] private SkinnedMeshRenderer headRenderer = null;
    [Space]
    [Header("Stats")]
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int armor = 0;
    [SerializeField] private float abilityCooldown = 0;
    //[SerializeField] private float abilityCooldownCount = 0;
    [SerializeField] private float movementSpeed = 0;
    //[SerializeField] private float cooldownReducion = 0;
    public float abilityCooldownCount = 0;

    [Space]
    [Header("Weapon Stats")]
    [SerializeField] private float reloadSpeed = 0;
    [SerializeField] private float accuracy = 0;
    [SerializeField] private float ammoCapacity = 0;
    [SerializeField] private GameObject starterWeapon;

    [Header("UI References")]
    [SerializeField] private Slider healthBar = null;
    [SerializeField] private Slider healthLossBar = null;
    [SerializeField] private Slider armorBar = null;
    [SerializeField] private Image abilityCooldownUI = null;
    [SerializeField] private Image reviveTimerImageUI = null;
    [SerializeField] private GameObject reviveTimerObjectUI = null;
    [SerializeField] private Image downImage = null;
    [SerializeField] private GameObject downCanvas = null;

    [Header("Events")]
    public UnityEvent<float> onChangedHealth = null;
    public UnityEvent onDied = null;

    private bool abilityIsReady = true;
    //private SurvivorAnimationIKManager 

    public bool AbilityIsReady
    {
        get { return abilityIsReady; }
        private set { abilityIsReady = value; }
    }

    private bool isDown;
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
                survivorSetup.Rpc_ToggleHead(connectionToClient);
            }
            else
            {
                playerEquipment.EquipmentItem?.Svr_Equip();
                fullBodyBipedIK.enabled = true;
                survivorSetup.Rpc_ToggleHead(connectionToClient);
            }
        }
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
    public float MovementSpeed => movementSpeed;
    public int GetHealth => currentHealth;
    public int Health
    {
        get => currentHealth;
        private set
        {
            int prevHealth = currentHealth;
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            healthBar.value = currentHealth;
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

    #region Serialization
    //public override bool OnSerialize(NetworkWriter writer, bool initialState)
    //{
    //	if (!initialState)
    //	{
    //		writer.WriteSurvivorClass(sClass);
    //	}
    //	else
    //	{
    //		writer.WriteSurvivorClass(sClass);
    //	}
    //	return true;
    //}
    //public override void OnDeserialize(NetworkReader reader, bool initialState)
    //{
    //	if (!initialState)
    //	{
    //		sClass = reader.ReadSurvivorClass();
    //	}
    //	else
    //	{
    //		sClass = reader.ReadSurvivorClass();
    //	}
    //}
    #endregion

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
    }

    public override void OnStartAuthority()
    {
        if (test) SetSurvivorClass(survivorSO);
        JODSInput.Controls.Survivor.ActiveAbility.performed += ctx => Ability();

    }

    #region ViewModel
    [TargetRpc]
    private void Rpc_InvokeOnChangedHealth(NetworkConnection target, float healthDifference)
    {
        onChangedHealth?.Invoke(healthDifference);
    }

    #endregion

    #region Ability Stuff

    private void Ability()
    {
        if (AbilityIsReady)
        {
            sClass.ActiveAbility();
        }
        else
        {
            sClass.ActiveAbilitySecondary();
        }
    }

    public IEnumerator AbilityCooldown()
    {
        AbilityIsReady = false;
        abilityCooldownUI.fillAmount = 0;
        while (abilityCooldownCount < abilityCooldown)
        {

            abilityCooldownCount += (Time.deltaTime * GetComponent<ModifierManager>().Cooldown);
            abilityCooldownUI.fillAmount = abilityCooldownCount / abilityCooldown;
            yield return null;
        }
        abilityCooldownCount = 0;
        AbilityIsReady = true;
    }

    public void StartAbilityCooldownCo()
    {
        StartCoroutine(AbilityCooldown());
    }

    [TargetRpc]
    public void Rpc_StartAbilityCooldown(NetworkConnection conn, Transform owner)
    {
        owner.GetComponentInParent<ActiveSClass>()?.StartAbilityCooldownCo();
    }

    [Command]
    public void Cmd_StartAbilityCooldown(Transform owner)
    {
        Rpc_StartAbilityCooldown(GetComponent<NetworkIdentity>().connectionToClient, owner);
    }

    #endregion

    #region Class Stuff
    [ClientRpc]
    public void Rpc_SetSurvivorClass(string _class)
    {
        List<SurvivorSO> survivorSOList = PlayableCharactersManager.Instance.GetAllSurvivors();
        foreach (SurvivorSO survivor in survivorSOList)
        {
            if (survivor.name == _class)
            {
                SetSurvivorClass(survivor);
                break;
            }
        }
    }

    public void SetSurvivorClass(SurvivorSO survivorSO)
    {
        this.survivorSO = survivorSO;
        if (hasAuthority)
        {
            Cmd_SpawnClass();
        }
    }

    private void SetSurvivorClassSettings(SurvivorClass oldValue, SurvivorClass newValue)
    {
        if (survivorSO.abilityObject && newValue)
        {
            newValue.abilityObject = survivorSO.abilityObject;
        }

        maxHealth = survivorSO.maxHealth;
        currentHealth = maxHealth;
        armor = survivorSO.startingArmor;

        accuracy = survivorSO.accuracy;
        reloadSpeed = survivorSO.reloadSpeed;   
        ammoCapacity = survivorSO.ammoCapacity;

        sController = GetComponent<SurvivorController>();
        movementSpeed = survivorSO.movementSpeed;
        GetComponent<ModifierManager>().MovementSpeed = movementSpeed;
        GetComponent<SurvivorAnimationIKManager>().anim.speed = movementSpeed;

        abilityCooldown = survivorSO.abilityCooldown;

        bodyRenderer.sharedMesh = survivorSO.bodyMesh;
        headRenderer.sharedMesh = survivorSO.headMesh;

        bodyRenderer.material = survivorSO.characterMaterial;
        headRenderer.material = survivorSO.characterMaterial;

        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        healthLossBar.maxValue = maxHealth;
        armorBar.value = armor;
    }

    [Command]
    private void Cmd_SpawnClass()
    {
        StartCoroutine(SpawnClassObjects());
    }

    IEnumerator SpawnClassObjects()
    {
        yield return new WaitForSeconds(0.2f);

        GameObject selectedClass = Instantiate(survivorSO.classScript);
        NetworkServer.Spawn(selectedClass, gameObject);
        selectedClass.transform.SetParent(gameObject.transform);

        sClass = selectedClass.GetComponent<SurvivorClass>();

        if (survivorSO.starterWeapon)
        {
            starterWeapon = Instantiate(survivorSO.starterWeapon, transform.position, transform.rotation);
            NetworkServer.Spawn(starterWeapon);
            yield return new WaitForSeconds(0.35f);
            starterWeapon.GetComponent<EquipmentItem>().Svr_PerformInteract(gameObject);
        }
    }
    #endregion

    #region Health Stuff

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

    IEnumerator DownCo;
    [SerializeField] private float downTime = 30;
    private bool beingRevived = false;
    private float downImageOpacity = 0;
    public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
    [SerializeField, SyncVar] private bool isInteractable = false;
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
        downCanvas.SetActive(true);
        sController.enabled = false;
    }

    [TargetRpc]
    private void Rpc_UpdateDownImage(NetworkConnection target, float downImageOpacity)
    {
        downImage.color = new Color(1f, 1f, 1f, downImageOpacity);
    }



    [SerializeField] private float reviveTime = 5;
    IEnumerator BeingRevivedCo;
    private IEnumerator BeingRevived()
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
        downCanvas.SetActive(false);
        sController.enabled = true;
    }



    IEnumerator ReviveTimerCo;
    private float reviveTimeCount = 0;
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

    #endregion


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GetComponentInChildren<IHitter>()?.OnFlyingKickHit(hit);
    }

    [Header("Debug")]
    public bool test;

    private void OnGUI()
    {
        if (test)
        {
            GUI.TextField(new Rect(20, 20, 150, 20), "Active S Class Test ON");
        }
    }
    [Server]
    public void Svr_PerformInteract(GameObject interacter)
    {
        if (!beingRevived)
        {
            interacter.GetComponent<ActiveSClass>().sController.enabled = false;
            interacter.GetComponent<ActiveSClass>().Rpc_StartReviveTimer(interacter.GetComponent<NetworkIdentity>().connectionToClient);
            BeingRevivedCo = BeingRevived();
            StartCoroutine(BeingRevivedCo);
        }
    }




    [Server]
    public void Svr_CancelInteract(GameObject interacter)
    {
        interacter.GetComponent<ActiveSClass>().sController.enabled = true;
        interacter.GetComponent<ActiveSClass>().Rpc_ReviveTimerCancelled(interacter.GetComponent<NetworkIdentity>().connectionToClient);
        ReviveCancelled();
    }
}
