using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyController : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private float acceleration = 1f;

    [Header("References")]
    [SerializeField]
    private Rigidbody playerRigidbody = null;
    [SerializeField]
    private Camera playerCamera = null;
    [SerializeField]
    private Transform playerModel = null;

    float sensititivy = 50f;
    float sensitivityMultiplier = 1f;


    Vector3 movement = Vector3.zero;
    private float horizontalMove = 0f;
    private float verticalMove = 0f;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        JODSInput.Controls.Survivor.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
    }

    private void OnDisable()
    {
        JODSInput.Controls.Survivor.Movement.performed -= ctx => Move(ctx.ReadValue<Vector2>());
    }

    private void FixedUpdate()
    {
        movement = playerModel.TransformDirection(horizontalMove, 0.0f, verticalMove);

        playerRigidbody.AddForce(movement * Time.fixedDeltaTime * moveSpeed);
    }

    private void Move(Vector2 moveValues)
    {
        horizontalMove = moveValues.x;
        verticalMove = moveValues.y;
        Debug.Log(moveValues);
    }
}
