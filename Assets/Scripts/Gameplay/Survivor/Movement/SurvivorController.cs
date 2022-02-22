﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	private float gravity = 20;
	private float baseSpeed = 2.7f;

	[SerializeField] private Transform groundCheck = null;
	[SerializeField] private LayerMask groundMask = 0;

	CharacterController cController;
	SurvivorAnimationManager anim;
	ModifierManager modifiers;
	public float walkSpeedMultiplier;
	public float sprintSpeedMultiplier;
	public float speedMultiplier;
	public float jumpSpeed;
	public bool isSprinting;
	public bool isGrounded;

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
		anim = GetComponent<SurvivorAnimationManager>();
		modifiers = GetComponent<ModifierManager>();
	}

	#region NetworkBehaviour Callbacks
	public override void OnStartAuthority()
	{
		JODSInput.Controls.Survivor.Movement.performed += Move;
		JODSInput.Controls.Survivor.Jump.performed += Jump;
		JODSInput.Controls.Survivor.Sprint.performed += OnSprintPerformed;
		JODSInput.Controls.Survivor.Sprint.canceled += OnSprintCanceled;
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

		if (cController.isGrounded)
		{
			moveDirection = transform.TransformDirection(new Vector3(horizontal, 0.00f, vertical)) * (modifiers.MovementSpeed * speedMultiplier * baseSpeed);
			if (isJumping)
			{
				moveDirection.y = jumpSpeed;
				isJumping = false;
			}
		}
		moveDirection.y -= gravity * Time.deltaTime;
		cController.Move(moveDirection * Time.deltaTime);
		if (isSprinting && vertical > 0.65)
		{
			speedMultiplier = sprintSpeedMultiplier;
		}
		else
		{
			speedMultiplier = walkSpeedMultiplier;
		}
		anim.SetFloat("xVelocity", x = Mathf.Lerp(x, horizontal, Time.deltaTime * 10));
		anim.SetFloat("yVelocity", y = Mathf.Lerp(y, Mathf.Clamp(vertical * speedMultiplier, -1f, 2f), Time.deltaTime * 10));
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
	}

	private void OnSprintCanceled(InputAction.CallbackContext context)
	{
		isSprinting = false;	
	}

	public bool IsMoving() => (moveDirection.z != 0 || moveDirection.x != 0);
}
