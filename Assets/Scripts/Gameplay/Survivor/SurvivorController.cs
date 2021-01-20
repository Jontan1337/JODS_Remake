using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorController : MonoBehaviour
{
    CharacterController cc;
    public float speed = 1f;
    public float gravity;
    Vector3 moveDirection = Vector3.zero;
    private float horizontal;
    private float vertical;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        //JODSInput.Controls.Enable();
    }

    private void OnEnable()
    {
        JODSInput.Controls.Survivor.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        //Debug.Log();
    }

    private void OnDisable()
    {
        JODSInput.Controls.Survivor.Movement.performed -= ctx => Move(ctx.ReadValue<Vector2>());
    }

    private void Update()
    {
        if (cc.isGrounded)
        {
            moveDirection = transform.TransformDirection(new Vector3(horizontal, 0.0f, vertical)) * speed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        cc.Move(moveDirection);
    }
    private void Move(Vector2 moveValues)
    {
        horizontal = moveValues.x;
        vertical = moveValues.y;
        Debug.Log(moveValues);
    }

}
