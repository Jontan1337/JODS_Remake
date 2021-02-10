using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master_TopdownController : MonoBehaviour
{
    private Camera cam = null;
    [Space]
    [SerializeField] private float movementSpeed = 20f;
    [Space]
    [SerializeField] private int currentFloor = 1;
    [SerializeField] private int positionChange = 5;
    [SerializeField] private int amountOfFloors = 0;

    [Header("UI")]
    [SerializeField] private Transform floorUIContainer;
    [SerializeField] private GameObject floorIndicatorPrefab;
    private List<FloorIndicator> floorIndicators = new List<FloorIndicator>();

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

        //Get all floors in level
        amountOfFloors = GameObject.FindGameObjectsWithTag("Floor").Length;
        Debug.Log($"There are {amountOfFloors} floors on this map");

        //Instantiate floor indicators for each floor on the map
        for (int i = 0; i < amountOfFloors; i++)
        {
            floorIndicators.Add(Instantiate(floorIndicatorPrefab, floorUIContainer).GetComponent<FloorIndicator>());
        }

        //Start on a random floor
        int rndFloor = Random.Range(1, amountOfFloors + 1);
        currentFloor = rndFloor;
        Debug.Log($"Current floor : {currentFloor}");

        ChangeFloor();



        //Input stuff

        //Shift Input
        JODSInput.Controls.Master.Shift.started += ctx => ShiftButton(true);
        JODSInput.Controls.Master.Shift.canceled += ctx => ShiftButton(false);

        // Movement Input
        JODSInput.Controls.Master.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());

        //Floor Input
        JODSInput.Controls.Master.FloorDown.performed += ctx => ChangeFloor(false);
        JODSInput.Controls.Master.FloorUp.performed += ctx => ChangeFloor(true);
    }
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
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + -Input.GetAxis("Mouse ScrollWheel") * 5, 10, 20);
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
        print("Changing floor " + (up ? "up" : "down"));

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