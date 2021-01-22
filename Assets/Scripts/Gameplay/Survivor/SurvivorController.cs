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

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        JODSInput.Controls.Survivor.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        JODSInput.Controls.Survivor.Jump.performed += ctx => Jump(ctx.ReadValue<float>());
    }

    private void OnDisable()
    {
        JODSInput.Controls.Survivor.Movement.performed -= ctx => Move(ctx.ReadValue<Vector2>());
        JODSInput.Controls.Survivor.Jump.performed -= ctx => Jump(ctx.ReadValue<float>());
    }

    private void Update()
    {
        if (cc.isGrounded)
        {
            moveDirection = transform.TransformDirection(new Vector3(horizontal, 0.0f, vertical)) * speed;
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Jump(jumpSpeed);
        //}
        moveDirection.y -= gravity * Time.deltaTime;
        cc.Move(moveDirection * Time.deltaTime);
    }

    private void Move(Vector2 moveValues)
    {
        horizontal = moveValues.x;
        vertical = moveValues.y;
    }

    private void Jump(float jumpSpeed)
    {
        if (cc.isGrounded)
        {
            moveDirection.y += jumpSpeed;
            print("g0");
        }
    }

}
