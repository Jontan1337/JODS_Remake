using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyController : NetworkBehaviour
{
    [Header("Movement settings")]
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private float acceleration = 1f;

    [Header("References")]
    [SerializeField]
    private Rigidbody playerRigidbody = null;

    Vector3 movement = Vector3.zero;
    private float horizontalMove = 0f;
    private float verticalMove = 0f;

    private void Awake()
    {
        if (acceleration == 1)
        {

        }
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
        movement = transform.TransformDirection(new Vector3(horizontalMove, 0f, verticalMove)) * moveSpeed;

        playerRigidbody.AddForce(movement, ForceMode.Impulse);
    }

    private void Move(Vector2 moveValues)
    {
        horizontalMove = moveValues.x;
        verticalMove = moveValues.y;
        Debug.Log(moveValues);
    }
}
