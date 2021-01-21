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
    public SOUnit unit;
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
    [SerializeField] private MasterClass masterClass = null;

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
    }
    [Space]
    public UserInterface UI;
    private List<UnitButton> unitButtons = new List<UnitButton>();

    [System.Serializable]
    public class TopdownMaster
    {
        public Camera camera = null;
        [Space]
        public float movementSpeed = 20f;
        [Space]
        public int currentFloor = 1;
        public int positionChange = 5;
        public int amountOfFloors = 0;
    }
    [Space]
    public TopdownMaster topdown;

    [System.Serializable]
    public class FlyingMaster
    {
        public Camera camera = null;
        public float movementSpeed = 20f;
    }
    [Space]
    public FlyingMaster flying;

    [Space]
    [SerializeField] private bool inTopdownView = true;

    [Header("Other")]
    [SerializeField] private LayerMask ignoreOnRaycast = 1 << 2;

    [Header("Particles and Effects")]
    public ParticleSystem spawnSmokeEffect;
    private AudioSource spawnSmokeAudio;
    private AudioSource globalAudio;

    #endregion

    #region Start / Awake

    private void Start()
    {
        //Add (MASTER to end of name)
        name += " (MASTER)";

        //Do basic startup for master, if player has authority (If the player is the master)
        if (hasAuthority)
        {
            //Enable the mouse cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            inTopdownView = true;

            //Default starting energy stats
            stats.currentEnergy = 50;
            stats.energyRechargeIncrement = 1;

            //Energy UI visuals
            UI.energyFillImage.color = masterClass.energyColor;
            UI.energyUseFillImage.color = masterClass.energyUseColor;

            //Update the Energy UI
            UpdateEnergyUI();
            UpdateEnergyUseUI(0);

            //Make Unit Buttons
            InitializeUnitButtons();

            //Default floor stuff

            //Get all floors in level
            topdown.amountOfFloors = GameObject.FindGameObjectsWithTag("Floor").Length;
            Debug.Log($"There are {topdown.amountOfFloors} floors on this map");

            //Start on a random floor
            topdown.currentFloor = Random.Range(1, topdown.amountOfFloors + 1);

            ChangeFloor();
            //-----------------------

            ActivateUpgradeDecisions(false);

            //Coroutines

            StartCoroutine(EnergyCoroutine());

            StartCoroutine(UpgradeCoroutine(stats.timeUntillNextUpgrade));

            //Misc
            spawnSmokeAudio = spawnSmokeEffect.GetComponent<AudioSource>();
            spawnSmokeAudio.clip = masterClass.spawnSound;

            globalAudio = GetComponent<AudioSource>();
            globalAudio.clip = masterClass.globalSound;

            SetUnitDestinationMarker(false);
            unitDestinationMarker.transform.SetParent(null);
        }
    }

    private void Awake()
    {
        // OI DIK'EDS, THIS DOES NOT WORK, BECAUSE IT DOESN'T GET AUTHORITY BY THE TIME THIS RUNS, SO IT WILL ALWAYS RETURN.
        //if (!hasAuthority) return;
        /*
        //Get the Master Input Action Map
        InputActionMap masterControls = controls.FindActionMap("Master");
        masterControls.Enable();

        //Main Input / Left Mouse basically
        mainInput = masterControls.FindAction("Main");
        mainInput.performed += OnMainInput;

        //Alternate Input / Right Mouse basically
        altInput = masterControls.FindAction("Alt");
        altInput.performed += OnAltInput;

        floorUpInput = masterControls.FindAction("Change Floor Up");
        floorUpInput.performed += context =>
        {
            ChangeFloor(true);
        };
        
        floorDownInput = masterControls.FindAction("Change Floor Down");
        floorDownInput.performed += context =>
        {
            ChangeFloor(false);
        };
        */
    }

    private void OnEnable()
    {
        // Left Mouse Input
        JODSInput.Controls.Master.LMB.performed += ctx => LMB();
        /*
        JODSInput.Controls.Master.ShiftLMB.performed += ctx => Shift_LMB();
        JODSInput.Controls.Master.CtrlLMB.performed += ctx => Ctrl_LMB();
        */

        // Right Mouse Input
        JODSInput.Controls.Master.RMB.performed += ctx => RMB();
        /*
        JODSInput.Controls.Master.ShiftRMB.performed += ctx => Shift_RMB();
        JODSInput.Controls.Master.CtrlRMB.performed += ctx => Ctrl_RMB();
        */

        //Shift Input
        JODSInput.Controls.Master.Shift.started += ctx => ShiftButton(true);
        JODSInput.Controls.Master.Shift.canceled += ctx => ShiftButton(false);

        //Ctrl Input
        JODSInput.Controls.Master.Ctrl.started += ctx => CtrlButton(true);
        JODSInput.Controls.Master.Ctrl.canceled += ctx => CtrlButton(false);


        // Unit Select Input
        JODSInput.Controls.Master.UnitSelecting.performed += ctx => ChooseUnit(Mathf.FloorToInt(ctx.ReadValue<float>() - 1));

        // Movement Input
        JODSInput.Controls.Survivor.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());

        //Floor Input
        JODSInput.Controls.Master.FloorDown.performed += ctx => ChangeFloor(false);
        JODSInput.Controls.Master.FloorUp.performed += ctx => ChangeFloor(true);
    }

    private void OnDisable()
    {
        // Left Mouse Input
        JODSInput.Controls.Master.LMB.performed -= ctx => LMB();
        /*
        JODSInput.Controls.Master.ShiftLMB.performed -= ctx => Shift_LMB();
        JODSInput.Controls.Master.CtrlLMB.performed -= ctx => Ctrl_LMB();
        */

        // Right Mouse Input
        JODSInput.Controls.Master.RMB.performed -= ctx => RMB();
        /*
        JODSInput.Controls.Master.ShiftRMB.performed -= ctx => Shift_RMB();
        JODSInput.Controls.Master.CtrlRMB.performed -= ctx => Ctrl_RMB();
        */

        //Shift Input
        JODSInput.Controls.Master.Shift.started -= ctx => ShiftButton(true);
        JODSInput.Controls.Master.Shift.canceled -= ctx => ShiftButton(false);

        //Ctrl Input
        JODSInput.Controls.Master.Ctrl.started -= ctx => CtrlButton(true);
        JODSInput.Controls.Master.Ctrl.canceled -= ctx => CtrlButton(false);

        // Unit Select Input
        JODSInput.Controls.Master.UnitSelecting.performed -= ctx => ChooseUnit(Mathf.FloorToInt(ctx.ReadValue<float>() - 1));

        //Floor Input
        JODSInput.Controls.Master.FloorDown.performed -= ctx => ChangeFloor(false);
        JODSInput.Controls.Master.FloorUp.performed -= ctx => ChangeFloor(true);
    }

    private void OnValidate()
    {
        if (masterClass)
        {
            SetMasterUnits();
        }
        //If there is no master class selected, then clear the list of units
        else if (unitList.Count != 0) unitList.Clear();
    }

    #endregion

    #region Gameplay Functions

    [SerializeField] private bool shift = false;
    [SerializeField] private bool ctrl = false;
    private void ShiftButton(bool down)
    {
        shift = down;
    }
    private void CtrlButton(bool down)
    {
        ctrl = down;
    }

    public void SetMasterClass(MasterClass mClass)
    {
        masterClass = mClass;
        SetMasterUnits();
        InitializeUnitButtons();
    }

    #region Normal Mouse
    private void LMB()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { return; }
        if (shift && !ctrl) { Shift_LMB(); return; }
        else if (ctrl && !shift) { Ctrl_LMB(); return; }

        //Somehow check if master clicks on a unit button, if so do not spawn anything.

        TryToSpawnUnit();
    }
    private void RMB()
    {
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

            print("select");
        }
    }

    #endregion

    #region UI Functions

    private void InitializeUnitButtons()
    {
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
            return Vector3.Distance(
                selectedUnit.gameObject.transform.position,
                selectedUnit.GetComponent<NavMeshAgent>().destination
                ) < 2;
        }
        else return false;
    }

    #endregion

    #region Unit Functions

    private void SetMasterUnits()
    {
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
    }

    private void UnchooseUnit()
    {
        hasChosenAUnit = false;

        //Change the UI
        UnitButtonChooseUI(false);
        UpdateEnergyUseUI(0);
    }
    #endregion

    #region Spawning & Refunding
    void TryToSpawnUnit()
    {
        Ray ray = CameraRay();

        //Shoot a raycast from the mouse cursor position
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~ignoreOnRaycast))
        {
            //Did the raycast hit an area where units can spawn? 
            //(The "Ground" tag is currently the only surface where units can spawn)
            if (hit.collider.CompareTag("Ground"))
            {
                //Have I chosen a unit to spawn?
                if (hasChosenAUnit)
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
        SOUnit chosenUnit = unitList[chosenUnitIndex].unit;

        //Check if the spawn location meets the requirements.
        if (!ViewCheck(hit.point, true)) return;

        //Spawn a smoke effect to hide the instantiation of the unit.
        SmokeEffect(hit.point);

        //If the spawn location meets the requirements, then spawn the currently selected unit.

        //A random unit from the chosen unit's prefab list gets picked, and the name gets sent to the server, which then spawns the unit.
        //This is because there can be multiple variations of one unit.
        CmdSpawnMyUnit(hit.point,
            chosenUnit.unitPrefab[Random.Range(0, chosenUnit.unitPrefab.Length)].name,
            unitList[chosenUnitIndex].level);

        //Master loses energy, because nothing is free in life
        IncreaseEnergy(-chosenUnit.energyCost);
        UpdateEnergyUI();//Update UI
        IncreaseXp(chosenUnit.xpGain); //Master gains xp though
    }

    void TryToRefundUnit()
    {
        Ray ray = CameraRay();

        //Shoot a raycast from the mouse cursor position
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~ignoreOnRaycast))
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
        Ray ray = CameraRay();

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~ignoreOnRaycast))
        {
            //If I click on a unit, try and select that unit
            if (hit.collider.TryGetComponent(out UnitBase unit))
            {
                if (unit.select.canSelect)
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
        selectedUnit.Select();

        StartSelectCoroutine(); print("Select Unit");
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
        Ray ray = CameraRay();

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~ignoreOnRaycast))
        {
            //If I don't have a unit currently selected, then return
            if (!selectedUnit) return;

            //Command my selected unit to move to the location
            selectedUnit.MoveToLocation(hit.point);

            StartSelectCoroutine(); print("Command Unit");
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
            print("START");
            selectCo = StartCoroutine(SelectCoroutine());
            selectCoroutineActive = true; 
        }
    }
    #endregion

    #region Checks

    //This function determines which camera to shoot a ray from
    private Ray CameraRay()
    {
        Ray ray;
        if (inTopdownView)
        {
            ray = topdown.camera.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            ray = flying.camera.ScreenPointToRay(Input.mousePosition);
        }
        return ray;
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
        int arraySize = masterClass.units.Length;
        unitList.Clear();

        for (int i = 0; i < arraySize; i++)
        {
            unitList.Add(new UnitList());
            unitList[i].unit = masterClass.units[i];
        }
    }

    private bool UnitInitialization()
    {
        if (unitList.Count == 0) return true;
        //Check if the list is as it should be. If it is ok, there is no need to remake it.
        for (int i = 0; i < masterClass.units.Length; i++)
        {
            if (unitList[i].unit != masterClass.units[i])
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
    private float horizontal; // These variables are used to move the player. 
    private float vertical; // They store the player's input values.
    private void Update()
    {
        //Movement
        transform.Translate(
            horizontal * Time.deltaTime * (topdown.movementSpeed * (shift ? 1.5f : 1f)),
            0, 
            vertical * Time.deltaTime * (topdown.movementSpeed * (shift ? 1.5f : 1f))) ;

        //Mouse Scroll / Camera Zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            topdown.camera.orthographicSize = Mathf.Clamp(topdown.camera.orthographicSize + -Input.GetAxis("Mouse ScrollWheel") * 5, 10, 20);
        }
    }
    private void Move(Vector2 moveValues)
    {
        horizontal = moveValues.x;
        vertical = moveValues.y;
    }
    #endregion

    #region Floor Navigation
    void ChangeFloor()
    {
        topdown.camera.transform.position = new Vector3(
            topdown.camera.transform.position.x,
            topdown.positionChange * topdown.currentFloor - 0.05f,
            topdown.camera.transform.position.z);
    }
    void ChangeFloor(bool up)
    {
        if (!inTopdownView)
        {
            Debug.Log($"Cannot move {(up ? "up" : "down")} floors when not in topdown view mode");
            return;
        }

        print("Changing floor " + (up ? "up" : "down"));

        topdown.currentFloor = Mathf.Clamp(topdown.currentFloor += up ? -1 : 1, 1, topdown.amountOfFloors); //This needs fixin

        topdown.camera.transform.position = new Vector3(
            topdown.camera.transform.position.x,
            topdown.positionChange * topdown.currentFloor - 0.05f,
            topdown.camera.transform.position.z);
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