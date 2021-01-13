using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class TaekwondoClass : NetworkBehaviour
{
	CharacterController characterController;
	public new Camera camera;
	private Movement move;
	private Shoot shoot;
    private Stats stats;
	public GameObject unit;
	public GameObject taekwondoLeg;
	public GameObject hands;
	public Animator legAnim;
	RaycastHit hit;
	bool flyKicking = false;
	float timeFlown = 1;
	public float timeInFlight = 0.5f;
	public float cooldownLeft = 1;
	public int flyingKickDamage = 100;
	public int kickDamage = 35;
	public int flightSpeed = 20;
    public LayerMask ignoreOnRaycast;

    private List<GameObject> zombiesHit = new List<GameObject>();
	
	void Start()
	{
        if (hasAuthority)
        {
            stats = GetComponent<Stats>();
            cooldownLeft = stats.maxCooldown;
            move = GetComponent<Movement>();
            characterController = GetComponent<CharacterController>();
            shoot = GetComponent<Shoot>();
        }
	}

	// Update is called once per frame
	void Update()
	{
        if (!hasAuthority) return;

        stats.SpecialCooldown = cooldownLeft;
        if (Input.GetKeyDown(KeyCode.C) && !flyKicking && cooldownLeft >= stats.maxCooldown)
        {
            cooldownLeft = 0;
            timeFlown = 0;
            FlyingKickStart();
        }
        if (flyKicking && timeFlown < 0.75)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * flightSpeed);
            timeFlown += Time.deltaTime;
            hands.SetActive(false);
        }
        else
        {
            FlyingKickStop();
        }
        if (cooldownLeft < stats.maxCooldown)
        {
            cooldownLeft += Time.deltaTime;
        }

        if (!shoot.hasWeapon && Input.GetKeyDown(KeyCode.Mouse0))
        {
            legAnim.SetTrigger("Side_Kick");
        }
	}


	public void FlyingKickStart()
	{
		//taekwondoLeg.SetActive(true);
		legAnim.SetBool("Flying_Kick", true);
		GetComponent<CapsuleCollider>().isTrigger = true;
		characterController.enabled = false;
		flyKicking = true;
		move.camera.transform.rotation = Quaternion.Euler(0, move.rotX, 0);
		move.cameraIsLocked = !move.cameraIsLocked;
		move.isSprinting = true;
		GetComponent<CapsuleCollider>().radius = 0.5f;		
	}
	void FlyingKickStop()
	{
		//taekwondoLeg.SetActive(false);
		legAnim.SetBool("Flying_Kick", false);
		flyKicking = false;
		move.cameraIsLocked = false;
		move.isSprinting = false;
		GetComponent<CapsuleCollider>().isTrigger = false;
		characterController.enabled = true;
		GetComponent<CapsuleCollider>().radius = 0.3f;
	}

    private void OnTriggerEnter(Collider hit)
    {
        if (!hasAuthority) return;

        switch (hit.gameObject.tag)
        {
            case "Zombie":
                shoot.DamageEnemy(flyingKickDamage, hit.gameObject, false);
                break;

            case "Head":
                break;

            case "Torso":
                break;

            case "Arm":
                break;

            case "Leg":
                break;

            default:
                FlyingKickStop();
                break;
        }
    }
	public void Kick()
	{
		Vector3 camPos = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z);
		if (Physics.Raycast(camPos, camera.transform.forward, out hit, 3, ~ignoreOnRaycast))
		{
			Debug.Log("Hit : " + hit.collider.transform.root.name + "'s " + hit.collider.name + " | Tag : " + hit.collider.tag);
			switch (hit.collider.tag)
			{
				case "Head":
					shoot.DamageEnemy(kickDamage * 2, hit.collider.gameObject, true);
					shoot.Blood(hit.point, hit.normal);
					break;

				case "Torso":
					shoot.DamageEnemy(kickDamage, hit.collider.gameObject, true);
					shoot.Blood(hit.point, hit.normal);
					break;

				case "Arm":
					shoot.DamageEnemy(kickDamage / 2, hit.collider.gameObject, true);
					shoot.Blood(hit.point, hit.normal);
					break;

				case "Leg":
					shoot.DamageEnemy(kickDamage / 3, hit.collider.gameObject, true);
					shoot.Blood(hit.point, hit.normal);
					break;
			}
		}
	}
}
