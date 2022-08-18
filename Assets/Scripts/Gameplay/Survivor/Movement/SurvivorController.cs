﻿using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SurvivorController : NetworkBehaviour
{
    float targetY;
    Vector3 moveDirection = Vector3.zero;

    private float horizontal;
    private float vertical;
    private float groundDistance = 0.2f;
    private float x, y = 0;
    private bool isJumping;
    private float baseSpeed = 2.9f;
    public float gravity = 20;

    [SerializeField] private Transform groundCheck = null;
    [SerializeField] private LayerMask groundMask = 0;
    [SerializeField] private AnimationManager animationManager = null;

    private bool moving;

    //public bool Moving
    //{
    //    get { return moving; }
    //    set
    //    {
    //        moving = value;
    //        if (!moving)
    //        {
    //            StoppedMoving();
    //        }
    //    }
    //}

    public Action OnMovementStopped;


    CharacterController cController;
    SurvivorAnimationIKManager anim;
    ModifierManager modifiers;
    public float jumpSpeed;
    public bool isSprinting;
    public bool isGrounded;

    private const string xVelocity = "xVelocity";
    private const string yVelocity = "yVelocity";

    public float BaseSpeed
    {
        get => baseSpeed;
        private set
        {
            baseSpeed = value;
        }
    }

    private void Start()
    {
        cController = GetComponent<CharacterController>();
        anim = GetComponent<SurvivorAnimationIKManager>();
        modifiers = GetComponent<ModifierManager>();
    }

    #region NetworkBehaviour Callbacks
    public override void OnStartAuthority()
    {
        JODSInput.Controls.Survivor.Movement.performed += Move;
        //JODSInput.Controls.Survivor.Movement.canceled += StoppedMoving;
        JODSInput.onMovementDisabled += OnMovementDisabled;
        JODSInput.Controls.Survivor.Jump.performed += Jump;
        JODSInput.Controls.Survivor.Sprint.performed += OnSprintPerformed;
        JODSInput.Controls.Survivor.Sprint.canceled += OnSprintCanceled;
    }
    public override void OnStopAuthority()
    {
        JODSInput.Controls.Survivor.Movement.performed -= Move;
        //JODSInput.Controls.Survivor.Movement.canceled -= StoppedMoving;
        JODSInput.onMovementDisabled -= OnMovementDisabled;
        JODSInput.Controls.Survivor.Jump.performed -= Jump;
        JODSInput.Controls.Survivor.Sprint.performed -= OnSprintPerformed;
        JODSInput.Controls.Survivor.Sprint.canceled -= OnSprintCanceled;
    }
    #endregion

    private void Update()
    {
        if (!hasAuthority) return;

        CheckGround();
        //print(IsMoving());

        if (cController.isGrounded)
        {
            float speed = (isSprinting ? baseSpeed * 1.75f : baseSpeed) * modifiers.MovementSpeed;
            moveDirection = transform.TransformDirection(new Vector3(horizontal, 0.00f, vertical)) * (speed);
            if (isJumping)
            {
                moveDirection.y = jumpSpeed;
                isJumping = false;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;
        cController.Move(moveDirection * Time.deltaTime);
        anim.SetFloat(xVelocity, x = Mathf.Lerp(x, horizontal, Time.deltaTime * 10));
        anim.SetFloat(yVelocity, y = Mathf.Lerp(y, Mathf.Clamp(vertical * modifiers.MovementSpeed, -1f, 2f), Time.deltaTime * 10));
        animationManager.PlayItemContainerAnimation(IsMoving());
    }

    private void Move(InputAction.CallbackContext context)
    {

        Vector2 moveValues = context.ReadValue<Vector2>();
        horizontal = moveValues.x;
        vertical = moveValues.y;
        StartCoroutine(Wait());
        //if (IsMoving2())
        //{
        //    StoppedMoving();
        //}
    }

    private void OnMovementDisabled()
    {
        horizontal = 0f;
        vertical = 0f;
        moveDirection = Vector3.zero;
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
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
    }

    public bool IsMoving() => moveDirection.z != 0 || moveDirection.x != 0;
    IEnumerator Wait()
    {
        yield return null;
        if (!IsMoving())
        {
            StoppedMoving();
        }
    }

    public void StoppedMoving()
    {
        OnMovementStopped?.Invoke();
    }
}
