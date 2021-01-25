using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master_FlyingController : MonoBehaviour
{
    private Camera cam = null;
    private Rigidbody rb = null;

    [SerializeField] private float movementSpeed = 50f;

    [SerializeField] private float minRotY = -60f;
    [SerializeField] private float maxRotY = 60f;
    [SerializeField] private float sensitivity = 1f;

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

    private void OnEnable()
    {
        /*
        // Shift Input
        JODSInput.Controls.Master.Shift.started += ctx => ShiftButton(true);
        JODSInput.Controls.Master.Shift.canceled += ctx => ShiftButton(false);

        // Movement Input
        JODSInput.Controls.Master.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());

        // Camera Input
        JODSInput.Controls.Master.Camera.performed += ctx => Rotate(ctx.ReadValue<Vector2>());
        */
    }

    private void OnDisable()
    {
        /*
        // Shift Input
        JODSInput.Controls.Master.Shift.started -= ctx => ShiftButton(true);
        JODSInput.Controls.Master.Shift.canceled -= ctx => ShiftButton(false);

        // Movement Input
        JODSInput.Controls.Master.Movement.performed -= ctx => Move(ctx.ReadValue<Vector2>());

        // Camera Input
        JODSInput.Controls.Master.Camera.performed -= ctx => Rotate(ctx.ReadValue<Vector2>());
        */
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
        Vector3 force = transform.forward * m_vertical * (movementSpeed * (shift ? 1.5f : 1f));

        //Movement
        rb.AddForce(force);

        /*
        cam.transform.Translate(
            m_horizontal * Time.deltaTime * (movementSpeed * (shift ? 1.5f : 1f)),
            0,
            m_vertical * Time.deltaTime * (movementSpeed * (shift ? 1.5f : 1f)),
            Space.Self);
        */
    }
    private void Move(Vector2 moveValues)
    {
        m_horizontal = moveValues.x;
        m_vertical = moveValues.y;
    }
    #endregion

    private void Rotate(Vector2 rotateValues)
    {
        r_horizontal = rotateValues.x;
        r_vertical = rotateValues.y;

        //This didnt work
        //r_vertical = Mathf.Clamp(r_vertical, minRotY, maxRotY);
    }
    private void Update()
    {
        //This needs to be changed, it doesnt even use sensitivity, 
        //nor does it consider the clamped y rotation
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x + -r_vertical,
            transform.eulerAngles.y + r_horizontal,
            0f);
    }
}
