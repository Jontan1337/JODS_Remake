using Mirror;
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

    [SerializeField] private Transform groundCheck = null;
    [SerializeField] private LayerMask groundMask = 0;
    
    CharacterController cc;
    SurvivorAnimationManager anim;
    public float speed;
    public float sprintSpeedMultiplier;
    public float jumpSpeed;
    public float gravity;
    public bool isSprinting;
    public bool isGrounded;


    private void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<SurvivorAnimationManager>();
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
		anim.SetFloat("xVelocity", x = Mathf.Lerp(x, horizontal, Time.deltaTime * 10));
		anim.SetFloat("yVelocity", y = Mathf.Lerp(y, vertical, Time.deltaTime * 10));
		//anim.SetFloat("xVelocity", horizontal);
        //anim.SetFloat("yVelocity", vertical);
        //anim.SetBool("Walking", IsMoving());
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
        anim.SpeedUp(sprintSpeedMultiplier);
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
        speed /= sprintSpeedMultiplier;
        anim.SlowDown(sprintSpeedMultiplier);
    }

    public bool IsMoving() => (moveDirection.z != 0 || moveDirection.x != 0);
}
