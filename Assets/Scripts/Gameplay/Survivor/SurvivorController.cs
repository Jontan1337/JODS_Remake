using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorController : MonoBehaviour
{
    CharacterController cc;
    public float speed;
    public float jumpSpeed;
    public float gravity;
    float targetY;
    Vector3 moveDirection = Vector3.zero;
    private float horizontal;
    private float vertical;

    [SerializeField]
    private Transform groundCheck;
    private float groundDistance = 0.2f;
    [SerializeField]
    private LayerMask groundMask;
    public bool isGrounded;

    private void Start()
    {
        JODSInput.Controls.Survivor.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        JODSInput.Controls.Survivor.Jump.performed += ctx => Jump();
        cc = GetComponent<CharacterController>();
    }


    private void Update()
    {
        CheckGround();
        if (cc.isGrounded)
        {
            moveDirection = transform.TransformDirection(new Vector3(horizontal, 0.05f, vertical)) * speed;
        }
        //if (JODSInput.Controls.Survivor.Jump.triggered) Jump();
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
            moveDirection.y = jumpSpeed;
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
}
