﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pathfinding;

[System.Serializable]
public class UnitList
{
    public string name;
    public UnitSO unit;
    [Header("Upgrades")]
    public int level = 0;
    [SerializeField] private int upgradeMilestone = 50;
    public int UpgradeMilestone
    {
        get { return upgradeMilestone; }
        set
        {
            upgradeMilestone = value;
            if (upgradePanel) upgradePanel.SetUpgradeText(value);
        }
    }
    public AnimationCurve upgradeCurve;
    [Space]
    [Space]
    public float healthModifier = 1;
    public int upgradesTillHealthTrait = 5;
    public bool hasHealthTrait = false;
    public int GetHealthStat() { return Mathf.RoundToInt((float)unit.health * healthModifier); }
    [Space]
    public float damageModifier = 1;
    public int upgradesTillDamageTrait = 5;
    public bool hasDamageTrait = false;
    public int GetDamageStat()
    {
        int damageStat = 0;
        switch (unit.unitDamageType)
        {
            case UnitDamageType.Melee:
                damageStat = Mathf.RoundToInt(((unit.melee.meleeDamageMin + unit.melee.meleeDamageMax) / 2) * damageModifier);
                break;
            case UnitDamageType.Ranged:
                damageStat = Mathf.RoundToInt(unit.ranged.rangedDamage * damageModifier);
                break;
            case UnitDamageType.Special:
                damageStat = Mathf.RoundToInt(unit.special.specialDamage * damageModifier);
                break;
        }
        return damageStat;
    }
    [Space]
    public float speedModifier = 1;
    public int upgradesTillSpeedTrait = 5;
    public bool hasSpeedTrait = false;
    public int GetSpeedStat() { return Mathf.RoundToInt((float)unit.movementSpeed * speedModifier); }
    [Space]
    [Space]
    public UnitUpgradePanel upgradePanel;
    public MasterUIGameplayButton unitButton;
    [Header("Other")]
    [Space]
    public int maxAmount;
    public bool unlocked;
    public bool chosen;
    public void Unlock(bool unlock)
    {
        unitButton.Unlock(unlock);
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
    [Header("Master Class")]
    [SerializeField] private UnitMasterSO masterSO = null;
    UnitMasterClass mClass = null;

    [System.Serializable]
    public class Stats
    {
        public int currentEnergy = 50; //Current amount of master energy
        public int maxEnergy = 100; //Maximum amount of energy that can be stored
        public int energyRechargeIncrement = 1; //How much energy recharges per second
        public int maxEnergyIncrement = 20; //Amount to increase max energy
        [Space]
        public int currentXP = 0; //Current amount of master xp
        [Space]
        public int timeUntillNextUpgrade = 50; //When does the next upgrade decision become available
    }
    [Space]
    public Stats stats;
    private int Energy
    {
        get { return stats.currentEnergy; }
        set
        {
            stats.currentEnergy = Mathf.Clamp(value, 0, stats.maxEnergy);
            UpdateEnergyUI();
        }
    }
    private int CurrentXP
    {
        get { return stats.currentXP; }
        set
        {
            stats.currentXP = value;

            //XP UI
            UI.xpText.text = CurrentXP.ToString() + "XP";
            UI.upgradeMenuXpText.text = "Current XP: " + CurrentXP.ToString();

            foreach (UnitList unitListItem in unitList)
            {
                unitListItem.upgradePanel.UnlockCheck(CurrentXP);
            }

            /*
            //Check if there is an upgrade or unlock available
            for (int i = 0; i < uiGameplayButtons.Count; i++)
            {
                int unitRange = unitList.Count;

                if (i < unitRange)
                {
                    UnitList unit = unitList[i];
                    UnitSO unitSO = unit.unit;
                    unit.
                }
                else
                {
                    DeployableList deployable = deployableList[i - unitRange];

                    if (!deployable.unlocked)
                    {

                    }
                    continue;
                }
            }
            */
        }
    }
    private int GetXPToUpgrade(int baseUpgrade, int level) => baseUpgrade + Mathf.RoundToInt((float)baseUpgrade * (0.2f * (level - 1)));


    [Header("Units")]
    [SerializeField] private List<UnitList> unitList = new List<UnitList>();
    public UnitList GetUnitList(int index) => unitList[index];
    [Space]
    [SerializeField, SyncVar] private int chosenSpawnableIndex = 0;
    [SerializeField] private bool hasChosenASpawnable = false;
    [SerializeField] private UnitBase selectedUnit = null;

    [Header("Deployables")]
    [SerializeField] private List<DeployableList> deployableList = new List<DeployableList>();
    private Dictionary<string, GameObject> deployableDict = new Dictionary<string, GameObject>();

    [Header("Spawning")]
    [SerializeField] private LayerMask playerLayer = 1 << 13;
    [SerializeField] private int spawnCheckRadius = 200;
    [SerializeField] private int minimumSpawnRadius = 5;
    private const string unitResourcesLocation = "Spawnables/Unit - Master/Units/";
    private const string deployableResourcesLocation = "Spawnables/Unit - Master/Deployables/";

    [System.Serializable]
    public class UserInterface
    {
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
        public GameObject RechargeRateButton = null;
        public GameObject MaxEnergyButton = null;
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

        [Header("Upgrade System UI")]
        public GameObject upgradeMenu;
        public Transform upgradeMenuContainer;
        public GameObject unitUpgradePanel;
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

    [Header("Other")]
    [SerializeField] private LayerMask ignoreOnRaycast = 1 << 2;
    [SerializeField] private LayerMask ignoreOnViewCheck = 1 << 9;
    [Space]
    [SerializeField] private Light tintLight = null;

    [Header("Particles and Effects")]
    public ParticleSystem spawnSmokeEffect;
    private AudioSource spawnSmokeAudio;
    private AudioSource globalAudio;

    [Header("Network")]
    [SerializeField] private GameObject[] disableForOthers = null;

    [Header("Debug")]
    [SerializeField] private bool test = false;
    [SerializeField] private UnitMasterSO testSO = null;

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
            SetMasterClass(testSO);
        }
    }

    public void Initialize()
    {
        name += $" ({masterSO.masterName})";
        
        SetMasterUnits();
        SetMasterDeployables();

        globalAudio = GetComponent<AudioSource>();

        if (!hasAuthority) return;

        //AddBinds();

        InitializeUnitButtons();
        InitializeDeployableButtons();

        //Setup the different camera modes
        flying.flyingController = flying.flyingController.GetComponent<UnitMaster_FlyingController>();
        flying.flyingController.master = this; //Assign the reference.
        inTopdownView = false; //This bool is simply to stop a raycast from being performed.
            //The bool will be set to true in this function.
        SwitchCamera(true);

        //Default starting energy stats
        stats.currentEnergy = masterSO.startEnergy;
        stats.maxEnergy = masterSO.startMaxEnergy;
        stats.energyRechargeIncrement = masterSO.energyRechargeIncrement;
        stats.maxEnergyIncrement = masterSO.maxEnergyUpgradeIncrement;
        stats.timeUntillNextUpgrade = masterSO.energyUpgradeInterval;

        //Energy UI visuals
        UI.energyFillImage.color = masterSO.energyColor;
        UI.energyUseFillImage.color = masterSO.energyUseColor;

        tintLight.color = masterSO.topdownLightColor;
        UI.screenTint.color = masterSO.screenTintColor;

        //Update the Energy UI
        UpdateEnergyUI();
        UpdateEnergyUseUI(0);

        UI.upgradeMenu.SetActive(false);

        //Other master visuals

        //Change the Flying controller's marker visuals (Mesh and colour)
        flying.flyingController.ChangeMarker(masterSO.markerMesh, masterSO.markerColor);
        EnableFlyingMarker(false); //Disable the marker on start. Default view mode is Topdown.

        //-----------------------

        ActivateUpgradeDecisions(false);

        //Coroutines

        StartCoroutine(EnergyCoroutine());

        StartCoroutine(UpgradeCoroutine(stats.timeUntillNextUpgrade));

        //Misc
        spawnSmokeAudio = spawnSmokeEffect.GetComponent<AudioSource>();
        spawnSmokeAudio.clip = masterSO.spawnSound;
                
        globalAudio.clip = masterSO.globalSound;

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
        JODSInput.Controls.Master.LMB.performed += ctx => StartContinuousSpawn();
        JODSInput.Controls.Master.LMB.canceled += ctx => StopContinuousSpawn();

        // Right Mouse Input
        JODSInput.Controls.Master.RMB.performed += ctx => RMB();

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
        JODSInput.Controls.Master.OpenUpgradeMenu.performed += ctx => OpenUpgradeMenu();

        //Menu Input
        PlayerManager.Instance.onMenuOpened += OnMenuEnabled;
        PlayerManager.Instance.onMenuClosed += OnMenuDisabled;
    }

    private void RemoveBinds()
    {
        // Left Mouse Input
        JODSInput.Controls.Master.LMB.performed -= ctx => LMB();
        JODSInput.Controls.Master.LMB.performed -= ctx => StartContinuousSpawn();
        JODSInput.Controls.Master.LMB.canceled -= ctx => StopContinuousSpawn();

        // Right Mouse Input
        JODSInput.Controls.Master.RMB.performed -= ctx => RMB();

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
        JODSInput.Controls.Master.OpenUpgradeMenu.performed -= ctx => OpenUpgradeMenu();


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

    [Header("Debugging")]
    [SerializeField] private bool shift = false;
    [SerializeField] private bool ctrl = false;
    [SerializeField] private bool alt = false;
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
    private Coroutine continuousSpawnCo;
    private bool continuousSpawnBool;
    private IEnumerator IEContinuousSpawn()
    {
        continuousSpawnBool = true;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            TryToSpawnUnit();
        }
    }
    private void StartContinuousSpawn()
    {
        if (alt || shift || ctrl) return;
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        continuousSpawnCo = StartCoroutine(IEContinuousSpawn());
    }
    private void StopContinuousSpawn()
    {
        if (!continuousSpawnBool) return;
        continuousSpawnBool = false;
        StopCoroutine(continuousSpawnCo);
    }
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
    public void UpgradeEnergy(bool rate)
    {
        //Deactivate the decisions
        ActivateUpgradeDecisions(false);

        //Play a sound
        Cmd_PlayGlobalSound(true);

        //Start the coroutine again
        StartCoroutine(UpgradeCoroutine(stats.timeUntillNextUpgrade));

        //If player chose to upgrade the recharge rate
        if (rate)
        {
            //Increase the increment
            stats.energyRechargeIncrement += 1;
        }

        //If player chose to upgrade the max amount of energy
        else
        {
            //Increase the max amount of energy
            stats.maxEnergy += stats.maxEnergyIncrement;

            //Set the UI slider's max value
            UI.energySlider.maxValue = stats.maxEnergy;

            UI.energyUseSlider.maxValue = stats.maxEnergy;

            UpdateEnergyUI();
        }

        //Update scoreboard stat
        Cmd_UpdateScore(1, PlayerDataStat.TotalUpgrades);
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
            Energy += stats.energyRechargeIncrement;
        }
    }

    private IEnumerator UpgradeCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        //Activate the upgrade decisions
        ActivateUpgradeDecisions(true);
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
            b.Unlock(u.unlocked);
        }
    }

    private void InitializeDeployableButtons()
    {
        //This is used to assign the indexes of these buttons
        int referenceIndex = uiGameplayButtons.Count;

        for (int i = 0; i < deployableList.Count; i++)
        {
            //Check if the index has a unit assigned, if not continue to next index.
            if (!deployableList[i].deployable)
            {
                Debug.LogError($"Deployable with index {i} has no deployable assigned!");
                continue; //Go to the next iteration
            }

            //Reference
            DeployableList d = deployableList[i];

            //Instantiate unit button prefab
            GameObject button = Instantiate(UI.deployableButtonPrefab, UI.deployableButtonContainer);
            MasterUIGameplayButton b = button.GetComponent<MasterUIGameplayButton>();

            d.deployableButton = b;

            //Add this button to the list
            uiGameplayButtons.Add(b);

            //Details
            b.SetDetails(d.deployable.deployableSprite, d.deployable.name, d.deployable.description);

            //Add an event to call the function ChooseDeployable whenever the button is pressed
            button.GetComponent<Button>().onClick.AddListener(delegate { ChooseDeployable(d); });

            //Start the unit button as Unlocked or Locked
            b.Unlock(d.unlocked);
        }
    }

    private void UpdateEnergyUI()
    {
        //Energy UI
        UI.energyText.text = Energy.ToString() + "/" + stats.maxEnergy;
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

    private void ActivateUpgradeDecisions(bool enable)
    {
        UI.MaxEnergyButton.SetActive(enable);
        UI.RechargeRateButton.SetActive(enable);
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

    public void OpenUpgradeMenu()
    {
        bool active = !UI.upgradeMenu.activeSelf;
        UI.upgradeMenu.SetActive(active);
        UI.inGameUI.SetActive(!active);

    }

    #endregion

    #region Particles and Effects Functions

    private void SpawnEffect(Vector3 point, bool smoke = true)
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
        chosenSpawnableIndex = unit.buttonIndex;

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

        chosenSpawnableIndex = deployable.buttonIndex;
        hasChosenASpawnable = true;

        UnitButtonChooseUI(true);

        if (!inTopdownView)
        {
            //This will enable the marker for the flying camera, which is mostly a visual aid
            EnableFlyingMarker(true);
        }
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
                    if (unitList[chosenSpawnableIndex].unit.energyCost <= stats.currentEnergy)
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
        (float, float, float) spawnModifiers = (0,0,0);

        //When spawning a deployable
        if (spawningADeployable)
        {
            int referenceIndex = chosenSpawnableIndex - unitList.Count;

            DeployableList listItem = deployableList[referenceIndex];
            DeployableSO chosenDeployable = listItem.deployable;
            spawnName = chosenDeployable.deployablePrefab.name;

            uiGameplayButtons[chosenSpawnableIndex].StartCooldown(listItem.deployableCooldown);
            StartCoroutine(DeployableCooldown(listItem));

            SpawnEffect(hit.point, false);

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

            //Spawn a smoke effect to hide the instantiation of the unit.
            SpawnEffect(hit.point);

            //If the spawn location meets the requirements, then spawn the currently selected type of unit.

            //A random unit from the chosen unit's prefab list gets picked, and the name gets sent to the server, which then spawns the unit.
            //This is because there can be multiple variations of one unit.
            //spawnLevel = unitList[chosenSpawnableIndex].level;
            spawnName = chosenUnit.unitPrefab.name;

            //Master loses energy, because nothing is free in life
            Energy += -chosenUnit.energyCost;

            CurrentXP += chosenUnit.xpGain; //Master gains xp though

            chosenUnitList.UpgradeMilestone = (int)Mathf.Clamp(chosenUnitList.UpgradeMilestone -= 1, 0, Mathf.Infinity);

            if (chosenUnitList.UpgradeMilestone <= 0)
            {
                chosenUnitList.upgradePanel.EnableUpgrades(true);
            }

            spawnModifiers = (chosenUnitList.healthModifier, chosenUnitList.damageModifier, chosenUnitList.speedModifier);

            Cmd_UpdateScore(1, PlayerDataStat.UnitsPlaced);
        }

        Cmd_Spawn(hit.point, spawnName, spawningADeployable, spawnModifiers);
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
        Cmd_RefundUnit(unitToRefund.gameObject);

        //Spawn a smoke effect to hide the removal of the unit.
        SpawnEffect(unitToRefund.transform.position);

        Energy += refundAmount;
    }
    #endregion

    #region Upgrade & Unlock
    public int UpgradeUnit(int unitIndex, int upgradePath, float upgradeAmount)
    {
        //Reference
        UnitList unit = unitList[unitIndex];

        int upgradeToReturn = 0;

        switch (upgradePath)
        {
            //Health upgrade
            case 0:
                unit.upgradesTillHealthTrait--;
                upgradeToReturn = unit.upgradesTillHealthTrait;
                unit.healthModifier += upgradeAmount;
                break;
            //Damage upgrade
            case 1:
                unit.upgradesTillDamageTrait--;
                upgradeToReturn = unit.upgradesTillDamageTrait;
                unit.damageModifier += upgradeAmount;
                break;
            //Speed upgrade
            case 2:
                unit.upgradesTillSpeedTrait--;
                upgradeToReturn = unit.upgradesTillSpeedTrait;
                unit.speedModifier += upgradeAmount;
                break;
        }

        unit.level++;

        unit.UpgradeMilestone = 50; // FIX THIS

        //Play spooky sound
        Cmd_PlayGlobalSound(true);

        //Update scoreboard stat
        Cmd_UpdateScore(1, PlayerDataStat.TotalUnitUpgrades);

        return upgradeToReturn;
    }

    public void UnlockNew(UnitList unit = null, DeployableList deployable = null)
    {
        if (deployable != null)
        {
            deployable.Unlock(true);

            deployable.unlocked = true;

            CurrentXP += -deployable.deployable.xpToUnlock;
        }
        else if (unit != null)
        {
            unit.Unlock(true);

            //Unlock the unit
            unit.unlocked = true;

            //Decrease xp by amount required to unlock the unit
            CurrentXP += -unit.unit.xpToUnlock;
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
                if (unit.select.canSelect && !unit.isDead)
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

            Raycast(out bool didHit, out RaycastHit hit);
            if (didHit)
            {
                StartCoroutine(IETakingControl(hit.transform));
            }
        }
    }
    bool takingControlBool = false;
    private IEnumerator IETakingControl(Transform targetToControl)
    {
        takingControlBool = true;

        JODSInput.Controls.Master.Disable();

        Camera currentCamera = GetCurrentCamera;
        if (!inTopdownView)
        {
            GameObject zoomCamera = Instantiate(currentCamera.gameObject, currentCamera.transform.position, currentCamera.transform.rotation);
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
                currentCamera.transform.position = Vector3.Lerp(currentCamera.transform.position, targetToControl.position, Time.deltaTime);
            }

            elapsedTime += Time.deltaTime;

            fade.a += Time.deltaTime;
            UI.fadeImage.color = fade;
            yield return null;
        }
        if (!inTopdownView) Destroy(currentCamera);
        yield return new WaitForSeconds(1f);

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

            distance = 100;
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
            Vector3 pPos = new Vector3(survivor.transform.position.x, survivor.transform.position.y + 1.75f, survivor.transform.position.z);
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
                    Debug.DrawRay(pos, pPos, angle > 60 ? Color.green : Color.red, 10f, false);

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
            u.UpgradeMilestone = u.unit.upgrades.unitsToPlace;
            u.upgradeCurve = u.unit.upgrades.upgradeCurve;
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
    void Cmd_UpdateScore(int amount, PlayerDataStat stat)
    {
        GamemodeBase.Instance.Svr_ModifyStat(GetComponent<NetworkIdentity>().netId, amount, stat);
    }

    [Command]
    void Cmd_Spawn(Vector3 pos, string name, bool deployable, (float, float, float) modifiers)
    {
        //Set the positions y value to be a bit higher, so that the spawnable doesn't spawn inside the floor
        pos = new Vector3(pos.x, pos.y + 0.1f, pos.z);

        //If the type is 0, then spawn a deployable, not a unit
        bool spawningDeployable = deployable;

        string resourceLocation = spawningDeployable ? deployableResourcesLocation : unitResourcesLocation;

        //Get the spawnable to spawn
        GameObject spawnableToSpawn = (GameObject)Resources.Load(resourceLocation + $"{name}");

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

            unit.SetUnitSO(unitList[chosenSpawnableIndex].unit);
            unit.Svr_IncreaseStats(modifiers);
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

        //Spawn the spawnable on the server
        NetworkServer.Spawn(newSpawnable);
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
    void Cmd_RefundUnit(GameObject unitToRefund)
    {
        NetworkServer.Destroy(unitToRefund);
    }
    [Command]
    void Cmd_PlayGlobalSound(bool randomPitch)
    {
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