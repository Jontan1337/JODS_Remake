using System.Collections;
using UnityEngine;
using Mirror;

public class SoldierClass : NetworkBehaviour
{
	public new Camera camera;
	public GameObject launcher;
	public GameObject rocket;
	public GameObject rocketSpawnPosition;
	public GameObject launcherPosition;
	public GameObject soldierWeapon;
	public GameObject armPivot;
	private Shoot shoot;
    private Stats stats;
	private Animator anim;


	public int rocketSpeed;
	float cooldownRemaining = 0;


    public override void OnStartAuthority()
    {
		anim = GetComponent<Animator>();
		shoot = GetComponent<Shoot>();
        stats = GetComponent<Stats>();
        cooldownRemaining = stats.maxCooldown;
    }

    void Update()
	{
		if (!hasAuthority) return;
        if (!shoot.hasWeapon)
        {
            StarterWeapon();
        }
        //Vector3 camPos = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z);
        stats.SpecialCooldown = cooldownRemaining;
		if ((Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.T)) && cooldownRemaining >= stats.maxCooldown)
		{
			anim.SetTrigger("Launch Rocket");
			cooldownRemaining = 0;
		}
		if (cooldownRemaining < stats.maxCooldown)
		{
			cooldownRemaining += Time.deltaTime;
		}
	}

    void StarterWeapon()
	{
        //shoot.hasWeapon = false;
		shoot.StarterWeapon(soldierWeapon.name);
	}

	// Called in rocket launcher animation
	public void DeactivateArms()
	{
		shoot.enabled = false;
		armPivot.SetActive(false);
	}
	// Called in rocket launcher animation
	public void ActivateArms()
	{
		shoot.enabled = true;
		armPivot.SetActive(true);
	}



	//------------------------Rocket--------------------------
	void RocketLaunch()
	{
        Vector3 spawnpos = new Vector3(rocketSpawnPosition.transform.position.x, rocketSpawnPosition.transform.position.y, rocketSpawnPosition.transform.position.z);
        CmdRocketLaunch(spawnpos, camera.transform.forward);
	}

	[Command]
	void CmdRocketLaunch(Vector3 spawnpos, Vector3 forw)
	{
        /*
        GameObject currentRocket = Instantiate(rocket, spawnpos, transform.rotation);
        Rigidbody rb = currentRocket.GetComponent<Rigidbody>();
        rb.AddForce(forw * rocketSpeed);
        rb.useGravity = false;
        NetworkServer.Spawn(currentRocket);
        */
        GameObject currentRocket = Instantiate(rocket, spawnpos, transform.rotation);
        NetworkServer.Spawn(currentRocket);
        RpcRocketLaunch(currentRocket, forw);
	}

	[ClientRpc]
	void RpcRocketLaunch(GameObject currentRocket, Vector3 forw)
	{
        Rigidbody rb = currentRocket.GetComponent<Rigidbody>();
        rb.AddForce(forw * rocketSpeed);
        rb.useGravity = false;
        //GameObject currentRocket = Instantiate(rocket, new Vector3(rocketSpawnPosition.transform.position.x, rocketSpawnPosition.transform.position.y, rocketSpawnPosition.transform.position.z), transform.rotation);
		//currentRocket.GetComponent<Rigidbody>().AddForce(camera.transform.forward * rocketSpeed);
		//currentRocket.GetComponent<Rigidbody>().useGravity = false;
		//CmdSpawnRocket(currentRocket);
	}

	[Command]
	void CmdSpawnRocket(GameObject currentRocket)
	{
		NetworkServer.Spawn(currentRocket);
	}
}
