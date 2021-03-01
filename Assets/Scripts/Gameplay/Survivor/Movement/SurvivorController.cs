using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SurvivorController : MonoBehaviour
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
        JODSInput.Controls.Survivor.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        JODSInput.Controls.Survivor.Jump.performed += ctx => Jump();
        JODSInput.Controls.Survivor.Sprint.performed += ctx => OnSprintPerformed();
        JODSInput.Controls.Survivor.Sprint.canceled += ctx => OnSprintCanceled();
        cc = GetComponent<CharacterController>();
    }


    private void Update()
    {
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

    private void Move(Vector2 moveValues)
    {
        horizontal = moveValues.x;
        vertical = moveValues.y;
    }

    private void Jump()
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

    private void OnSprintPerformed()
    {
        isSprinting = true;
        speed *= sprintSpeedMultiplier;
    }

    private void OnSprintCanceled()
    {
        isSprinting = false;
        speed /= sprintSpeedMultiplier;
    }

    public bool IsMoving() => (moveDirection.z != 0 || moveDirection.x != 0);

}
