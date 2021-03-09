using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SurvivorController : NetworkBehaviour
{
    CharacterController cc;
    public float speed;
    public float sprintSpeedMultiplier;
    public float jumpSpeed;
    public float gravity;
    public bool isSprinting;
    float targetY;
    Vector3 moveDirection = Vector3.zero;

    private float horizontal;
    private float vertical;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    private float groundDistance = 0.2f;
    public bool isGrounded;
    bool isJumping;

    private void Start()
    {
    }

    #region NetworkBehaviour Callbacks
    public override void OnStartAuthority()
    {
        JODSInput.Controls.Survivor.Movement.performed += Move;
        JODSInput.Controls.Survivor.Jump.performed += Jump;
        JODSInput.Controls.Survivor.Sprint.performed += OnSprintPerformed;
        JODSInput.Controls.Survivor.Sprint.canceled += OnSprintCanceled;
        cc = GetComponent<CharacterController>();
    }
    public override void OnStopAuthority()
    {
        JODSInput.Controls.Survivor.Movement.performed -= Move;
        JODSInput.Controls.Survivor.Jump.performed -= Jump;
        JODSInput.Controls.Survivor.Sprint.performed -= OnSprintPerformed;
        JODSInput.Controls.Survivor.Sprint.canceled -= OnSprintCanceled;
    }
    #endregion


    private void Update()
    {
        if (!hasAuthority) return;

        CheckGround();
        if (cc.isGrounded)
        {
            moveDirection = transform.TransformDirection(new Vector3(horizontal, 0.00f, vertical)) * speed;
            if (isJumping)
            {
                moveDirection.y = jumpSpeed;
                isJumping = false;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;
        cc.Move(moveDirection * Time.deltaTime);
    }

    private void Move(InputAction.CallbackContext context)
    {
        Vector2 moveValues = context.ReadValue<Vector2>();
        horizontal = moveValues.x;
        vertical = moveValues.y;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            isJumping = true;
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        isSprinting = true;
        speed *= sprintSpeedMultiplier;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
        speed /= sprintSpeedMultiplier;
    }

    public bool IsMoving() => (moveDirection.z != 0 || moveDirection.x != 0);
}
