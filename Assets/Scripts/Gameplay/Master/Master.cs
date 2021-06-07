using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using System.Reflection;
using UnityEngine.EventSystems;
using UnityEngine.AI;

[System.Serializable]
public class UnitList
{
    public string name;
    public UnitSO unit;
    public int level = 1;
    public int maxAmount;
    public bool unlocked;
    public bool chosen;
    [Space]
    public int unitIndex = 0;
}
[RequireComponent(typeof(AudioSource))]
public class Master : NetworkBehaviour
{
    #region Fields
    [Header("Master Class")]
    [SerializeField] private MasterSO masterSO = null;
    MasterClass mClass = null;

    [System.Serializable]
    public class Stats
    {
        public int currentEnergy = 50; //Current amount of master energy
        public int maxEnergy = 100; //Maximum amount of energy that can be stored
        public int energyRechargeIncrement = 1; //How much energy recharges per second
        public int maxEnergyIncrement = 20; //Amount to increase max energy
        [Space]
        public int currentXp = 0; //Current amount of master xp
        [Space]
        public int timeUntillNextUpgrade = 50; //When does the next upgrade decision become available
    }
    [Space]
    public Stats stats;

    [Header("Units")]
    [SerializeField] private List<UnitList> unitList = new List<UnitList>();
    [Space]
    [SerializeField] private int chosenUnitIndex = 0;
    [SerializeField] private bool hasChosenAUnit = false;
    [SerializeField] private UnitBase selectedUnit = null;
    [SerializeField] private GameObject unitDestinationMarker = null;

    [Header("Spawning")]
    [SerializeField] private LayerMask playerLayer = 1 << 15;
    [SerializeField] private int spawnCheckRadius = 20;
    [SerializeField] private int minimumSpawnRadius = 5;

    [System.Serializable]
    public class UserInterface
    {
        public Text energyText = null;
        public Slider energySlider = null;
        public Image energyFillImage;
        public Slider energyUseSlider = null;
        public Image energyUseFillImage;
        [Space]
        public Text xpText = null;
        public Text spawnText = null;
        [Space]
        public GameObject RechargeRateButton = null;
        public GameObject MaxEnergyButton = null;
        [Space]
        public GameObject unitButtonPrefab;
        public Transform unitButtonContainer;
        [Space]
        public Image screenTint;
    }
    [Space]
    public UserInterface UI;
    private List<UnitButton> unitButtons = new List<UnitButton>();

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
        public Camera camera = null;
        public Master_FlyingController flyingController;
    }
    [Space]
    public FlyingMaster flying;

    [Space]
    [SerializeField] private bool inTopdownView = true;

    [Header("Other")]
    [SerializeField] private LayerMask ignoreOnRaycast = 1 << 2;
    [Space]
    [SerializeField] private Light tintLight = null;

    [Header("Particles and Effects")]
    public ParticleSystem spawnSmokeEffect;
    private AudioSource spawnSmokeAudio;
    private AudioSource globalAudio;

    [Header("Network")]
    [SerializeField] private GameObject[] disableForOthers;

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
    }

    public override void OnStartAuthority()
    {
        //Add (MASTER to end of name)
        name += $" ({masterSO.masterName})";
        print("OnStartAuthority");


        SetMasterUnits();
        InitializeUnitButtons();

        //Do basic startup for master, if player has authority (If the player is the master)
        if (hasAuthority)
        {
            //Setup the different camera modes
            flying.flyingController = flying.camera.GetComponent<Master_FlyingController>();
            flying.flyingController.master = this; //Assign the reference.
            inTopdownView = false; //This bool is simply to stop a raycast from being performed.
             //The bool will be set to true in this function.
            SwitchCamera(true);

            //Default starting energy stats
            stats.currentEnergy = 50;
            stats.energyRechargeIncrement = 1;

            //Energy UI visuals
            UI.energyFillImage.color = masterSO.energyColor;
            UI.energyUseFillImage.color = masterSO.energyUseColor;

            tintLight.color = masterSO.topdownLightColor;
            UI.screenTint.color = masterSO.screenTintColor;

            //Update the Energy UI
            UpdateEnergyUI();
            UpdateEnergyUseUI(0);

            //Other master visuals

            //Change the Flying controller's marker visuals (Mesh and colour)
            flying.flyingController.ChangeMarker(masterSO.markerMesh, masterSO.markerColor);
            EnableFlyingMarker(false); //Disable the marker on start. Default view mode is Topdown.

            //Change Select's position marker visuals (Mesh and colour)
            Material markerMat = unitDestinationMarker.GetComponent<MeshRenderer>().sharedMaterial;
            markerMat.color = masterSO.selectPositionMarkerColor;
            markerMat.SetColor("_EmissionColor", masterSO.selectPositionMarkerColor);
            unitDestinationMarker.GetComponent<MeshFilter>().mesh = masterSO.selectPositionMarkerMesh;
            unitDestinationMarker.transform.SetParent(null);

            SetUnitDestinationMarker(false);

            //-----------------------

            ActivateUpgradeDecisions(false);

            //Coroutines

            StartCoroutine(EnergyCoroutine());

            StartCoroutine(UpgradeCoroutine(stats.timeUntillNextUpgrade));

            //Misc
            spawnSmokeAudio = spawnSmokeEffect.GetComponent<AudioSource>();
            spawnSmokeAudio.clip = masterSO.spawnSound;

            globalAudio = GetComponent<AudioSource>();
            globalAudio.clip = masterSO.globalSound;

            //Attach the master's custom script to the gameobject.
            System.Type masterType = System.Type.GetType(masterSO.masterClass.name + ",Assembly-CSharp");
            mClass = (MasterClass)gameObject.AddComponent(masterType);
            mClass.UseSpecial();
        }
    }

    private void OnEnable()
    {
        // Left Mouse Input
        JODSInput.Controls.Master.LMB.performed += ctx => LMB();

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

        // Unit Select Input
        JODSInput.Controls.Master.UnitSelecting.performed += ctx => ChooseUnit(Mathf.FloorToInt(ctx.ReadValue<float>() - 1));

        //Camera Change Input
        JODSInput.Controls.Master.ChangeCamera.performed += ctx => SwitchCamera(!inTopdownView);
    }

    private void OnDisable()
    {
        // Left Mouse Input
        JODSInput.Controls.Master.LMB.performed -= ctx => LMB();

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
        JODSInput.Controls.Master.UnitSelecting.performed -= ctx => ChooseUnit(Mathf.FloorToInt(ctx.ReadValue<float>() - 1));

        //Camera Change Input
        JODSInput.Controls.Master.ChangeCamera.performed -= ctx => SwitchCamera(!inTopdownView);
    }

    private void OnValidate()
    {
        if (masterSO)
        {
            SetMasterUnits();
        }
        //If there is no master class selected, then clear the list of units
        else if (unitList.Count != 0) unitList.Clear();
    }

    #endregion

    #region Gameplay Functions

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

    public void SetMasterClass(MasterSO mClass)
    {
        masterSO = mClass;
    }

    #region Normal Mouse
    private void LMB()
    {
        if (alt) return;
        if (EventSystem.current.IsPointerOverGameObject()) { return; }
        if (shift && !ctrl) { Shift_LMB(); return; }
        else if (ctrl && !shift) { Ctrl_LMB(); return; }

        //Somehow check if master clicks on a unit button, if so do not spawn anything.

        TryToSpawnUnit();
    }
    private void RMB()
    {
        if (alt) return;
        if (EventSystem.current.IsPointerOverGameObject()) { return; }
        if (shift && !ctrl) { Shift_RMB(); return; }
        else if (ctrl && !shift) { Ctrl_RMB(); return; }

        //Unchoose the current unit type
        UnchooseUnit();
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
        TryToSpawnUnit();
    }
    private void Ctrl_RMB()
    {
        TryToRefundUnit();
    }
    #endregion
    public void UpgradeEnergy(bool rate)
    {
        //Deactivate the decisions
        ActivateUpgradeDecisions(false);

        //Play a sound
        CmdPlayGlobalSound(true);

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
            IncreaseEnergy(stats.energyRechargeIncrement);
        }
    }

    private IEnumerator UpgradeCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        //Activate the upgrade decisions
        ActivateUpgradeDecisions(true);
    }

    private Coroutine selectCo;
    private bool selectCoroutineActive;
    private IEnumerator SelectCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            SetUnitDestinationMarker(!IsUnitCloseToDestination());
        }
    }

    #endregion

    #region UI Functions

    private void InitializeUnitButtons()
    {
        print("InitializeUnitButtons");
        for (int i = 0; i < unitList.Count; i++)
        {
            //Check if the index has a unit assigned, if not continue to next index.
            if (!unitList[i].unit)
            {
                Debug.LogError($"Unit with index {unitList[i].unitIndex} has no unit assigned!");
                continue; //Go to the next iteration
            }

            //Reference
            UnitList u = unitList[i];

            //Instantiate unit button prefab
            GameObject button = Instantiate(UI.unitButtonPrefab, UI.unitButtonContainer);
            UnitButton b = button.GetComponent<UnitButton>();

            //Add this button to the list
            unitButtons.Add(b);

            //Set button values

            //Image
            if (u.unit.unitSprite) b.SetImage(u.unit.unitSprite);
            else Debug.LogError($"{u.unit.name} has no unit sprite assigned!");

            //Level
            b.SetUnitLevel(u.level);

            //Details
            b.SetDetails(u.unit.description, u.unit.powerStat, u.unit.healthStat);

            //Index
            b.UnitIndex = i;

            //Add an event to call the function ChooseUnit whenever the button is pressed
            button.GetComponent<Button>().onClick.AddListener(delegate { ChooseUnit(b.UnitIndex); });

            //Add events to call the functions whenever the buttons are pressed
            b.upgradeButton.GetComponent<Button>().onClick.AddListener(delegate { UpgradeUnit(b.UnitIndex); });
            b.unlockButton.GetComponent<Button>().onClick.AddListener(delegate { UnlockNewUnit(b.UnitIndex); });

            //Start the unit button as Unlocked or Locked
            b.Unlock(u.unlocked);
        }
    }

    private void UpdateEnergyUI()
    {
        //Energy UI
        UI.energyText.text = stats.currentEnergy.ToString() + "/" + stats.maxEnergy;
        UI.energySlider.value = stats.currentEnergy;
    }

    private void UpdateEnergyUseUI(int use)
    {
        UI.energyUseSlider.value = use;
    }

    private void UnitButtonChooseUI(bool choose)
    {
        unitButtons[chosenUnitIndex].Choose(choose);
    }

    private void ActivateUpgradeDecisions(bool enable)
    {
        UI.MaxEnergyButton.SetActive(enable);
        UI.RechargeRateButton.SetActive(enable);
    }

    private void SpawnTextReset()
    {
        UI.spawnText.text = "";
        UI.spawnText.gameObject.SetActive(false);
    }

    private void SetSpawnText(string newText)
    {
        UI.spawnText.text = newText;
        UI.spawnText.gameObject.SetActive(true);
        Invoke(nameof(SpawnTextReset), 1f);
    }

    #endregion

    #region Particles and Effects Functions

    private void SmokeEffect(Vector3 point)
    {
        //Spawn audio
        spawnSmokeAudio.pitch = Random.Range(0.9f, 1.1f);
        spawnSmokeAudio.PlayOneShot(spawnSmokeAudio.clip);

        //Smoke particles
        spawnSmokeEffect.transform.position = point;
        spawnSmokeEffect.transform.SetParent(null);
        spawnSmokeEffect.Emit(50);
    }

    #endregion

    #region Other

    private void IncreaseXp(int amount)
    {
        stats.currentXp += amount;

        //XP UI
        UI.xpText.text = stats.currentXp.ToString() + "xp";

        //Check if there is an upgrade or unlock available
        for (int i = 0; i < unitList.Count; i++)
        {
            UnitList unit = unitList[i];

            //If the unit is unlocked
            if (unit.unlocked)
            {
                //If player has enough xp to upgrade it, show the upgrade button
                if (unit.unit.xpToUpgrade <= stats.currentXp) unitButtons[i].ShowUpgradeButton(true);
                else unitButtons[i].ShowUpgradeButton(false);
                continue;
            }
            else
            {
                //Else, if the player has enough xp to unlock it, show the unlock button
                if (unit.unit.xpToUnlock <= stats.currentXp) unitButtons[i].ShowUnlockButton(true);
                else unitButtons[i].ShowUnlockButton(false);
                continue;
            }
        }
    }
    private void IncreaseEnergy(int amount)
    {
        stats.currentEnergy = Mathf.Clamp(stats.currentEnergy += amount, 0, stats.maxEnergy);
        UpdateEnergyUI();
    }

    private void SetUnitDestinationMarker(bool enable)
    {
        if (enable)
        {
            if (selectedUnit)
            {
                unitDestinationMarker.SetActive(true);
                unitDestinationMarker.transform.Rotate(0, Random.Range(0, 360),0);
                unitDestinationMarker.transform.position = selectedUnit.GetComponent<NavMeshAgent>().destination;
            }
            else unitDestinationMarker.SetActive(false);
        }
        else unitDestinationMarker.SetActive(false);
    }
    private bool IsUnitCloseToDestination()
    {
        if (selectedUnit)
        {
            NavMeshAgent nav = selectedUnit.GetComponent<NavMeshAgent>();
            return Vector3.Distance(
                nav.transform.position,
                nav.destination
                ) < nav.stoppingDistance;
        }
        else return false;
    }

    private void EnableFlyingMarker(bool enable)
    {
        flying.flyingController.ShowMarker(enable);
    }

    #endregion

    #region Unit Functions

    private void SetMasterUnits()
    {
        print("SetMasterUnits");
        //This will assign all the units that the master class has, to the master. (Essentially this remakes the list)
        if (UnitInitialization() || unitList.Count == 0) SetMasterUnitsInEditor(); //Only if the current list is wrong though, which this bool method will check

        if (unitList.Count == 0) return; //If there are no units, there is no need to continue.

        SetMasterUnitValues();
    }

    #region Choosing
    private void ChooseUnit(int indexNum)
    {
        if (indexNum >= unitList.Count)
        {
            Debug.Log("There is no unit with the number " + indexNum);
            UnchooseUnit();
            return;
        }

        //Reference
        UnitList unit = unitList[indexNum];

        //If the unit is unlocked
        if (!unit.unlocked) return;

        //If the master has already chosen a unit, unchoose that unit
        unit.chosen = false;
        if (hasChosenAUnit) UnitButtonChooseUI(false);

        //Then choose the new unit
        chosenUnitIndex = indexNum;
        hasChosenAUnit = true;
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

    private void UnchooseUnit()
    {
        hasChosenAUnit = false;

        //Change the UI
        UnitButtonChooseUI(false);
        UpdateEnergyUseUI(0);

        if (!inTopdownView)
        {
            //This will disable the marker for the flying camera
            EnableFlyingMarker(false);
        }
    }
    #endregion

    #region Spawning & Refunding
    void TryToSpawnUnit()
    {
        //Have I chosen a unit to spawn?
        if (!hasChosenAUnit) return; //if not, then don't proceed

        Raycast(out bool didHit, out RaycastHit hit);

        if (didHit)
        {
            //Did the raycast hit an area where units can spawn? 
            //(The "Ground" tag is currently the only surface where units can spawn)
            if (hit.collider.CompareTag("Ground"))
            {
                //Do I have enough energy to spawn the chosen unit?
                if (unitList[chosenUnitIndex].unit.energyCost <= stats.currentEnergy)
                {
                    //If yes, spawn the unit at the location
                    SpawnUnit(hit);
                }
                else
                {
                    SetSpawnText("Not enough energy");
                }
            }
            else
            {
                SetSpawnText(UI.spawnText.text = "Cannot spawn unit here");
            }
        }
    }

    void SpawnUnit(RaycastHit hit)
    {
        //Reference
        UnitSO chosenUnit = unitList[chosenUnitIndex].unit;

        //Check if the spawn location meets the requirements.
        if (!ViewCheck(hit.point, true)) return;

        //Spawn a smoke effect to hide the instantiation of the unit.
        SmokeEffect(hit.point);

        //If the spawn location meets the requirements, then spawn the currently selected unit.

        //A random unit from the chosen unit's prefab list gets picked, and the name gets sent to the server, which then spawns the unit.
        //This is because there can be multiple variations of one unit.
        CmdSpawnMyUnit(hit.point,
            chosenUnit.unitPrefab.name,
            unitList[chosenUnitIndex].level);

        //Master loses energy, because nothing is free in life
        IncreaseEnergy(-chosenUnit.energyCost);
        UpdateEnergyUI();//Update UI
        IncreaseXp(chosenUnit.xpGain); //Master gains xp though
    }

    void TryToRefundUnit()
    {
        Raycast(out bool didHit, out RaycastHit hit);

        if (didHit)
        {
            //If I hit a unit, try and refund that unit
            if (hit.collider.TryGetComponent(out UnitBase unit))
            {
                int refundAmount = unit.Refund();
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
        CmdRefundUnit(unitToRefund.gameObject);

        //Spawn a smoke effect to hide the removal of the unit.
        SmokeEffect(unitToRefund.transform.position);

        IncreaseEnergy(refundAmount);
        UpdateEnergyUI();//Update UI
    }
    #endregion

    #region Upgrade & Unlock
    public void UpgradeUnit(int which)
    {
        //Reference
        UnitList unit = unitList[which];

        //If player does NOT have enough xp (Which shouldn't be possible), return.
        if (stats.currentXp < unit.unit.xpToUpgrade) return;

        //Increase the unit's level
        unit.level += 1;
        unitButtons[which].SetUnitLevel(unit.level);

        //Decrease xp by amount required to upgrade the unit
        IncreaseXp(-unit.unit.xpToUpgrade);

        //Play spooky sound
        CmdPlayGlobalSound(true);
    }

    public void UnlockNewUnit(int which)
    {
        //Reference
        UnitList unit = unitList[which];

        //If player does NOT have enough xp (Which shouldn't be possible), return.
        if (stats.currentXp < unit.unit.xpToUnlock) return;

        //Unlock the unit
        unit.unlocked = true;

        //Unlock the unit on the button
        unitButtons[which].Unlock(true);

        //Decrease xp by amount required to upgrade the unit
        IncreaseXp(-unit.unit.xpToUnlock);

        //Play spooky sound
        CmdPlayGlobalSound(true);
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
                if (selectedUnit != null)
                {
                    DeselectUnit();
                }
            }
        }
    }
    private void SelectUnit(UnitBase unit)
    {
        //If a unit is already selected, deselect it before selecting the new one.
        if (selectedUnit)
        {
            DeselectUnit();
        }

        //Select the unit
        selectedUnit = unit;
        selectedUnit.Select(masterSO.unitSelectColor);

        StartSelectCoroutine();
    }
    private void DeselectUnit()
    {
        if (selectCoroutineActive)
        {
            StopCoroutine(selectCo);
            selectCoroutineActive = false;
        }
        SetUnitDestinationMarker(false);

        selectedUnit.Deselect();
        selectedUnit = null;
    }
    private void TryToCommandUnit()
    {
        //If I don't have a unit currently selected, then return
        if (!selectedUnit) return;

        Raycast(out bool didHit, out RaycastHit hit);

        if (didHit)
        {
            print(hit.transform.name);
            if (hit.transform.TryGetComponent(out LiveEntity destructible))
            {
                print("wall");
                selectedUnit.AcquireTarget(hit.transform, true, true, true);
            }

            //Command my selected unit to move to the location
            selectedUnit.MoveToLocation(hit.point);

            StartSelectCoroutine();
        }
    }
    private void StartSelectCoroutine()
    {
        if (!IsUnitCloseToDestination())
        {
            SetUnitDestinationMarker(true);
        }

        if (!selectCoroutineActive) 
        {
            selectCo = StartCoroutine(SelectCoroutine());
            selectCoroutineActive = true; 
        }
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
            ray = flying.camera.ScreenPointToRay(Input.mousePosition);

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
            Vector3 pPos = new Vector3(survivor.transform.position.x, pos.y, survivor.transform.position.z);
            Vector3 dir = pPos - pos;
            //Do a raycast, to check if it hits anything on the way to the survivor.
            if (Physics.Raycast(pos, dir, out RaycastHit newhit, 100f))
            {
                //If it hits something, then check if it is a player or not.
                if (newhit.collider.CompareTag("Player"))
                {
                    //If it does hit the survivor, then first check if it is within the survivor's view angle.
                    dir = pos - pPos;
                    float angle = Vector3.Angle(dir, survivor.transform.forward);
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
                    //Then check if it is within the minimum distance to spawn away from a survivor.
                    if (Vector3.Distance(pos, newhit.collider.transform.position) <= minimumSpawnRadius)
                    {
                        //If it is within the minimum spawn distance, then it cannot spawn a unit.
                        SetSpawnText( spawn ?
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

    private void SetMasterUnitValues()
    {
        //Set the names and indexes of each unit
        for (int i = 0; i < unitList.Count; i++)
        {
            UnitList u = unitList[i];
            //Set name of unit
            if (u.unit) u.name = u.unit.name;
            //Set index of unit
            u.unitIndex = i;
            //Set this unit to be unlocked, if it is a starter unit
            u.unlocked = u.unit.starterUnit;
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
                flying.camera.transform.position = new Vector3(hit.point.x,
                    hit.point.y + 3,
                    hit.point.z);
            }
        }

        //Switch the bool
        inTopdownView = top;

        //Deactivate / Activate the cameras based on the bool
        topdown.camera.gameObject.SetActive(top);
        flying.camera.gameObject.SetActive(!top);

        //Change the cursor settings to be either visible or not visible
        Cursor.lockState = top ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = top; //Visible in Topdown view mode, not visible in flying mode

        tintLight.enabled = top;

        if (hasChosenAUnit) //If a unit is currently chosen, then enable/disable marker
        {
            //If changing to flying camera, enable the unit marker, which is mostly a visual aid.
            EnableFlyingMarker(!inTopdownView);
            //If changing to topdown, then disable it.
        }
    }
    #endregion

    #region Network Commands

    [Command]
    void CmdSpawnMyUnit(Vector3 pos, string name, int level)
    {
        //Set the positions y value to be a bit higher, so that the unit doesn't spawn inside the floor
        pos = new Vector3(pos.x, pos.y + 0.5f, pos.z);

        //Get the unit to spawn
        GameObject unitToSpawn = (GameObject)Resources.Load($"Spawnables/Units/{name}");
        if (unitToSpawn == null)
        {
            Debug.LogError("Could not spawn unit, didn't find the unit in 'Spawnables/Units/', make sure they are in that folder");
            return;
        }

        //Instantiate the unit at the position
        GameObject newUnit = Instantiate(unitToSpawn, pos, Quaternion.identity);

        //Set the units rotation to be random
        newUnit.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));

        UnitBase unit = newUnit.GetComponent<UnitBase>();

        unit.SetUnitSO(unitList[chosenUnitIndex].unit);
        unit.SetLevel(level);

        //Spawn the unit on the server
        NetworkServer.Spawn(newUnit);
    }
    [Command]
    void CmdRefundUnit(GameObject unitToRefund)
    {
        NetworkServer.Destroy(unitToRefund);
    }
    [Command]
    void CmdPlayGlobalSound(bool randomPitch)
    {
        //Assign a random pitch to the audio source
        globalAudio.pitch = randomPitch ? Random.Range(0.95f, 1.05f) : 1;

        //Play the audio
        globalAudio.PlayOneShot(globalAudio.clip);
    }

    #endregion
}