using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Collections;

public class Shoot : NetworkBehaviour
{
	[Header("References")]
	public new Camera camera;
	public Transform[] weaponPos;
	public GameObject hands;
	[SyncVar] public bool hasWeapon;

	public GameObject weapon;
	public WeaponStats weaponStats;
	public WeaponType weaponType;
	public Camera[] Cameras;
	public bool isZooming;
    public SurviveTimer survivalTimer;
    public Image crossHairImage;
    public Sprite defaultCrossHairImage;
    public Image sniperCrossHairImage;
    public WeaponHandIK handIK;
    public AimDownSights aimDownSights;
    public WeaponRecoil weaponRecoil;
    public LayerMask raycastLayerIgnore;

	private Animator handsAnim;
	private PlayerUnit pUnit;
	private Movement move;
	private Stats stats;
    private GameObject hitObject;
    private Vector2 weaponCrossHairSize = new Vector2(5f, 5f);
    private GameObject thrownGrenade;

	[Header("Other")]
	public int damage = 100;
	private float currentZoom = 60f;
	private float shootInterval;
    private Vector3 camPos;

    private void Start()
	{
        // Ignore the player layer.
        raycastLayerIgnore = ~raycastLayerIgnore;

		move = GetComponent<Movement>();
		stats = GetComponent<Stats>();
		handsAnim = hands.GetComponent<Animator>();
		pUnit = GetComponent<PlayerUnit>();
        if (GameObject.FindGameObjectWithTag("SurvivalTimer"))
        {
            survivalTimer = GameObject.FindGameObjectWithTag("SurvivalTimer").GetComponent<SurviveTimer>();
        }
	}

	void Update()
	{
        if (!hasAuthority) return;

        camPos = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z);
        hands.SetActive(hasWeapon);
        crossHairImage.transform.localScale = weaponCrossHairSize;
        if (hasWeapon)
		{
			shootInterval -= Time.deltaTime;
			damage = weaponStats.damage;

			if (handsAnim != null && handsAnim.isActiveAndEnabled)
			{
				handsAnim.SetBool("Melee", weaponType.weaponType == WeaponType.Type.melee);
                handsAnim.SetBool("Shotgun", weaponType.weaponType == WeaponType.Type.shotgun);
                handsAnim.SetBool("AR", weaponType.weaponType == WeaponType.Type.rifle);
                handsAnim.SetBool("Knife", weaponType.weaponType == WeaponType.Type.knife);
            }

            //---------SHOOTING---------
			if (!move.isSprinting && !stats.isReloading || weaponType.weaponType == WeaponType.Type.melee || weaponType.weaponType == WeaponType.Type.knife)
			{
				if (!weaponStats.isAutomatic && Input.GetKeyDown(KeyCode.Mouse0) || weaponStats.isAutomatic && Input.GetKey(KeyCode.Mouse0))
				{
                    WeaponAttack();
				}
				if (weaponStats.isAutomatic && Input.GetKeyUp(KeyCode.Mouse0))
				{
					weaponStats.accuracy = weaponStats.minAccuracy;
				}

			}
		}
		else
		{
			weaponStats = null;
            crossHairImage.sprite = defaultCrossHairImage;
            Vector2 defaultSize = new Vector2(1f, 1f);
            crossHairImage.transform.localScale = defaultSize;
		}
        //Zoom
        if (hasWeapon)
		{
			if (!move.isSprinting && !stats.isReloading)
			{
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    if (weaponType.name == "M24")
                    {
                        sniperCrossHairImage.gameObject.SetActive(true);
                        crossHairImage.gameObject.SetActive(false);
                    }
                    currentZoom -= Time.deltaTime * 200;
                    isZooming = true;
                }
                else if (currentZoom < 60)
				{
                    sniperCrossHairImage.gameObject.SetActive(false);
                    crossHairImage.gameObject.SetActive(true);
                    currentZoom += Time.deltaTime * 150;
                    isZooming = false;
				}
				currentZoom = Mathf.Clamp(currentZoom, weaponStats.zoom, 60f);
				Cameras[0].fieldOfView = currentZoom;
				Cameras[1].fieldOfView = currentZoom;
			}
			else
			{
				if (currentZoom < 60f)
				{
					Cameras[0].fieldOfView = currentZoom;
					Cameras[1].fieldOfView = currentZoom;
					currentZoom += Time.deltaTime * 150;
					isZooming = false;
				}
			}
		}
	}
    public void WeaponAttack()
    {
        if (!hasAuthority) return;
        RaycastHit hit;
        if (weaponStats.magazineCurrent == 0)
        {
            stats.ReloadStart(weaponStats.reloadTime);
        }
        if (weaponStats.magazineCurrent > 0 && shootInterval <= 0)
        {
            shootInterval = weaponStats.fireRate;
            //handsAnim.SetTrigger("Shoot");
            weaponRecoil.Recoil();
            CmdShoot();
            for (int i = 0; i < weaponStats.multiShot; i++)
            {
                // Get random floats to simulate recoil when shooting.
                Vector3 accuracyDirection = camera.transform.forward;
                accuracyDirection.y += Random.Range(-weaponStats.accuracy, weaponStats.accuracy);
                accuracyDirection.x += Random.Range(-weaponStats.accuracy, weaponStats.accuracy);
                accuracyDirection.z += Random.Range(-weaponStats.accuracy, weaponStats.accuracy);

                if (weaponStats.isAutomatic && weaponStats.accuracy < 0.05)
                {
                    weaponStats.accuracy += Time.deltaTime / 10;
                }
                if (Physics.Raycast(camPos, accuracyDirection, out hit, weaponStats.weaponRange, raycastLayerIgnore))
                {
                    Collider tempCol = hit.collider;
                    switch (hit.collider.tag)
                    {
                        // ----- ZOMBIE PARTS START-----
                        case "Head":
                            DamageEnemy(damage * 2, hit.collider.gameObject, true);
                            if (hit.collider.gameObject.TryGetComponent(out IExplodable explodable))
                            {
                                explodable.Explode();
                            }
                            CmdBlood(hit.point, hit.normal);
                            break;

                        case "Torso":
                            DamageEnemy(damage, hit.collider.gameObject, true);

                            CmdBlood(hit.point, hit.normal);
                            break;

                        case "Arm":
                            DamageEnemy(damage / 2, hit.collider.gameObject, true);

                            CmdBlood(hit.point, hit.normal);
                            break;

                        case "Leg":
                            DamageEnemy(damage / 3, hit.collider.gameObject, true);

                            CmdBlood(hit.point, hit.normal);
                            break;
                        // ----- ZOMBIE PARTS END-----

                        case "LiveEntity":
                            hitObject = tempCol.gameObject;
                            if (hitObject.GetComponent<LiveEntity>().entityType == LiveEntity.EntityType.explosive)
                            {
                                CmdDamageEntity(hitObject, damage);
                            }
                            break;
                        case "Player":
                            CmdBlood(hit.point, hit.normal);
                            break;
                        default:
                            CmdBulletHole(hit.point, hit.normal);
                            break;
                    }
                }
            }
        }
        else if (weaponType.weaponType == WeaponType.Type.melee || weaponType.weaponType == WeaponType.Type.knife)
        {
            shootInterval = weaponStats.fireRate;
            // Shoot is actually the melee swing animation
            handsAnim.SetTrigger("Shoot");
        }
    }

    // Called by the melee swing animation
    public void MeleeAttack()
    {
        RaycastHit hit;
        if (Physics.Raycast(camPos, camera.transform.forward, out hit, weaponStats.weaponRange, raycastLayerIgnore))
        {
            Debug.Log("Hit : " + hit.collider.transform.root.name + "'s " + hit.collider.name + " | Tag : " + hit.collider.tag);

            CmdShoot();// ----- ZOMBIE PARTS -----
            switch (hit.collider.tag)
            {
                case "Head":
                    DamageEnemy(damage * 2, hit.collider.gameObject, true);

                    
                    CmdBlood(hit.point, hit.normal);
                    break;

                case "Torso":
                    DamageEnemy(damage, hit.collider.gameObject, true);

                    CmdBlood(hit.point, hit.normal);
                    break;

                case "Arm":
                    DamageEnemy(damage / 2, hit.collider.gameObject, true);

                    CmdBlood(hit.point, hit.normal);
                    break;

                case "Leg":
                    DamageEnemy(damage / 3, hit.collider.gameObject, true);

                    CmdBlood(hit.point, hit.normal);
                    break;
                default:
                    hitObject = hit.collider.gameObject;
                    if (hitObject.TryGetComponent(out IDamagable damagable))
                    {
                        CmdDamageEntity(hitObject, damage / 4);
                    }
                    break;
            }
            if (hit.collider.tag == "Zombie")
            {
                GameObject bodypart = hit.collider.gameObject;

                DamageEnemy(damage, bodypart, true);

                CmdBlood(hit.point, hit.normal);

            }
            else
            {
                if (weaponType.weaponType != WeaponType.Type.melee)
                {
                    if (hit.collider.tag == "Player")
                    {
                        CmdBlood(hit.point, hit.normal);
                    }
                    else
                    {
                        CmdBulletHole(hit.point, hit.normal);
                    }
                }
            }
        }
    }

	public void NewWeapon(GameObject weapon)
	{
		CmdNewWeapon(weapon, null);
		move.GetGun(true);
		CmdDestroyWeapon(weapon);
	}

    public void StarterWeapon(string weaponName)
    {
        CmdNewWeapon(null, weaponName);
        move.GetGun(true);
    }


	//----------COMMANDS----------
    //---------------------NEW WEAPON START---------------------------

    [Command]
	void CmdNewWeapon(GameObject newWeapon, string weaponName)
	{
        if (!newWeapon && weaponName == null) return;

        // If weaponName is not null, it means we should load new gameobject
        // instead of using newWeapon gameobject.
        if (weaponName != null)
        {
            newWeapon = Resources.Load<GameObject>("Spawnables/" + weaponName);
        }
        //Spawn weapon for myself
        WeaponType wt = newWeapon.GetComponent<WeaponType>();
		GameObject weaponObject = Instantiate(wt.weaponPrefab, weaponPos[0]);
        NetworkServer.Spawn(weaponObject);
        Svr_SetupNewWeaponValues(weaponObject, newWeapon.GetComponent<WeaponStats>().ammunition, newWeapon.GetComponent<WeaponStats>().magazineCurrent);
		RpcNewWeapon(weaponObject);
        this.hasWeapon = true;
    }

    [Server]
    private void Svr_SetupNewWeaponValues(GameObject newWeapon, int ammo, int mag)
    {
        // Set ammunition and magazine to new weapon ammunition and magizine values.
        newWeapon.GetComponent<WeaponStats>().ammunition = ammo;
        newWeapon.GetComponent<WeaponStats>().magazineCurrent = mag;
    }

    [ClientRpc]
	void RpcNewWeapon(GameObject newWeapon)
	{
        if (this.weapon != null)
        {
            // Destroy previous weapon and reset stats and type.
            Destroy(this.weapon);
            this.weaponStats = null;
            this.weaponType = null;

            //Part of Spawn weapon for others
            //Destroy(this.weaponPos[1].transform.GetChild(0).gameObject);
        }
        if (hasAuthority && isLocalPlayer)
        {
            //Spawn weapon for myself
            this.weapon = newWeapon;
            //print($"My new weapon {weapon}");
            this.weaponStats = newWeapon.GetComponent<WeaponStats>();
            this.weaponType = newWeapon.GetComponent<WeaponType>();

            this.weapon.transform.parent = weaponPos[0];
            this.weapon.transform.localPosition = Vector3.zero;
            this.weapon.transform.localRotation = Quaternion.identity;
            this.weapon.transform.GetChild(0).gameObject.layer = 10;

            crossHairImage.sprite = weaponType.crossHair;

            // Set vector and rotation recoil values.
            weaponRecoil.recoilAmountV = weaponStats.recoilAmountV;
            weaponRecoil.recoilAmountR = weaponStats.recoilAmountR;

            // Commented out because handIK is not used
            //if (handIK)
            //{
            //    handIK.SetWeaponPoints(weaponType.rightHandTarget, weaponType.leftHandTarget);
            //}

            //aimDownSights.SetAimSettings(weaponType.sightsPointRef);
            if (this.weapon.GetComponent<WeaponType>().weaponType == WeaponType.Type.shotgun)
            {
                // the hell is going on with this code...
                // this shouldn't be necessary.
                this.weapon.transform.Rotate(new Vector3(-5, 0, -25));
            }
            if (this.weapon.GetComponent<WeaponType>().weaponType == WeaponType.Type.rifle)
            {

            }
        }
        else
        {
            //Spawn weapon for others 
            this.weapon = newWeapon;
            this.weaponStats = newWeapon.GetComponent<WeaponStats>();
            this.weaponType = newWeapon.GetComponent<WeaponType>();
            this.weapon.transform.parent = weaponPos[1];
            this.weapon.transform.localPosition = Vector3.zero;
            this.weapon.transform.localRotation = Quaternion.identity;
        }
    }

	//---------------------NEW WEAPON END---------------------------


	//---------------------DESTROY WEAPON---------------------------
	[Command]
	void CmdDestroyWeapon(GameObject weapon)
	{
		NetworkServer.Destroy(weapon);
	}

    //---------------------DISABLE ENEMY START---------------------------

    /// <summary>
    /// Damage is amount of damage, enemy is the enemy taking damage, part is if you hit a bodypart or the whole enemy (true = bodypart), explosion?
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="enemy"></param>
    /// <param name="part"></param>
    /// <param name="explosion"></param>
    public void DamageEnemy(int damage, GameObject enemy, bool part)
    {
        if (part)
        {
            enemy = enemy.transform.root.gameObject;
        }
        CmdDamageEnemy(damage, enemy);
    }

    [Command]
    void CmdDamageEnemy(int damage, GameObject enemy)
    {
        enemy.GetComponent<IDamagable>()?.Svr_Damage(damage);
        if (survivalTimer)
        {
            if (!survivalTimer.gameStarted)
            {
                survivalTimer.StartGame();
            }
        }
    }

	//---------------------DISABLE ENEMY END---------------------------


	//---------------------FX START---------------------------
	//---------------------BLOOD---------------------------
    public void Blood(Vector3 hit, Vector3 rot)
    {
        CmdBlood(hit, rot);
    }

	[Command]
	public void CmdBlood(Vector3 hit, Vector3 rot)
	{
		var blood = (GameObject)Resources.Load("Spawnables/FX/BloodSplatter1");
		blood = Instantiate(blood, hit, Quaternion.LookRotation(rot));
		NetworkServer.Spawn(blood);
        StartCoroutine(DestroyFX(1f, blood));
    }
	//---------------------BULLET HOLE---------------------------
	[Command]
	void CmdBulletHole(Vector3 hit, Vector3 rot)
	{
		var hole = (GameObject)Resources.Load("Spawnables/FX/BulletHole");
		hole = Instantiate(hole, hit, Quaternion.LookRotation(rot));
        //random rotation, virker ikke, har ikke rigtig prøvet. RE: Hvaaa... er det dovenskab jeg ser her??
        //hole.transform.rotation = new Quaternion(Random.Range(0, 360), hole.transform.rotation.y, hole.transform.rotation.z, 0);
		NetworkServer.Spawn(hole);
        StartCoroutine(DestroyFX(1f,hole));
	}
    
    IEnumerator DestroyFX(float tiem, GameObject go)
    {
        yield return new WaitForSeconds(tiem);

        NetworkServer.Destroy(go);
    }

    //---------------------FX END---------------------------


    //---------------------SHOOT START---------------------------

    [Command]
	void CmdShoot()
	{
        //REMOVE COMMENT WHEN SMOEK LOOKS GOOD. RE: SMOKE NEEDS SNOOP CERTIFICATE.
        //if (weaponType.smoke) { weaponType.smoke.Play(); }

        RpcShoot();

        if (weaponStats != null)
        {
            if (weaponStats.magazineCurrent >= 1)
            {
                weaponStats.magazineCurrent -= 1;
            }
        }
	}

    [ClientRpc]
    void RpcShoot()
    {
        var flash = weaponType.muzzleFlash;
        if (flash.GetComponent<SFXPlayer>())
        {
            flash.GetComponent<SFXPlayer>().PlaySFX();
        }
        if (flash.GetComponent<ParticleSystem>())
        {
            flash.GetComponent<ParticleSystem>().Emit(15);
        }
    }

    //---------------------SHOOT END---------------------------

    [Command]
    public void CmdDamageEntity(GameObject sendEntity, int damage)
    {
        sendEntity.GetComponent<IDamagable>()?.Svr_Damage(damage);
    }
}
