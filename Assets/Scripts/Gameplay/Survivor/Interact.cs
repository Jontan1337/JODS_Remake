using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Interact : NetworkBehaviour
{
	private new Camera camera;
	private Shoot shoot;
    //private UseItem useItemScript;
	private Vector3 camPos;
    private Movement move;
    private WeaponStats weaponStats;

	public Text pickUpText;
	private PlayerUnit pUnit;
	public Stats stats;
	private UseItem useItemScript;
    //private Medkit medkit;

    [Header("Audio")]
    private AudioSource AS;
    public AudioClip[] pickupSounds;

	private void Start()
	{		
		shoot = GetComponent<Shoot>();
		useItemScript = GetComponent<UseItem>();
		camera = shoot.camera;
		pUnit = GetComponent<PlayerUnit>();
		stats = GetComponent<Stats>();
		//medkit = GetComponent<Medkit>();
        move = GetComponent<Movement>();
        AS = GetComponent<AudioSource>();
	}

	void Update()
	{
        if (!hasAuthority) return;

		//Debug.DrawRay(camPos, camera.transform.forward, Color.red);

		//-----INTERACT RAY---- -
		RaycastHit hit;
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (Physics.Raycast(camPos, camera.transform.forward, out hit, 3f, shoot.raycastLayerIgnore))
			{
                switch (hit.collider.tag)
                {
                    case "Door":
                        CmdOpenDoor(hit.collider.transform.parent.gameObject, transform.forward.x, transform.forward.z, false);
                        break;

                    case "Weapon":
					    if (hit.collider.GetComponent<WeaponType>().pickUp && !stats.isReloading)
					    {
						    if (shoot.hasWeapon)
						    {
							    Cmd_CreatePickup(hit.collider.gameObject, shoot.weaponType.name);
						    }
						    shoot.NewWeapon(hit.collider.gameObject);
                            PlayPickupSound();
					    }
                        break;

                    case "Item":
                        if (useItemScript.hasItem)
                        {
                            Cmd_GetNewItem(hit.collider.gameObject);
                        }
                        else
                        {
                            useItemScript.NewItem(hit.collider.gameObject);
                        }
                        PlayPickupSound();
                        break;

                    case "Healing":
						CmdHeal(hit.collider.GetComponent<Medkit>().UseMedkit(stats.HealthPoints));
                        PlayPickupSound();
                        break;

                    case "Flashlight":
                        var fl = GetComponent<Flashlight>();
                        if (!fl.hasFlashlight)
                        {
                            fl.hasFlashlight = true;
                            CmdDestroyObject(hit.collider.gameObject);
                            PlayPickupSound();
                        }
                        break;

                    case "Ammo":
                        if (shoot.hasWeapon)
                        {
                            if (shoot.weaponType.weaponType == WeaponType.Type.shotgun)
                            {
                                CmdPickUpAmmo(1, hit.collider.gameObject);
                            }
                            if (shoot.weaponType.weaponType == WeaponType.Type.rifle)
                            {
                                CmdPickUpAmmo(2, hit.collider.gameObject);
                            }
                            if (shoot.weaponType.weaponType == WeaponType.Type.pistol)
                            {
                                CmdPickUpAmmo(3, hit.collider.gameObject);
                            }
                            PlayPickupSound();
                        }
                        break;
                    case "Armour":
                        CmdPickUpArmour(Random.Range(20, 50), hit.collider.gameObject);
                        PlayPickupSound();
                        break;
                }
            }
            if (Physics.Raycast(camPos, camera.transform.forward, out hit, 20f, shoot.raycastLayerIgnore))
            {
                switch (hit.collider.tag)
                {
                    case "Player":
                        pickUpText.text = hit.collider.gameObject.GetComponent<PlayerUnit>().playerName;
                        break;
                    default: break;
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (shoot.hasWeapon && !shoot.weapon.GetComponent<WeaponStats>().isStarterWeapon)
            {
                //Reloader når man dropper. Fjern stats.CmdReloadFinished(); for fix, men ifølge Jonathan kommer der en anden fejl.
                //stats.CmdReloadFinished();
                CmdDropWeapon(shoot.weaponType.name);
                //shoot.aimDownSights.ResetAimSettings();
            }
        }
	}
	private void FixedUpdate()
	{
        if (hasAuthority)
        {
            camPos = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z);
            //-----CONSTANT RAY-----
            RaycastHit hit;
            if (Physics.Raycast(camPos, camera.transform.forward, out hit, 3f, shoot.raycastLayerIgnore))
            {
                switch (hit.collider.tag)
                {
                    case "Weapon":
                        string weaponName = hit.collider.name.Split('_')[1];
                        weaponName = weaponName.Replace("(Clone)", "");
                        pickUpText.text = "Press E to pick up " + weaponName;
                        break;
                    case "Door":
                        var door = hit.collider.transform.parent.gameObject.GetComponent<Door>();
                        pickUpText.text = door.locked ? "Door is locked" : "Press E to open door";
                        if (!door.locked) 
                        { 
                            if (door.pushDoor) 
                            {
                                if (Vector3.Distance(transform.position, door.transform.position) < 2)
                                {
                                    if (move.walking)
                                    {
                                        CmdOpenDoor(door.gameObject, transform.forward.x, transform.forward.z, true);
                                    }
                                }
                            } 
                        }
                        break;
                    case "Item": pickUpText.text = "Press E to pick up " + hit.collider.GetComponent<Item>().itemName; break;
                    case "Healing": pickUpText.text = "Press E to heal"; break;
                    case "Flashlight": pickUpText.text = "Press E to pick up Flashlight"; break;
                    case "Ammo": pickUpText.text = "Press E to pick up Ammo"; break;
                    case "Armour": pickUpText.text = "Press E to pick up Armour"; break;
                    default: pickUpText.text = ""; break;
                }
            }
            else
            {
                pickUpText.text = "";
            }
        }
	}

    [Command]
    private void Cmd_GetNewItem(GameObject newItem)
    {
        if (useItemScript.hasItem)
        {
            GameObject spawnItem = Instantiate(Resources.Load<GameObject>("Spawnables/Pickup/Pickup_" + useItemScript.currentItem.GetComponent<Item>().itemName), newItem.transform);
            NetworkServer.Spawn(spawnItem);
            spawnItem.transform.parent = null;
            Rpc_GetNewItem(connectionToClient, newItem);
        }
    }
    [TargetRpc]
    private void Rpc_GetNewItem(NetworkConnection target, GameObject newItem)
    {
        useItemScript.NewItem(newItem);
    }

	[Command]
	void CmdOpenDoor(GameObject door, float forwardx, float forwardz, bool push)
	{
        if (push)
        {
            door.GetComponent<Door>().RpcPush(forwardx, forwardz);
        }
        else
        {
            door.GetComponent<Door>().RpcOpen(forwardx, forwardz);
        }
    }

    [Command]
    void CmdDropWeapon(string name)
    {
        var weapon = Resources.Load<GameObject>("Spawnables/Pickup/Pickup_" + name);
        GameObject spawnWeapon = Instantiate(weapon, camera.transform.position + camera.transform.forward, Quaternion.identity);
        spawnWeapon.transform.parent = null;
        NetworkServer.Spawn(spawnWeapon);
        // Send current weapon ammunition and magazine for the new pickup weapon.
        Svr_PlaceWeapon(shoot.weaponStats.ammunition, shoot.weaponStats.magazineCurrent, spawnWeapon);
        RpcDropWeapon();
    }

    [ClientRpc]
    void RpcDropWeapon()
    {
        //reset weapon
        Destroy(shoot.weapon);
        shoot.weaponStats = null;
        shoot.weaponType = null;
        shoot.hasWeapon = false;
    }

	[Command]
	void Cmd_CreatePickup(GameObject newWeaponPickup, string name)
	{
        // If current weapon is start weapon, don't create a pickup.
		if (newWeaponPickup && !shoot.weapon.GetComponent<WeaponStats>().isStarterWeapon)
		{
			var weapon = Resources.Load<GameObject>("Spawnables/Pickup/Pickup_" + name);
            print(name);
            print(weapon);
			GameObject spawnWeapon = Instantiate(weapon, newWeaponPickup.transform);
			spawnWeapon.transform.parent = null;
			NetworkServer.Spawn(spawnWeapon);
			Svr_PlaceWeapon(shoot.weaponStats.ammunition, shoot.weaponStats.magazineCurrent, spawnWeapon);
			Destroy(newWeaponPickup);
		}
	}

	[Server]
	void Svr_PlaceWeapon(int ammo, int mag, GameObject weapon)
	{
		weapon.GetComponent<WeaponStats>().ammunition = ammo;
		weapon.GetComponent<WeaponStats>().magazineCurrent = mag;
	}

    [Command]
    void CmdHeal(float amount)
    {
        RpcHeal(amount);
    }

    [ClientRpc]
    void RpcHeal(float amount)
    {
        stats.HealthPoints = amount;
    }

    [Command]
    void CmdPickUpArmour(float amount, GameObject vest)
    {
        NetworkServer.Destroy(vest);
        RpcPickUpArmour(amount);
    }

    [ClientRpc]
    void RpcPickUpArmour(float amount)
    {
        stats.Armor += amount;
    }

    [Command]
    void CmdDestroyObject(GameObject obj)
    {
        NetworkServer.Destroy(obj);
    }


    /// <summary>
    /// What ammo ? 1 = Shotgun, 2 = Rifle, 3 = Pistol
    /// </summary>
    /// <param name="which"></param>
    [Command]
    void CmdPickUpAmmo(int which, GameObject ammo)
    {
        ammo.GetComponent<Ammo>().RpcPickUpAmmo(which, gameObject);
    }






    private void PlayPickupSound()
    {
        AS.volume = 0.8f;
        AS.pitch = Random.Range(0.9f, 1.1f);
        //AS.PlayOneShot(pickupSounds[Random.Range(0, pickupSounds.Length)]);
    }
}
