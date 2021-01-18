using UnityEngine;

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

    Vector3 movement = Vector3.zero;
    private Controls controls = null;
    private float horizontalMove = 0f;
    private float verticalMove = 0f;

    private void Awake()
    {
        controls = new Controls();
        playerRigidbody = GetComponent<Rigidbody>();
        controls.Enable();
    }

    private void OnEnable()
    {
        controls.Survivor.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
    }

    private void OnDisable()
    {
        controls.Survivor.Movement.performed -= ctx => Move(ctx.ReadValue<Vector2>());
    }

    private void FixedUpdate()
    {
        movement = transform.TransformDirection(new Vector3(horizontalMove, 0f, verticalMove)) * moveSpeed;

        playerRigidbody.AddForce(movement * moveSpeed * Time.fixedDeltaTime, ForceMode.Force);
    }

    private void Move(Vector2 moveValues)
    {
        horizontalMove = moveValues.x;
        verticalMove = moveValues.y;
        print(movement);
    }
}
