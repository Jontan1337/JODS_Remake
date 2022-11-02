using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitMaster_TopdownController : MonoBehaviour
{
    private Camera cam = null;
    [Space]
    [SerializeField] private float movementSpeed = 20f;
    [Space]
    [SerializeField] private int currentFloor = 1;
    [SerializeField] private int positionChange = 5;
    [SerializeField] private int amountOfFloors = 0;

    [Header("UI")]
    [SerializeField] private Transform floorUIContainer = null;
    [SerializeField] private GameObject floorIndicatorPrefab = null;
    private List<UnitMasterFloorIndicator> floorIndicators = new List<UnitMasterFloorIndicator>();

    private float horizontal; // These variables are used to move the player. 
    private float vertical; // They store the player's input values.

    private bool shift = false; //Used to check if the player is holding down the shift key

    private void ShiftButton(bool down)
    {
        shift = down; //yep
    }

    private void Start()
    {
        cam = GetComponent<Camera>();


        //Default floor stuff
        FloorSetup();


        //Input stuff

        //Shift Input
        JODSInput.Controls.Master.Shift.started += ctx => ShiftButton(true);
        JODSInput.Controls.Master.Shift.canceled += ctx => ShiftButton(false);

        // Movement Input
        JODSInput.Controls.Master.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());

        //Floor Input
        JODSInput.Controls.Master.FloorDown.performed += ctx => ChangeFloor(false);
        JODSInput.Controls.Master.FloorUp.performed += ctx => ChangeFloor(true);

        JODSInput.Controls.Master.ScrollWheel.performed += OnScroll;
    }

    private void OnScroll(InputAction.CallbackContext context)
    {        
        scrollInput = context.ReadValue<Vector2>();
    }

    public Vector2 scrollInput = new Vector2(0, 0);

    #region Movement
    private void Update()
    {
        //Movement
        transform.Translate(
            horizontal * Time.deltaTime * (movementSpeed * (shift ? 1.5f : 1f)),
            0,
            vertical * Time.deltaTime * (movementSpeed * (shift ? 1.5f : 1f)),
            Space.World);

        //Mouse Scroll / Camera Zoom
        if (scrollInput.y != 0f)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + -scrollInput.y, 10, 20);
            scrollInput = Vector2.zero;
        }
    }
    private void Move(Vector2 moveValues)
    {
        horizontal = moveValues.x;
        vertical = moveValues.y;
    }
    #endregion
    #region Floor Navigation

    void FloorSetup()
    {
        WorldSettings worldSettings = WorldSettings.instance;

        //Get amount of floors from WorldSettings. If there is no worldSettings then assign with a value of 1.
        amountOfFloors = worldSettings == null ? 1 : worldSettings.amountOfFloors;

        //Debug.Log($"There are {amountOfFloors} floors on this map");

        //Instantiate floor indicators for each floor on the map
        for (int i = 0; i < amountOfFloors; i++)
        {
            floorIndicators.Add(Instantiate(floorIndicatorPrefab, floorUIContainer).GetComponent<UnitMasterFloorIndicator>());
        }

        //Start on a random floor
        int rndFloor = Random.Range(1, amountOfFloors + 1);
        currentFloor = rndFloor;
        //Debug.Log($"Current floor : {currentFloor}");

        ChangeFloor();
    }

    void ChangeFloor()
    {
        for (int i = 0; i < floorIndicators.Count; i++)
        {
            floorIndicators[i].Select(i == currentFloor - 1);
        }

        cam.transform.position = new Vector3(
            cam.transform.position.x,
            positionChange * currentFloor - 0.05f,
            cam.transform.position.z);
    }
    void ChangeFloor(bool up)
    {
        //print("Changing floor " + (up ? "up" : "down"));

        currentFloor = Mathf.Clamp(currentFloor += up ? 1 : -1, 1, amountOfFloors); //This needs fixin

        for (int i = 0; i < floorIndicators.Count; i++)
        {
            floorIndicators[i].Select(i == currentFloor - 1);
        }

        cam.transform.position = new Vector3(
            cam.transform.position.x,
            positionChange * currentFloor - 0.05f,
            cam.transform.position.z);
    }
    #endregion
}