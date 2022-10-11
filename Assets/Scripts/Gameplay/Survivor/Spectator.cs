using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Spectator : MonoBehaviour
{
    private CharacterController characterController = null;
    Vector3 moveDirection = Vector3.zero;

    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float gravity = 20f;

    private float m_horizontal;
    private float m_vertical;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        JODSInput.Controls.Survivor.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
    }

    private void OnDisable()
    {
        JODSInput.Controls.Survivor.Movement.performed -= ctx => Move(ctx.ReadValue<Vector2>());
    }

    private void Move(Vector2 moveValues)
    {
        m_horizontal = moveValues.x;
        m_vertical = moveValues.y;
    }

    private void Update()
    {
        //Movement
        if (characterController.isGrounded)
        {
            moveDirection = transform.TransformDirection(new Vector3(m_horizontal, 0.00f, m_vertical)) * movementSpeed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
