using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pathfinding;
using Sirenix.OdinInspector;

[System.Serializable]
public class UnitList
{  
    public string name;
    public UnitSO unit;
    [Space]
    [ShowIf("masterReference", null)] public UnitMaster masterReference;

    [Header("Upgrades")]
    [SyncVar] public int level = 0;
    public int upgradeMilestone = 50;
    public AnimationCurve upgradeCurve;
    public int totalUpgrades = 0;
    public int upgradesAvailable = 0;
    [Space]
    [Space]
    public ModifierManagerUnitData modifiers;    
    [Space]
    [Space]
    public int upgradesTillHealthTrait = 5;
    public bool hasHealthTrait = false;
    public int GetHealthStat() { return Mathf.RoundToInt((float)unit.health * modifiers.Health); }
    [Space]
    public int upgradesTillDamageTrait = 5;
    public bool hasDamageTrait = false;
    public int GetDamageStat()
    {
        int damageStat = 0;
        switch (unit.unitDamageType)
        {
            case UnitDamageType.Melee:
                damageStat = Mathf.RoundToInt(((unit.melee.meleeDamageMin + unit.melee.meleeDamageMax) / 2) * modifiers.MeleeDamage);
                break;
            case UnitDamageType.Ranged:
                damageStat = Mathf.RoundToInt(unit.ranged.rangedDamage * modifiers.RangedDamage);
                break;
            case UnitDamageType.Special:
                damageStat = Mathf.RoundToInt(unit.special.specialDamage * modifiers.SpecialDamage);
                break;
        }
        return damageStat;
    }
    [Space]
    public int upgradesTillSpeedTrait = 5;
    public bool hasSpeedTrait = false;
    public float GetSpeedStat() { return unit.movementSpeed * modifiers.MovementSpeed; }

    [Space]
    [Space]
    [ShowIf("upgradePanel", null)] public UnitUpgradePanel upgradePanel;
    [ShowIf("unitButton", null)] public MasterUIGameplayButton unitButton;
    [Header("Other")]
    [Space]
    public int maxAmount;
    [SerializeField] private int currentAmount;
    public int CurrentAmount
    {
        get { return currentAmount; }
        set 
        { 
            currentAmount = value;
            if (unitButton) masterReference.Svr_SetUnitAmountText(value, maxAmount, unitIndex);
        }
    }

    [Space]
    public bool unlocked;
    public bool chosen;
    public void Unlock(bool unlock)
    {
        unitButton.Unlock(unlock);
        unitButton.UpdateUnitAmount(currentAmount, maxAmount);
    }
    [Space]
    public int unitIndex;
    public int buttonIndex;
}

[System.Serializable]
public class DeployableList
{
    public string name;
    public DeployableSO deployable;
    public bool unlocked;
    public bool chosen;
    [Space]
    public bool onCooldown = false;
    public int deployableCooldown = 0;
    [Space]
    public DeployableUpgradePanel upgradePanel;
    public MasterUIGameplayButton deployableButton;
    public void Unlock(bool unlock)
    {
        deployableButton.Unlock(unlock);
    }
    [Space]
    public int buttonIndex;
}

[RequireComponent(typeof(AudioSource))]
public class UnitMaster : NetworkBehaviour
{
    #region Fields

    [BoxGroup("Debug")]
    [SerializeField] private bool test = false;

    [Header("Master Class")]
    [SerializeField] private UnitMasterSO masterSO = null;
    UnitMasterClass mClass = null;
    private MasterPlayerData playerData = null;

    [SerializeField, BoxGroup("Energy"),
        SyncVar(hook = nameof(CurrentEnergy_Hook))] private int currentEnergy = 50; //Current amount of master energy
    private void CurrentEnergy_Hook(int oldVal, int newVal)
    {
        UpdateEnergyUI();
    }
    [SerializeField, BoxGroup("Energy"),
        SyncVar(hook = nameof(MaxEnergy_Hook))] private int maxEnergy = 100; //Maximum amount of energy that can be stored

    private void MaxEnergy_Hook(int oldVal, int newVal)
    {
        //Set the UI slider's max value
        UI.energySlider.maxValue = maxEnergy;

        UI.energyUseSlider.maxValue = maxEnergy;

        UpdateEnergyUI();
    }

    [SerializeField, BoxGroup("Energy"),
        SyncVar] private int energyRechargeIncrement = 1; //How much energy recharges per second

    [SerializeField, BoxGroup("Energy")] private int maxEnergyIncrement = 20; //Amount to increase max energy

    [TargetRpc]
    private void Rpc_UpdateXPProgressBar(NetworkConnection target, int newVal, int maxVal)
    {
        if (UI.masterLevelImage == null) return;
        UI.masterLevelImage.fillAmount = (float)newVal / (float)maxVal;
    }
    [TargetRpc]
    private void Rpc_UpdateMasterLevelUI(NetworkConnection target, int newVal)
    {
        if (UI.masterLevelText == null) return;
        UI.masterLevelText.text = newVal.ToString();
    }




    [Space]

    [SyncVar(hook = nameof(MasterUpgradesHook))]private int masterUpgrades = 0;

    public int MasterUpgrades
    {
        get { return masterUpgrades; }
        set 
        { 
            masterUpgrades = value; 
            Rpc_UnlockMasterUpgradeButtons(connectionToClient, playerData.Level);
        }
    }

    private void MasterUpgradesHook(int oldVal, int newVal)
    {
        UI.masterUpgradePointsText.text = "Upgrade Points: " + newVal;
    }
    [TargetRpc]
    private void Rpc_UnlockMasterUpgradeButtons(NetworkConnection target, int masterLevel)
    {
        foreach (Button b in UI.masterUpgradeButtons)
        {
            b.GetComponent<MasterUpgradeUIButton>().UnlockButton(masterLevel, masterUpgrades > 0);
        }
    }

    private int Energy
    {
        get { return currentEnergy; }
        set
        {
            if (!isServer) return;

            currentEnergy = Mathf.Clamp(value, 0, maxEnergy);
        }
    }


    [TargetRpc]
    private void Rpc_UpdateXpUI(NetworkConnection target, int value)
    {
        //XP UI
        UI.xpText.text = value.ToString() + "XP";
        UI.upgradeMenuXpText.text = "Current XP: " + value.ToString();

        foreach (UnitList unitListItem in unitList)
        {
            unitListItem.upgradePanel.UnlockCheck(value);
        }

        foreach (DeployableList deployableListItem in deployableList)
        {
            deployableListItem.upgradePanel.UnlockCheck(value);
        }
    }


    [Title("Units", titleAlignment: TitleAlignments.Centered)]
    [SerializeField] private List<UnitList> unitList = new List<UnitList>();
    public UnitList GetUnitList(int index) => unitList[index];
    public UnitList GetUnitList(UnitSO so)
    {
        foreach (UnitList unitListItem in unitList)
        {
            if (unitListItem.unit == so)
            {
                return unitListItem;
            }
        }
        return null;
    } 
    [Space]
    [SerializeField, SyncVar] private int chosenSpawnableIndex = 0;
    [SerializeField] private bool hasChosenASpawnable = false;
    [SerializeField] private UnitBase selectedUnit = null;

    [Title("Deployables", titleAlignment: TitleAlignments.Centered)]
    [SerializeField] private List<DeployableList> deployableList = new List<DeployableList>();
    private Dictionary<string, GameObject> deployableDict = new Dictionary<string, GameObject>();
    public DeployableList GetDeployableList(int index) => deployableList[index];

    [Title("Spawning", titleAlignment: TitleAlignments.Centered)]
    [SerializeField] private LayerMask playerLayer = 1 << 13;
    [SerializeField] private int spawnCheckRadius = 200;
    [SerializeField] private int minimumSpawnRadius = 5;
    private const string unitResourcesLocation = "Spawnables/Unit - Master/Units/";
    private const string deployableResourcesLocation = "Spawnables/Unit - Master/Deployables/";

    [System.Serializable]
    public class UserInterface
    {
        [Header("Master Level UI")]
        public Text masterLevelText = null;
        public Image masterLevelImage = null;

        [Header("General UI")]
        public GameObject inGameUI;
        [Space]
        public Text energyText = null;
        public Slider energySlider = null;
        public Image energyFillImage;
        public Slider energyUseSlider = null;
        public Image energyUseFillImage;
        [Space]
        public Text xpText = null;
        public Text upgradeMenuXpText = null;
        public Text spawnText = null;
        [Space]
        public GameObject unitButtonPrefab;
        public Transform unitButtonContainer;
        [Space]
        public GameObject deployableButtonPrefab;
        public Transform deployableButtonContainer;
        [Space]
        public Image screenTint;
        [Space]
        public Image fadeImage;

        [Header("Unit Upgrade System UI")]
        public GameObject unitUpgradeMenu;
        public Transform upgradeMenuContainer;
        [Space]
        public GameObject unitUpgradePanel;
        [Space]
        public GameObject deployableUpgradePanel;
        public Transform deployableUgradeMenuContainer;

        [Header("Master Upgrade System UI")]
        public GameObject masterUpgradeMenu;
        public Text masterUpgradePointsText;
        public Button[] masterUpgradeButtons;
    }
    [Space]
    public UserInterface UI;

    private List<MasterUIGameplayButton> uiGameplayButtons = new List<MasterUIGameplayButton>();

    [System.Serializable]
    public class TopdownMaster
    {
        public Camera camera = null;
    }
    [Space]
    public TopdownMaster topdown;

    [System.Serializable]
    public class FlyingMaster
    {
        public UnitMaster_FlyingController flyingController;
    }
    [Space]
    public FlyingMaster flying;

    [Space]
    [SerializeField] private bool inTopdownView = true;

    [Title("Other", titleAlignment: TitleAlignments.Centered)]
    [SerializeField] private LayerMask ignoreOnRaycast = 1 << 2;
    [SerializeField] private LayerMask ignoreOnViewCheck = 1 << 9;
    [Space]
    [SerializeField] private Light tintLight = null;

    [Title("Particles and Effects", titleAlignment: TitleAlignments.Centered)]
    public ParticleSystem spawnSmokeEffect;
    private AudioSource spawnSmokeAudio;
    private AudioSource globalAudio;

    [Title("Network", titleAlignment: TitleAlignments.Centered)]
    [SerializeField] private GameObject[] disableForOthers = null;



    #endregion

    #region Start / Awake / Setup

    private void Start()
    {
        if (!hasAuthority)
        {
            foreach(GameObject _gameObject in disableForOthers)
            {
                _gameObject.SetActive(false);
            }
        }
        if (test)
        {
            SetMasterClass(masterSO);
        }
    }

    private void OnLevelChanged(int value)
    {
        Rpc_UpdateMasterLevelUI(connectionToClient, value);

        MasterUpgrades++;
        playerData.Score += 100;

        Rpc_UnlockMasterUpgradeButtons(connectionToClient, value);
    }
    private void OnExpChanged(int value)
    {
        Rpc_UpdateXPProgressBar(connectionToClient, value - playerData.previousExpRequired, playerData.ExpRequired - playerData.previousExpRequired);

        Rpc_UpdateXpUI(connectionToClient, value);
    }
    private void OnExpRequirementChanged(int value)
    {
        Rpc_UpdateXPProgressBar(connectionToClient, playerData.Exp - playerData.previousExpRequired, value - playerData.previousExpRequired);
    }


    public void Initialize()
    {
        name += $" ({masterSO.masterName})";       

        globalAudio = GetComponent<AudioSource>();
        globalAudio.clip = masterSO.globalSound;

        if (isServer)
        {
            playerData = GetComponent<MasterPlayerData>();

            //Default starting energy stats
            currentEnergy = masterSO.startEnergy;
            maxEnergy = masterSO.startMaxEnergy;
            energyRechargeIncrement = masterSO.energyRechargeIncrement;
            maxEnergyIncrement = masterSO.maxEnergyUpgradeIncrement;

            playerData.BaseExpRequired = masterSO.baseExpRequired;
            playerData.onExpChanged += OnExpChanged;
            playerData.onExpRequirementChanged += OnExpRequirementChanged;
            playerData.onLevelChanged += OnLevelChanged;

            Rpc_UnlockMasterUpgradeButtons(connectionToClient, 0);

            StartCoroutine(EnergyCoroutine());
        }

        if (!hasAuthority) return;

        //Set all the units/deployables' stats
        SetMasterUnits();
        SetMasterDeployables();

        //Instatiate and set unit buttons' functions
        InitializeUnitButtons();
        InitializeDeployableButtons();

        //Setup the different camera modes
        flying.flyingController = flying.flyingController.GetComponent<UnitMaster_FlyingController>();
        flying.flyingController.master = this; //Assign the reference.
        inTopdownView = false; //This bool is simply to stop a raycast from being performed.
            //The bool will be set to true in this function.
        SwitchCamera(true);

        //Energy UI visuals
        UI.energyFillImage.color = masterSO.energyColor;
        UI.energyUseFillImage.color = masterSO.energyUseColor;

        tintLight.color = masterSO.topdownLightColor;
        UI.screenTint.color = masterSO.screenTintColor;

        //Update the Energy UI
        UpdateEnergyUI();
        UpdateEnergyUseUI(0);

        UI.unitUpgradeMenu.SetActive(false);

        //Other master visuals

        //Change the Flying controller's marker visuals (Mesh and colour)
        flying.flyingController.ChangeMarker(masterSO.markerMesh, masterSO.markerColor);
        EnableFlyingMarker(false); //Disable the marker on start. Default view mode is Topdown.

        //-----------------------

        //Misc
        spawnSmokeAudio = spawnSmokeEffect.GetComponent<AudioSource>();
        spawnSmokeAudio.clip = masterSO.spawnSound;

        //Attach the master's custom script to the gameobject.
        System.Type masterType = System.Type.GetType(masterSO.masterClass.name + ",Assembly-CSharp");
        mClass = (UnitMasterClass)gameObject.AddComponent(masterType);
        mClass.UseSpecial();
    }

    public override void OnStartClient()
    {
        if (hasAuthority)
        {
            AddBinds();
        }
    }
    public override void OnStopClient()
    {
        if (hasAuthority)
        {
            RemoveBinds();
        }
    }

    #region Binds

    private void AddBinds()
    {
        // Left Mouse Input
        JODSInput.Controls.Master.LMB.performed += ctx => LMB();
        JODSInput.Controls.Master.LMB.performed += ctx => HoldLMB_Start();
        JODSInput.Controls.Master.LMB.canceled += ctx => HoldLMB_Stop();

        // Right Mouse Input
        JODSInput.Controls.Master.RMB.performed += ctx => RMB();
        JODSInput.Controls.Master.RMB.performed += ctx => HoldRMB_Start();
        JODSInput.Controls.Master.RMB.canceled += ctx => HoldRMB_Stop();

        //Shift Input
        JODSInput.Controls.Master.Shift.started += ctx => ShiftButton(true);
        JODSInput.Controls.Master.Shift.canceled += ctx => ShiftButton(false);

        //Ctrl Input
        JODSInput.Controls.Master.Ctrl.started += ctx => CtrlButton(true);
        JODSInput.Controls.Master.Ctrl.canceled += ctx => CtrlButton(false);

        //Alt Input
        JODSInput.Controls.Master.Alt.started += ctx => AltButton(true);
        JODSInput.Controls.Master.Alt.canceled += ctx => AltButton(false);

        //Unit Select Input
        JODSInput.Controls.Master.UnitSelecting.performed += ctx => ChooseUnit(null, Mathf.FloorToInt(ctx.ReadValue<float>() - 1));

        //Camera Change Input
        JODSInput.Controls.Master.ChangeCamera.performed += ctx => SwitchCamera(!inTopdownView);

        //Take Control of unit Input
        JODSInput.Controls.Master.TakeControl.performed += ctx => TryToTakeControl();

        //Upgrade menu Input
        JODSInput.Controls.Master.OpenUnitUpgradeMenu.performed += ctx => OpenUnitUpgradeMenu();
        JODSInput.Controls.Master.OpenMasterUpgradeMenu.performed += ctx => OpenMasterUpgradeMenu();

        //Menu Input
        PlayerManager.Instance.onMenuOpened += OnMenuEnabled;
        PlayerManager.Instance.onMenuClosed += OnMenuDisabled;
    }

    private void RemoveBinds()
    {
        // Left Mouse Input
        JODSInput.Controls.Master.LMB.performed -= ctx => LMB();
        JODSInput.Controls.Master.LMB.performed -= ctx => HoldLMB_Start();
        JODSInput.Controls.Master.LMB.canceled -= ctx => HoldLMB_Stop();

        // Right Mouse Input
        JODSInput.Controls.Master.RMB.performed -= ctx => RMB();
        JODSInput.Controls.Master.RMB.performed -= ctx => HoldRMB_Start();
        JODSInput.Controls.Master.RMB.canceled -= ctx => HoldRMB_Stop();

        //Shift Input
        JODSInput.Controls.Master.Shift.started -= ctx => ShiftButton(true);
        JODSInput.Controls.Master.Shift.canceled -= ctx => ShiftButton(false);

        //Ctrl Input
        JODSInput.Controls.Master.Ctrl.started -= ctx => CtrlButton(true);
        JODSInput.Controls.Master.Ctrl.canceled -= ctx => CtrlButton(false);

        //Alt Input
        JODSInput.Controls.Master.Alt.started += ctx => AltButton(true);
        JODSInput.Controls.Master.Alt.canceled += ctx => AltButton(false);

        // Unit Select Input
        JODSInput.Controls.Master.UnitSelecting.performed -= ctx => ChooseUnit(null, Mathf.FloorToInt(ctx.ReadValue<float>() - 1));

        //Camera Change Input
        JODSInput.Controls.Master.ChangeCamera.performed -= ctx => SwitchCamera(!inTopdownView);

        //Take Control of unit Input
        JODSInput.Controls.Master.TakeControl.performed -= ctx => TryToTakeControl();

        //Upgrade menu Input
        JODSInput.Controls.Master.OpenUnitUpgradeMenu.performed -= ctx => OpenUnitUpgradeMenu();
        JODSInput.Controls.Master.OpenUnitUpgradeMenu.performed -= ctx => OpenMasterUpgradeMenu();


        //Menu Input
        PlayerManager.Instance.onMenuOpened -= OnMenuEnabled;
        PlayerManager.Instance.onMenuClosed -= OnMenuDisabled;
    }
    #endregion

    private void OnValidate()
    {
        if (masterSO)
        {
            SetMasterUnits();
            SetMasterDeployables();
        }
        //If there is no master class selected, then clear the list of units
        else if (unitList.Count != 0) unitList.Clear();
    }

    private void OnMenuEnabled()
    {
        JODSInput.DisableCamera();
    }
    private void OnMenuDisabled()
    {
        JODSInput.EnableCamera();
    }

    #endregion

    #region Input Functions

    private bool shift = false;
    private bool ctrl = false;
    private bool alt = false;
    private void ShiftButton(bool down)
    {
        shift = down;
    }
    private void CtrlButton(bool down)
    {
        ctrl = down;
    }
    private void AltButton(bool down)
    {
        alt = down;
        if (!inTopdownView)
        {
            //Change the cursor settings to be either visible or not visible
            Cursor.lockState = down ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = down; //Visible in Topdown view mode, not visible in flying mode
        }
    }

    [ClientRpc]
    public void Rpc_SetMasterClass(string _class)
    {
        List<UnitMasterSO> masterSOList = PlayableCharactersManager.Instance.GetAllMasters();

        foreach (UnitMasterSO master in masterSOList)
        {
            if (master.name == _class)
            {
                SetMasterClass(master);
                break;
            }
        }
    }
    public void SetMasterClass(UnitMasterSO masterSO)
    {
        this.masterSO = masterSO;

        Initialize();
    }

    #region Normal Mouse
    #region LMB
    private void LMB()
    {
        if (alt) return;
        if (EventSystem.current.IsPointerOverGameObject()) { return; }
        if (shift && !ctrl) { Shift_LMB(); return; }
        else if (ctrl && !shift) { Ctrl_LMB(); return; }
        DeselectUnit();

        //Somehow check if master clicks on a unit button, if so do not spawn anything.
        TryToSpawnUnit();
    }

    private Coroutine holdLMBco;
    private bool holdLMB;
    private IEnumerator HoldLMB_IE()
    {
        holdLMB = true;
        while (true)
        {
            yield return new WaitForSeconds(0.075f);

            LMB();
        }
    }
    private void HoldLMB_Start()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        holdLMBco = StartCoroutine(HoldLMB_IE());
    }
    private void HoldLMB_Stop()
    {
        if (!holdLMB) return;
        holdLMB = false;
        StopCoroutine(holdLMBco);
    }
    #endregion
    #region RMB
    private void RMB()
    {
        if (alt) return;
        if (EventSystem.current.IsPointerOverGameObject()) { return; }
        if (shift && !ctrl) { Shift_RMB(); return; }
        else if (ctrl && !shift) { Ctrl_RMB(); return; }

        DeselectUnit();
        //Unchoose the current unit type
        Unchoose();
    }
    private Coroutine holdRMBco;
    private bool holdRMB;
    private IEnumerator HoldRMB_IE()
    {
        holdRMB = true;
        while (true)
        {
            yield return new WaitForSeconds(0.075f);

            RMB();
        }
    }
    private void HoldRMB_Start()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        holdRMBco = StartCoroutine(HoldRMB_IE());
    }
    private void HoldRMB_Stop()
    {
        if (!holdRMB) return;
        holdRMB = false;
        StopCoroutine(holdRMBco);
    }
    #endregion

    #endregion
    #region Shift Mouse
    private void Shift_LMB()
    {
        TryToSelectUnit();
    }
    private void Shift_RMB()
    {
        TryToCommandUnit();
    }
    #endregion
    #region CTRL Mouse Raycasts
    private void Ctrl_LMB()
    {
        DeselectUnit();
        TryToSpawnUnit();
    }
    private void Ctrl_RMB()
    {
        DeselectUnit();
        TryToRefundUnit();
    }
    #endregion
    public void UpgradeMasterEnergy(MasterUpgradeType upgradeType)
    {
        Cmd_UpgradeEnergy(upgradeType);       
    }

    [Command]
    private void Cmd_UpgradeEnergy(MasterUpgradeType upgradeType)
    {
        MasterUpgrades--;

        //Play a sound
        Rpc_PlayGlobalSound(true);

        switch (upgradeType)
        {
            case MasterUpgradeType.RechargeRate:
                //Increase the increment
                energyRechargeIncrement += 1;
                break;
            case MasterUpgradeType.MaxEnergy:
                //Increase the max amount of energy
                maxEnergy += maxEnergyIncrement;
                break;
            case MasterUpgradeType.UnitCapacity:
                foreach(UnitList unit in unitList)
                {
                    unit.maxAmount += unit.unit.maxAmountAliveIncrement;
                    unit.CurrentAmount = unit.CurrentAmount; //This updates the ui, so the new max amount becomes visible.
                }
                break;
            case MasterUpgradeType.SurvivorOutlines:
                print("We dont gotgitget outlnes yuet");
                break;
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator EnergyCoroutine()
    {
        while (true)
        {
            //Wait 1 second between each energy increment
            yield return new WaitForSeconds(1);

            //Increment the energy by energyRechargeIncrement, clamping it at the max amount of energy
            Energy += energyRechargeIncrement;
        }
    }

    #endregion

    #region UI Functions

    private void InitializeUnitButtons()
    {
        for (int i = 0; i < unitList.Count; i++)
        {
            //Reference
            UnitList u = unitList[i];

            //Instantiate unit button prefab
            GameObject button = Instantiate(UI.unitButtonPrefab, UI.unitButtonContainer);
            MasterUIGameplayButton b = button.GetComponent<MasterUIGameplayButton>();

            u.unitButton = b;

            GameObject upgradePanelGO = Instantiate(UI.unitUpgradePanel, UI.upgradeMenuContainer);
            UnitUpgradePanel upgradePanel = upgradePanelGO.GetComponent<UnitUpgradePanel>();

            u.upgradePanel = upgradePanel;
            upgradePanel.InitializeUnitUpgradePanel(this, u.unit, i);

            //Add this button to the list
            uiGameplayButtons.Add(b);

            u.buttonIndex = i;
            
            //Details
            b.SetDetails(u.unit.unitSprite, u.unit.name, u.unit.description, u.unit.powerStat, u.unit.healthStat);

            //Add an event to call the function ChooseUnit whenever the button is pressed
            button.GetComponent<Button>().onClick.AddListener(delegate { ChooseUnit(u); });

            //Start the unit button as Unlocked or Locked
            u.Unlock(u.unlocked);
        }
    }

    private void InitializeDeployableButtons()
    {
        //This is used to assign the indexes of these buttons
        int referenceIndex = uiGameplayButtons.Count;

        for (int i = 0; i < deployableList.Count; i++)
        {
            //Check if the index has a deployable assigned, if not continue to next index.
            if (!deployableList[i].deployable)
            {
                Debug.LogError($"Deployable with index {i} has no deployable assigned!");
                continue; //Go to the next iteration
            }

            //Reference
            DeployableList d = deployableList[i];

            d.buttonIndex = referenceIndex + i;

            //Instantiate unit button prefab
            GameObject button = Instantiate(UI.deployableButtonPrefab, UI.deployableButtonContainer);
            MasterUIGameplayButton b = button.GetComponent<MasterUIGameplayButton>();

            d.deployableButton = b;

            GameObject upgradePanelGO = Instantiate(UI.deployableUpgradePanel, UI.deployableUgradeMenuContainer);
            DeployableUpgradePanel upgradePanel = upgradePanelGO.GetComponent<DeployableUpgradePanel>();

            d.upgradePanel = upgradePanel;
            upgradePanel.InitializeUnitUpgradePanel(this, d.deployable, i);

            //Add this button to the list
            uiGameplayButtons.Add(b);

            //Details
            b.SetDetails(d.deployable.deployableSprite, d.deployable.name, d.deployable.description);

            //Add an event to call the function ChooseDeployable whenever the button is pressed
            button.GetComponent<Button>().onClick.AddListener(delegate { ChooseDeployable(d); });

            //Start the unit button as Unlocked or Locked
            d.Unlock(d.unlocked);
        }
    }

    private void UpdateEnergyUI()
    {
        //Energy UI
        UI.energyText.text = Energy.ToString() + "/" + maxEnergy;
        UI.energySlider.value = Energy;
    }

    private void UpdateEnergyUseUI(int use)
    {
        UI.energyUseSlider.value = use;
    }

    private void UnitButtonChooseUI(bool choose)
    {
        uiGameplayButtons[chosenSpawnableIndex].Choose(choose);
    }

    private void SetSpawnText(string newText)
    {
        if (spawnTextBool)
        {
            StopCoroutine(spawnTextCo);
        }
        spawnTextCo = StartCoroutine(IESpawnText(newText));
    }
    private Coroutine spawnTextCo = null;
    private bool spawnTextBool = false;
    private IEnumerator IESpawnText(string newText)
    {
        spawnTextBool = true;

        UI.spawnText.text = newText;
        UI.spawnText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        UI.spawnText.text = "";
        UI.spawnText.gameObject.SetActive(false);

        spawnTextBool = false;
    }

    [TargetRpc]
    private void Rpc_SetSpawnText(NetworkConnection target, string newText)
    {
        SetSpawnText(newText);
    }

    public void OpenUnitUpgradeMenu()
    {
        bool active = !UI.unitUpgradeMenu.activeSelf;
        UI.unitUpgradeMenu.SetActive(active);
        UI.inGameUI.SetActive(!active);
    }

    public void OpenMasterUpgradeMenu()
    {
        bool active = !UI.masterUpgradeMenu.activeSelf;
        UI.masterUpgradeMenu.SetActive(active);
    }

    [Server]
    public void Svr_SetUnitAmountText(int amount, int maxAmount, int index)
    {
        Rpc_SetUnitAmountText(connectionToClient, amount, maxAmount, index);
    }

    [TargetRpc]
    private void Rpc_SetUnitAmountText(NetworkConnection target, int amount, int maxAmount, int index)
    {
       GetUnitList(index).unitButton.UpdateUnitAmount(amount, maxAmount);
    }

    #endregion

    #region Particles and Effects Functions

    [TargetRpc]
    private void Rpc_SpawnEffect(NetworkConnection target, Vector3 point, bool smoke)
    {
        //Smoke particles
        spawnSmokeEffect.transform.position = point;
        spawnSmokeEffect.transform.SetParent(null);

        if (smoke)
        {
            spawnSmokeEffect.Emit(50);
        }

        //Spawn audio
        spawnSmokeAudio.pitch = Random.Range(0.9f, 1.1f);
        spawnSmokeAudio.PlayOneShot(spawnSmokeAudio.clip);
    }

    #endregion

    #region Other
    private bool IsUnitCloseToDestination()
    {
        if (selectedUnit)
        {
            return selectedUnit.GetComponent<AIPath>().reachedDestination;
        }
        else return false;
    }

    private void EnableFlyingMarker(bool enable)
    {
        flying.flyingController.ShowMarker(enable);
    }

    #endregion

    #region Gameplay Functions

    private void SetMasterUnits()
    {
        //This will assign all the units that the master class has, to the master. (Essentially this remakes the list)
        if (UnitInitialization() || unitList.Count == 0) SetMasterUnitsInEditor(); //Only if the current list is wrong though, which this bool method will check

        if (unitList.Count == 0) return; //If there are no units, there is no need to continue.

        SetMasterUnitValues();
    }

    private void SetMasterDeployables()
    {
        if (DeployableInitialization() || deployableList.Count == 0) SetMasterDeployablesInEditor();

        if (deployableList.Count == 0) return;

        SetMasterDeployableValues();
    }

    #region Choosing
    private void ChooseUnit(UnitList unit, int unitIndex = 100)
    {
        if (unitIndex != 100)
        {
            foreach(UnitList unitListItem in unitList)
            {
                if (unitListItem.unitIndex == unitIndex)
                {
                    unit = unitListItem;
                    break;
                }
            }
        }

        if (unit == null) return;

        //If the unit is unlocked
        if (!unit.unlocked) return;

        //If the master has already chosen a unit, unchoose that unit
        unit.chosen = false;
        if (hasChosenASpawnable) UnitButtonChooseUI(false);

        //Then choose the new unit
        Cmd_ChooseSpawnable(unit.buttonIndex);

        hasChosenASpawnable = true;
        unit.chosen = true;

        //And change the UI
        UnitButtonChooseUI(true);

        UpdateEnergyUseUI(unit.unit.energyCost);

        if (!inTopdownView)
        {
            //This will enable the marker for the flying camera, which is mostly a visual aid
            EnableFlyingMarker(true);
        }
    }

    private void ChooseDeployable(DeployableList deployable)
    {
        Unchoose(true);

        //If the deployable is not unlocked, return;
        if (!deployable.unlocked) return;

        Cmd_ChooseSpawnable(deployable.buttonIndex);
        hasChosenASpawnable = true;

        UnitButtonChooseUI(true);

        if (!inTopdownView)
        {
            //This will enable the marker for the flying camera, which is mostly a visual aid
            EnableFlyingMarker(true);
        }
    }

    [Command]
    private void Cmd_ChooseSpawnable(int number)
    {
        chosenSpawnableIndex = number;
    }

    private void Unchoose(bool keepMarker = false)
    {
        hasChosenASpawnable = false;

        //Change the UI
        UnitButtonChooseUI(false);
        UpdateEnergyUseUI(0);

        if (!inTopdownView && !keepMarker)
        {
            //This will disable the marker for the flying camera
            EnableFlyingMarker(false);
        }
    }
    #endregion

    #region Spawning & Refunding
    void TryToSpawnUnit()
    {
        bool spawningADeployable = chosenSpawnableIndex >= unitList.Count;

        //Have I chosen anyting to spawn?
        if (!hasChosenASpawnable) return; //if not then don't proceed

        Raycast(out bool didHit, out RaycastHit hit);

        if (didHit)
        {
            //Did the raycast hit an area where spawnables can spawn? 
            //(The "Ground" tag is currently the only surface where spawnables can spawn)
            if (hit.collider.CompareTag("Ground"))
            {
                //Spawn Deployable
                if (spawningADeployable)
                {
                    int referenceIndex = chosenSpawnableIndex - unitList.Count;

                    DeployableList listItem = deployableList[referenceIndex];

                    if (listItem.onCooldown)
                    {
                        SetSpawnText("On cooldown");
                    }
                    else
                    {
                        Spawn(hit, spawningADeployable);
                    }
                }
                else //Spawn Unit
                {
                    //Do I have enough energy to spawn the chosen unit?
                    if (unitList[chosenSpawnableIndex].unit.energyCost <= currentEnergy)
                    {
                        //If yes, spawn the unit at the location
                        Spawn(hit, spawningADeployable);
                    }
                    else
                    {
                        SetSpawnText("Not enough energy");
                    }
                }
            }
            else
            {
                SetSpawnText($"Cannot spawn {(spawningADeployable ? "deployable" : "unit")} here");
            }
        }
    }

    void Spawn(RaycastHit hit, bool spawningADeployable)
    {
        //Check if the spawn location meets the requirements.
        if (!ViewCheck(hit.point, true)) return;

        string spawnName;

        //When spawning a deployable
        if (spawningADeployable)
        {
            int referenceIndex = chosenSpawnableIndex - unitList.Count;

            DeployableList listItem = deployableList[referenceIndex];
            DeployableSO chosenDeployable = listItem.deployable;
            spawnName = chosenDeployable.deployablePrefab.name;

            uiGameplayButtons[chosenSpawnableIndex].StartCooldown(listItem.deployableCooldown);
            StartCoroutine(DeployableCooldown(listItem));           

            Unchoose();

            if (deployableDict.ContainsKey(spawnName))
            {
                Cmd_MoveDeployable(hit.point, spawnName);
                return;
            }
            else
            {
                if (!isServer) deployableDict.Add(spawnName, null); //Add an entry to the dictionary with a null value.
                //The server will have its own version of this dictionary, but it will contain a valid gameobject.
                //The client only needs the key.
            }
        }
        else //When spawning a unit
        {
            //References
            UnitList chosenUnitList = unitList[chosenSpawnableIndex];
            UnitSO chosenUnit = chosenUnitList.unit;

            //If the spawn location meets the requirements, then spawn the currently selected type of unit.

            //A random unit from the chosen unit's prefab list gets picked, and the name gets sent to the server, which then spawns the unit.
            //This is because there can be multiple variations of one unit.
            spawnName = chosenUnit.unitPrefab.name;
        }

        Cmd_Spawn(hit.point, spawnName, spawningADeployable);
    }

    IEnumerator DeployableCooldown(DeployableList deployable)
    {
        deployable.onCooldown = true;

        yield return new WaitForSeconds(deployable.deployableCooldown);

        deployable.onCooldown = false;
    }

    void TryToRefundUnit()
    {
        Raycast(out bool didHit, out RaycastHit hit);

        if (didHit)
        {
            //If I hit a unit, try and refund that unit
            if (hit.collider.TryGetComponent(out UnitBase unit))
            {
                int refundAmount = unit.Refund;
                if (refundAmount != 0)
                {
                    RefundUnit(unit, refundAmount);
                }
                else print($"{name} has taken damage and could not be refunded");
            }
        }
    }

    void RefundUnit(UnitBase unitToRefund, int refundAmount)
    {
        //Check if the refund location meets the requirements.
        if (!ViewCheck(unitToRefund.transform.position, false)) return;

        //If the location meets the requirements, then refund the unit.
        Cmd_RefundUnit(unitToRefund.gameObject, refundAmount);
    }
    #endregion

    #region Upgrade & Unlock
    #region Upgrading Stats
    public void UpgradeUnit(int unitIndex, int upgradePath, float upgradeAmount)
    {
        Cmd_UpgradeUnit(unitIndex, upgradePath, upgradeAmount);

        //Play spooky sound
        Cmd_PlayGlobalSound(true);
    }

    [Command]
    private void Cmd_UpgradeUnit(int unitIndex, int upgradePath, float upgradeAmount)
    {
        //Reference
        UnitList unit = unitList[unitIndex];

        float newValue = 0;
        int upgradesLeft = 0;

        switch (upgradePath)
        {
            //Health upgrade
            case 0:
                upgradesLeft = --unit.upgradesTillHealthTrait;
                unit.modifiers.Health += upgradeAmount;
                newValue = unit.GetHealthStat();
                break;
            //Damage upgrade
            case 1:
                upgradesLeft = --unit.upgradesTillDamageTrait;
                unit.modifiers.MeleeDamage += upgradeAmount;
                newValue = unit.GetDamageStat();
                break;
            //Speed upgrade
            case 2:
                upgradesLeft = --unit.upgradesTillSpeedTrait;
                unit.modifiers.MovementSpeed += upgradeAmount;
                newValue = unit.GetSpeedStat();
                break;
        }

        UnitLevelUp(unit);

        Rpc_UpdateStats(netIdentity.connectionToClient, unitIndex, newValue, upgradesLeft, upgradePath);
    }

    [TargetRpc]
    private void Rpc_UpdateStats(NetworkConnection target, int unitIndex, float newValue, int upgradesLeft, int upgradePath)
    {
        //Reference
        UnitList unit = unitList[unitIndex];

        switch (upgradePath)
        {
            //Health upgrade
            case 0:
                unit.upgradePanel.UpdateHealthText(newValue,upgradesLeft);
                break;
            //Damage upgrade
            case 1:
                unit.upgradePanel.UpdateDamageText(newValue, upgradesLeft);
                break;
            //Speed upgrade
            case 2:
                unit.upgradePanel.UpdateSpeedText(newValue, upgradesLeft);
                break;
        }
    }
    #endregion
    #region Unlocking Traits
    public void UnlockTrait(int unitIndex, int upgradePath)
    {
        Cmd_UnlockTrait(unitIndex, upgradePath);

        //Play spooky sound
        Cmd_PlayGlobalSound(true);
    }

    [Command]
    public void Cmd_UnlockTrait(int unitIndex, int upgradePath)
    {
        //Reference
        UnitList unit = unitList[unitIndex];

        switch (upgradePath)
        {
            //Health upgrade
            case 0:
                unit.hasHealthTrait = true;
                break;
            //Damage upgrade
            case 1:
                unit.hasDamageTrait = true;
                break;
            //Speed upgrade
            case 2:
                unit.hasSpeedTrait = true; ;
                break;
        }

        UnitLevelUp(unit);        
    }
    #endregion

    private void UnitLevelUp(UnitList unit)
    {
        playerData.TotalUnitUpgrades++;

        unit.upgradesAvailable--;
        Rpc_SetUpgradesAvailable(netIdentity.connectionToClient, unit.unitIndex, unit.upgradesAvailable);
    }

    public void UnlockNew(UnitList unit = null, DeployableList deployable = null)
    {
        if (deployable != null)
        {
            deployable.Unlock(true);

            deployable.unlocked = true;

            //Cmd_SetCurrentXP(CurrentXP - deployable.deployable.xpToUnlock);
        }
        else if (unit != null)
        {
            unit.Unlock(true);

            //Unlock the unit
            unit.unlocked = true;

            //Decrease xp by amount required to unlock the unit
            //Cmd_SetCurrentXP(CurrentXP - unit.unit.xpToUnlock);
        }

        //Play spooky sound
        Cmd_PlayGlobalSound(true);
    }
    #endregion

    #region Selecting & Commanding
    private void TryToSelectUnit()
    {
        Raycast(out bool didHit, out RaycastHit hit);

        if (didHit)
        {
            //If I click on a unit, try and select that unit
            if (hit.collider.TryGetComponent(out UnitBase unit))
            {
                if (unit.select.canSelect && !unit.IsDead)
                {
                    SelectUnit(unit);
                }
            }
            //If I click on nothing, deselect my unit.
            else
            {
                DeselectUnit();
            }
        }
    }
    private void SelectUnit(UnitBase unit)
    {
        //If a unit is already selected, deselect it before selecting the new one.
        DeselectUnit();

        //Select the unit
        selectedUnit = unit;
        selectedUnit.Select(masterSO.unitSelectColor);
        Cmd_SetSelectedUnit(unit.gameObject);
    }
    private void DeselectUnit()
    {
        if (!selectedUnit) return;
        selectedUnit.Deselect();
        selectedUnit = null;
        Cmd_SetSelectedUnit(null);
    }

    [Command]
    private void Cmd_SetSelectedUnit(GameObject unit)
    {
        selectedUnit = unit != null ? unit.GetComponent<UnitBase>() : null;
    }
    private void TryToCommandUnit()
    {
        //If I don't have a unit currently selected, then return
        if (!selectedUnit) return;

        Raycast(out bool didHit, out RaycastHit hit);

        if (didHit)
        {
            if (hit.transform.TryGetComponent(out LiveEntity destructible))
            {
                selectedUnit.AcquireTarget(hit.transform, true, true, true);
            }

            //Command my selected unit to move to the location
            Cmd_MoveToLocation(hit.point);
        }
    }
    #endregion

    #region Control

    private void TryToTakeControl()
    {
        if (!takingControlBool) {
            if (Energy < 30)
            {
                SetSpawnText("Not enough energy to take control of units");
                return;
            }
            Raycast(out bool didHit, out RaycastHit hit);
            if (didHit)
            {
                if (hit.transform.TryGetComponent(out UnitBase unitBase))
                {
                    StartCoroutine(IETakingControl(unitBase));
                }
            }
        }
    }
    bool takingControlBool = false;
    private IEnumerator IETakingControl(UnitBase targetToControl)
    {
        takingControlBool = true;

        JODSInput.Controls.Master.Disable();

        Camera currentCamera = GetCurrentCamera;
        if (!inTopdownView)
        {
            GameObject zoomCamera = 
                Instantiate(currentCamera.gameObject, currentCamera.transform.position, currentCamera.transform.rotation);

            currentCamera = zoomCamera.GetComponent<Camera>();
        }

        float elapsedTime = 0;
        Color fade = UI.fadeImage.color;
        while (elapsedTime < 1)
        {
            if (inTopdownView)
            {
                currentCamera.orthographicSize -= Time.deltaTime * 10;
            }
            else
            {
                currentCamera.fieldOfView += Time.deltaTime * 20;
                currentCamera.transform.position = 
                    Vector3.Lerp(currentCamera.transform.position, targetToControl.transform.position, Time.deltaTime);
            }

            elapsedTime += Time.deltaTime;

            fade.a += Time.deltaTime;
            UI.fadeImage.color = fade;
            yield return null;
        }
        if (!inTopdownView) Destroy(currentCamera);
        yield return new WaitForSeconds(1f);

        //TAKE CONTROL OF DA ZOMBE
    }

    #endregion

    #region Checks

    //This function will shoot a ray from the currently used camera, and will return a Raycast Hit
    //This is used by all functions with raycast requirements
    //This function is to reduce redundancy, because there are multiple functions that use raycasts.
    //Even other scripts such as the Master_FlyingController uses this to do raycasts.
    public void Raycast(out bool didHit, out RaycastHit rayHit)
    {
        Ray ray; //Empty variables
        int distance;

        //Is the master currently in topdown view?
        if (inTopdownView)
        {
            //Send the ray from the topdown camera
            ray = topdown.camera.ScreenPointToRay(Input.mousePosition);

            distance = 150;
        }
        //Is the master currently in flying view?
        else
        {
            //Send the ray from the flying camera
            ray = flying.flyingController.cam.ScreenPointToRay(Input.mousePosition);

            distance = 20; //Decrease the distance of the raycast.
        }

        //Perform the raycast with the chosen parameters
        //The bool will be assigned to true if the raycast hit anything.
        didHit = Physics.Raycast(ray, out RaycastHit hit, distance, ~ignoreOnRaycast);
        //The hit will be assigned as the raycast hit from the raycast.
        rayHit = hit;
        //Return the variables
        return;
    }

    //This function determines if a position is within the requirements to either spawn or refund units.
    private bool ViewCheck(Vector3 where, bool spawn)
    {
        //The position to check from. (where the unit will spawn or despawn)
        Vector3 pos = new Vector3(where.x, where.y + 2, where.z);

        //Send out an OverlapSphere, to check for nearby survivors in range.
        Collider[] survivorsInRadius = Physics.OverlapSphere(pos, spawnCheckRadius, playerLayer);

        //Iterate through each survivor in range
        foreach (Collider survivor in survivorsInRadius)
        {
            //Get the position of the survivor, and get the direction to check for visibility of the survivor.
            Vector3 pPos = new Vector3(survivor.transform.position.x, survivor.transform.position.y + 0.5f, survivor.transform.position.z);
            Vector3 dir = pPos - pos;

            Debug.DrawLine(pos, pPos, Color.cyan, 20f, false);

            //Do a raycast, to check if it hits anything on the way to the survivor.
            if (Physics.Raycast(pos, dir, out RaycastHit newhit, spawnCheckRadius, ~ignoreOnViewCheck))
            {
                Debug.DrawRay(survivor.transform.position, survivor.transform.forward * 5, Color.blue,2f);

                //If it hits something, then check if it is a player or not.
                if (newhit.transform == survivor.transform)
                {
                    //If it does hit the survivor, then first check if it is within the survivor's view angle.
                    dir = pos - pPos;
                    float angle = Vector3.Angle(dir, survivor.transform.forward);

                    //Debugs
                    Debug.DrawLine(pos, pPos, angle > 60 ? Color.green : Color.red, 10f, false);

                    //Is it inside the view angle of the survivor
                    if (angle < 60)
                    {
                        //If it is within the view angle, then it cannot spawn a unit.
                        SetSpawnText( spawn ?
                            "Must spawn out of view of survivors" :
                            "Cannot refund unit in view of survivors"
                            );
                        return false;
                    }
                    //Check if it is within the minimum distance to spawn away from a survivor.
                    if (Vector3.Distance(pos, survivor.transform.position) <= minimumSpawnRadius)
                    {
                        //If it is within the minimum spawn distance, then it cannot spawn a unit.
                        SetSpawnText(spawn ?
                            "Must spawn further away from survivors" :
                            "Cannot refund unit close to survivors"
                            );
                        return false;
                    }
                }
            }
        }

        //If everything is good.
        return true;
    }

    #endregion

    #endregion

    #region Editor Functions

    private void SetMasterUnitsInEditor()
    {
        int arraySize = masterSO.units.Length;
        unitList.Clear();

        for (int i = 0; i < arraySize; i++)
        {
            unitList.Add(new UnitList());
            unitList[i].unit = masterSO.units[i];
        }
    }

    private void SetMasterDeployablesInEditor()
    {
        int arraySize = masterSO.deployables.Length;
        deployableList.Clear();

        for (int i = 0; i < arraySize; i++)
        {
            deployableList.Add(new DeployableList());
            deployableList[i].deployable = masterSO.deployables[i];
        }
    }

    private bool UnitInitialization()
    {
        if (unitList.Count == 0) return true;
        //Check if the list is as it should be. If it is ok, there is no need to remake it.
        for (int i = 0; i < masterSO.units.Length; i++)
        {
            if (unitList[i].unit != masterSO.units[i])
            {
                //Remake the list
                return true;
            }
        }
        //List does not need to be remade
        return false;
    }

    private bool DeployableInitialization()
    {
        if (deployableList.Count == 0) return true;
        //Check if the list is as it should be. If it is ok, there is no need to remake it.
        for (int i = 0; i < masterSO.deployables.Length; i++)
        {
            if (deployableList[i].deployable != masterSO.deployables[i])
            {
                //Remake the list
                return true;
            }
        }
        //List does not need to be remade
        return false;
    }

    private void SetMasterUnitValues()
    {
        //Set the names and indexes of each unit, and set other values
        for (int i = 0; i < unitList.Count; i++)
        {
            UnitList u = unitList[i];
            //Set name of unit
            if (u.unit) u.name = u.unit.name;

            //Set index of unit
            u.unitIndex = i;

            //Set this unit to be unlocked, if it is a starter unit
            u.unlocked = u.unit.starterUnit;

            u.maxAmount = u.unit.maxAmountAlive;

            u.upgradesTillHealthTrait = u.unit.upgrades.unitUpgradesHealth.amountOfUpgrades;
            u.upgradesTillDamageTrait = u.unit.upgrades.unitUpgradesDamage.amountOfUpgrades;
            u.upgradesTillSpeedTrait = u.unit.upgrades.unitUpgradesSpeed.amountOfUpgrades;

            u.totalUpgrades = u.upgradesTillHealthTrait + u.upgradesTillDamageTrait + u.upgradesTillSpeedTrait;

            u.upgradeMilestone = u.unit.upgrades.unitsToPlace;
            u.upgradeCurve = u.unit.upgrades.upgradeCurve;

            u.modifiers = new ModifierManagerUnitData();

            u.masterReference = this;
        }
    }

    private void SetMasterDeployableValues()
    {
        //Set the names and values of each deployable
        for (int i = 0; i < deployableList.Count; i++)
        {
            DeployableList d = deployableList[i];

            if (d.deployable) d.name = d.deployable.name;
           
            d.unlocked = false;

            d.deployableCooldown = d.deployable.cooldownOnUse;

            d.onCooldown = false;
        }
    }

    #endregion

    #region Movement
    private void SwitchCamera(bool top)
    {
        //If the master is currently in topdown view (switching to flying),
        //Then set the position of the flying camera to be at the mouse cursor.
        if (inTopdownView == true)
        {
            Raycast(out bool didHit, out RaycastHit hit);

            if (didHit)
            {
                flying.flyingController.transform.position = new Vector3(hit.point.x,
                    hit.point.y + 3,
                    hit.point.z);
            }
        }

        //Switch the bool
        inTopdownView = top;

        //Deactivate / Activate the cameras based on the bool
        topdown.camera.gameObject.SetActive(top);
        flying.flyingController.gameObject.SetActive(!top);

        //Change the cursor settings to be either visible or not visible
        Cursor.lockState = top ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = top; //Visible in Topdown view mode, not visible in flying mode

        tintLight.enabled = top;

        if (hasChosenASpawnable) //If a unit is currently chosen, then enable/disable marker
        {
            //If changing to flying camera, enable the unit marker, which is mostly a visual aid.
            EnableFlyingMarker(!inTopdownView);
            //If changing to topdown, then disable it.
        }
    }

    private Camera GetCurrentCamera => inTopdownView ? topdown.camera : flying.flyingController.cam;

    #endregion

    #region Network Commands

    [Command]
    void Cmd_MoveToLocation(Vector3 location)
    {
        selectedUnit.Svr_MoveToLocation(location);
    }

    [Command]
    void Cmd_Spawn(Vector3 pos, string name, bool deployable)
    {
        //If the type is 0, then spawn a deployable, not a unit
        bool spawningDeployable = deployable;

        if (spawningDeployable)
        {

        }
        else
        {
            UnitList chosenUnitList = unitList[chosenSpawnableIndex];
            if (chosenUnitList.CurrentAmount >= chosenUnitList.maxAmount)
            {
                Rpc_SetSpawnText(netIdentity.connectionToClient, "Max amount of " + chosenUnitList.name + " alive.");
                return;
            }
        }

        //Set the positions y value to be a bit higher, so that the spawnable doesn't spawn inside the floor
        pos = new Vector3(pos.x, pos.y + 0.1f, pos.z);

        string resourceLocation = spawningDeployable ? deployableResourcesLocation : unitResourcesLocation;

        //Get the spawnable to spawn
        GameObject spawnableToSpawn = (GameObject)Resources.Load(resourceLocation + $"{name}");
        //print(resourceLocation + $"{name}");

        if (spawnableToSpawn == null)
        {
            string errorName = (spawningDeployable ? "deployable" : "unit");
            Debug.LogError($"Could not spawn {name} {errorName}, didn't find the {errorName} in '{resourceLocation}', make sure they are in that folder");
            return;
        }

        //Instantiate the spawnable at the position
        GameObject newSpawnable = Instantiate(spawnableToSpawn, pos, Quaternion.identity);

        //Set the spawnable's rotation to be random
        newSpawnable.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));

        //If we're spawning a unit
        if (!spawningDeployable)
        {
            //Then set the unit level and unit SO of that unit on spawn
            UnitBase unit = newSpawnable.GetComponent<UnitBase>();

            UnitList chosenUnitList = unitList[chosenSpawnableIndex];

            unit.GetComponent<ModifierManagerUnit>().data = chosenUnitList.modifiers;

            chosenUnitList.upgradeMilestone = (int)Mathf.Clamp(chosenUnitList.upgradeMilestone -= 1, 0, Mathf.Infinity);
            Rpc_UpdateMilestoneForClient(netIdentity.connectionToClient, chosenSpawnableIndex, chosenUnitList.upgradeMilestone);

            if (chosenUnitList.upgradeMilestone <= 0)
            {
                chosenUnitList.upgradesAvailable++;
                Rpc_SetUpgradesAvailable(netIdentity.connectionToClient, chosenSpawnableIndex, chosenUnitList.upgradesAvailable);

                chosenUnitList.level++;

                int newMilestone = Mathf.RoundToInt(chosenUnitList.unit.upgrades.unitsToPlace *
                (1 + chosenUnitList.upgradeCurve.Evaluate(
                    (chosenUnitList.upgradeCurve.keys[chosenUnitList.upgradeCurve.keys.Length - 1].time / 
                    chosenUnitList.totalUpgrades) * chosenUnitList.level)));

                chosenUnitList.upgradeMilestone = newMilestone;

                Rpc_UpdateMilestoneForClient(netIdentity.connectionToClient, chosenUnitList.unitIndex, chosenUnitList.upgradeMilestone);
            }

            unit.SetUnitSO(chosenUnitList.unit);

            unit.OnDeath += OnUnitDeath;

            unit.traits = new bool[3];
            if (chosenUnitList.hasDamageTrait) unit.traits[0] = true;
            if (chosenUnitList.hasHealthTrait) unit.traits[1] = true;
            if (chosenUnitList.hasSpeedTrait) unit.traits[2] = true;

            chosenUnitList.CurrentAmount++;

            //Master loses energy, because nothing is free in life
            Energy += -chosenUnitList.unit.energyCost;

            playerData.Exp += chosenUnitList.unit.xpGain; //Master gains xp though

            playerData.UnitsPlaced++;
        }
        else
        {
            //If the dictionary does not contain this deployable
            if (!deployableDict.ContainsKey(name))
            {
                //Then add it 
                deployableDict.Add(name, newSpawnable);
                //The client has its own version of this dictionary, just without the gameobject.
                //This dictionary is used to move the deployable, instead of spawning a new one.
                //(Only 1 instance of a deployable is permitted in the world at a time. So when a deployable is instantiated into the world, it will stay there until the match ends.)
            }
        }

        Rpc_SpawnEffect(netIdentity.connectionToClient, pos, !spawningDeployable);

        //Spawn the spawnable on the server
        NetworkServer.Spawn(newSpawnable);
    }

    private void OnUnitDeath(UnitSO unit)
    {
        UnitList chosenUnitList = GetUnitList(unit);

        chosenUnitList.CurrentAmount--;
    }

    [TargetRpc]
    private void Rpc_SetUpgradesAvailable(NetworkConnection target, int unitIndex, int upgradesAvailable)
    {
        UnitList chosenUnitList = unitList[unitIndex];

        chosenUnitList.upgradePanel.UpgradesAvailable = upgradesAvailable;
    }

    [TargetRpc]
    private void Rpc_UpdateMilestoneForClient(NetworkConnection target, int unitIndex, int upgradeMilestone)
    {
        UnitList unit = unitList[unitIndex];

        unit.upgradePanel.SetUpgradeText(upgradeMilestone);
    }

    [Command] 
    void Cmd_MoveDeployable(Vector3 pos, string name)
    {
        foreach(KeyValuePair<string,GameObject> d in deployableDict)
        {
            print(d.Key);
            print(d.Value);
        }

        GameObject deployable = deployableDict[name];

        if (deployable == null)
        {
            Debug.LogError("Deployable had no gameobject reference in dictionary.");
            return;
        }

        //Teleport the deployable to the new position
        deployable.transform.position = pos;

        //Set the deployable's rotation to be random
        deployable.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));

        //OPTIONAL
        //Do an effect here.
        //Like make the deployable rise up from the ground. Or reverse dissolve.
    }
    [Command]
    void Cmd_RefundUnit(GameObject unitToRefund, int refundAmount)
    {
        Energy += refundAmount;

        UnitList unit = GetUnitList(unitToRefund.GetComponent<UnitBase>().UnitSO);
        unit.CurrentAmount--;

        Rpc_SpawnEffect(netIdentity.connectionToClient, unitToRefund.transform.position, true);

        NetworkServer.Destroy(unitToRefund);
    }
    [Command]
    void Cmd_PlayGlobalSound(bool randomPitch)
    {
        Rpc_PlayGlobalSound(randomPitch);
    }
    [ClientRpc]
    void Rpc_PlayGlobalSound(bool randomPitch)
    {
        if (globalAudio.clip == null) return;

        //Assign a random pitch to the audio source
        globalAudio.pitch = randomPitch ? Random.Range(0.95f, 1.05f) : 1;

        //Play the audio
        globalAudio.PlayOneShot(globalAudio.clip);
    }

    #endregion

    private void OnGUI()
    {
        if (test)
        {
            GUI.TextField(new Rect(20, 80, 150, 20), "Master Test ON");
        }
    }
}