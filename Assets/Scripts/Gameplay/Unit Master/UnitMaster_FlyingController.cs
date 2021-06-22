using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMaster_FlyingController : MonoBehaviour
{
    private Camera cam = null;
    private Rigidbody rb = null;
    public UnitMaster master;

    [SerializeField] private float movementSpeed = 50f;
    [SerializeField] private float speedModifier = 1.8f;
    [Space]
    [SerializeField] private float minRotY = -60f;
    [SerializeField] private float maxRotY = 60f;
    [Space]
    [SerializeField] [Range(1, 20)] private float sensitivity = 10f;
    [Space]
    [SerializeField] private Transform cursorMarker = null;
    [SerializeField] private bool showMarker = false;    

    //Movement variables
    private float m_horizontal; // These variables are used to move the player. 
    private float m_vertical; // They store the player's input values.

    //Camera variables
    private float r_horizontal; // These variables are used to move the player. 
    private float r_vertical; // They store the player's input values.

    private void Start()
    {
        cam = GetComponent<Camera>();
        rb = GetComponent<Rigidbody>();

        //Input stuff

        // Shift Input
        JODSInput.Controls.Master.Shift.started += ctx => ShiftButton(true);
        JODSInput.Controls.Master.Shift.canceled += ctx => ShiftButton(false);

        // Movement Input
        JODSInput.Controls.Master.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());

        // Camera Input
        JODSInput.Controls.Master.Camera.performed += ctx => Rotate(ctx.ReadValue<Vector2>());
    }

    [Space]
    [SerializeField] private bool shift = false;
    private void ShiftButton(bool down)
    {
        shift = down;
    }

    #region Movement
    private void FixedUpdate()
    {
        Vector3 forwardForce = transform.forward * m_vertical * 
            (movementSpeed * (shift ? speedModifier : 1f));
        Vector3 sideForce = transform.right * m_horizontal * 
            (movementSpeed * (shift ? speedModifier : 1f));

        //Movement
        rb.AddForce(forwardForce);
        rb.AddForce(sideForce);
    }
    private void Move(Vector2 moveValues)
    {
        m_horizontal = moveValues.x;
        m_vertical = moveValues.y;
    }
    #endregion

    private void Rotate(Vector2 rotateValues)
    {
        r_horizontal = rotateValues.x * sensitivity;
        r_vertical = rotateValues.y * sensitivity;
    }
    private void Update()
    {
        //Rotation

        //This does not clamp the rotation
        transform.Rotate(Vector3.right * -r_vertical * Time.deltaTime, Space.Self);
        transform.Rotate(Vector3.up * r_horizontal * Time.deltaTime, Space.World);


        //Raycast Marker
        if (showMarker) //If the master has selected a unit, this bool will be true
        {
            //Call the master's Raycast method, which returns a raycast's hit and a bool if it did hit.
            //This output changes based on what camera controller the master is currently using.
            master.Raycast(out bool didHit, out RaycastHit hit);

            cursorMarker.gameObject.SetActive(didHit);

            if (didHit)
            {
                //Change the position of the marker, to where the raycast hit.
                cursorMarker.position = hit.point;
            }
        }
    }

    #region Marker

    public void ShowMarker(bool active)
    {
        showMarker = active;
        cursorMarker.gameObject.SetActive(active);
    }

    public void ChangeMarker(Mesh markerMesh, Color markerColor)
    {
        //Get the material, and change the colours
        Material mat = cursorMarker.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        mat.color = markerColor;
        mat.SetColor("_EmissionColor", markerColor);

        //Change the mesh
        cursorMarker.gameObject.GetComponent<MeshFilter>().mesh = markerMesh;
    }

    #endregion
}
