using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DavidTest : MonoBehaviour
{
    private CharacterController controller;

    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    public Vector3 move;

    public bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        JODSInput.Controls.Survivor.Jump.performed += ctx => Jump();

        JODSInput.Controls.Master.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;

        moveDirection += move;
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    private void Jump()
    {
        print("Jump");

        if (controller.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
    }

    private void Move(Vector2 movement)
    {
        print("Move");
        Vector3 forward = transform.forward * movement.y;
        Vector3 side = transform.right * movement.x;

        move = (forward + side).normalized;
    }
}
