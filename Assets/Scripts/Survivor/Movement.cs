using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Movement : NetworkBehaviour
{
	CharacterController characterController;
    MenuHandler menu;
    private Shoot shoot;
	public new Camera camera;
    private Animator anim;
    [SerializeField] private Animator armPivotAnim = null;
    [SerializeField] private Animator cameraAnim = null;
    [SyncVar] public bool walking;
    [SyncVar] public bool hasGun;

    
	public float jumpSpeed = 4.0f;
	public float gravity = 20.0f;
	float minRotY = -75f;
	float maxRotY = 75F;
	public float rotY;
	public float rotX;
	public bool auth;
	public bool isSprinting = false;
	public bool cameraIsLocked;
    public bool isGrabbed = false;
	public float standardSpeed = 4f;
    private float speed;
	float backSpeed = 0.75f;
	float sideSpeed = 0.75f;
	float sprintSpeed = 1.5f;
	float zoomSpeed = 0.8f;
	float lookSpeed = 3.0f;
    float sensitivity = 1f;

	//float verticalSpeed = 1.5f;

	public Vector3 moveDirection = Vector3.zero;

    [Header("Audio")]
    private AudioSource AS;
    public AudioClip[] footsteps;
    private bool footstep;

    private float horizontal;
    private float vertical;


    void Start()
    {
        speed = standardSpeed;
        shoot = GetComponent<Shoot>();
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        menu = GetComponent<MenuHandler>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameObject[] tempGO = GameObject.FindGameObjectsWithTag("WallPieces");
        if (tempGO.Length > 0)
        {
            foreach (GameObject item in tempGO)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), item.GetComponent<MeshCollider>(), true);
            }
        }
        AS = GetComponent<AudioSource>();
    }
	void Update()
    {
		if (!cameraIsLocked)
		{
            if (hasAuthority)
            {
                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");
            }

            // Set player movement input to zero
            if (menu.IsOpen || isGrabbed)
            {
                horizontal = vertical = 0;
            }

            //If slowed down, speed up over tiem
            if (speed < standardSpeed)
            {
                speed = Mathf.Clamp(speed += Time.deltaTime,0,standardSpeed);
                //Debug.Log(speed);
            }

            isSprinting = false;
			walking = characterController.velocity.magnitude >= 0.5f;
            //hasGun = shoot.hasWeapon;
            cameraAnim.SetBool("IsWalking", walking);
            armPivotAnim.SetBool("IsWalking", walking);
            cameraAnim.speed = Mathf.Clamp(characterController.velocity.magnitude / 4, 0.5f, 5f);
            armPivotAnim.speed = Mathf.Clamp(characterController.velocity.magnitude / 4, 0.5f, 5f);
			if (hasAuthority == false && auth == false)
			{
				return;
			}
            Cmd_IsWalking(walking);
			if (characterController.isGrounded)
			{
				moveDirection = new Vector3(horizontal, 0.0f, vertical);
				moveDirection = transform.TransformDirection(moveDirection);

                //Fixes the issue of moving faster if going forwards and sideways.
                if (moveDirection.magnitude > 1)
                {
                    moveDirection.Normalize();
                }

                //Just for footsteps lol
                if (walking)
                {
                    if (!footstep)
                    {
                        footstep = true;
                        PlayFootstepSound();
                    }
                }


                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
				{
					moveDirection = moveDirection * speed * sprintSpeed;
					isSprinting = true;
				}
				else if (Input.GetKey(KeyCode.S))
				{
					moveDirection = moveDirection * speed * backSpeed;
				}
				else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
				{
					moveDirection = moveDirection * speed * sideSpeed;
				}
				else
				{
					moveDirection = moveDirection * speed;
				}

				if (Input.GetButton("Jump") && !isGrabbed)
				{
					moveDirection.y = jumpSpeed;
				}
			}

            //First Person Controls
            if (!menu.IsOpen)
            {
                if (PlayerPrefs.GetFloat("Mouse Sensitivity") != sensitivity) sensitivity = PlayerPrefs.GetFloat("Mouse Sensitivity");


                transform.Rotate(0, lookSpeed * Input.GetAxis("Mouse X") * sensitivity, 0);

                if (shoot.isZooming)
                {
                    lookSpeed = 1f;
                }
                else
                {
                    lookSpeed = 3f;
                }

			    rotY += Input.GetAxis("Mouse Y") * lookSpeed * sensitivity;
			    rotX += Input.GetAxis("Mouse X") * lookSpeed * sensitivity;


			    rotY = Mathf.Clamp(rotY, minRotY, maxRotY);


			    camera.transform.rotation = Quaternion.Euler(-rotY, rotX, 0f);
            }


			moveDirection.y -= gravity * Time.deltaTime;
            characterController.Move(shoot.isZooming ? moveDirection * Time.deltaTime * zoomSpeed : moveDirection * Time.deltaTime);
		}
	}

    [Command]
    private void Cmd_IsWalking(bool isWalking)
    {
        anim.SetBool("Walk", walking);
        anim.SetBool("hasGun", hasGun);
    }

    public void GetGun(bool has)
    {
        CmdGetGun(has);
    }

    [Command]
    void CmdGetGun(bool has)
    {
        hasGun = has;
    }

    public void ToggleGrab()
    {
        Debug.LogError("grab");
        isGrabbed = !isGrabbed;
    }

    public void SlowDown()
    {
        //Debug.Log("Old speed " + speed);
        //Debug.Log("Speed range : " + standardSpeed / 4 + " | " + standardSpeed / 3);
        float newSpeed = Mathf.Clamp(speed -= Random.Range(speed / 4, speed / 3), standardSpeed / 3, standardSpeed);
        //Debug.Log("New speed : " + newSpeed);
        speed -= Mathf.Clamp(speed - newSpeed,standardSpeed / 4,standardSpeed);
        //Debug.Log("New speed " + speed);
    }

    private void PlayFootstepSound()
    {
        if (walking)
        {
            AS.volume = 0.7f;
            AS.pitch = Random.Range(0.9f, 1.1f);
            AS.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
            Invoke(nameof(PlayFootstepSound), (isSprinting) ? 0.3f : 0.5f);
        }
        else
        {
            footstep = false;
        }
    }
}
