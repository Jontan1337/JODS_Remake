using Mirror;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;



public class AutoTurret : NetworkBehaviour, IDamagable, IPlaceable
{
	[Header("Stats")]
	[SerializeField] private float range = 30;
	[SerializeField] private float fireRate = 50f;
	[SerializeField] private float rotateSpeed = 0.4f;
	[SerializeField] private float searchInterval = 1f;
	[SerializeField] private int damage = 20;
	[SerializeField, SyncVar] private int duration = 20;
	[SerializeField, SyncVar] private int health = 200;


	[Header("References")]
	[SerializeField] private Transform swivel = null;
	[SerializeField] private Transform pivot = null;
	[SerializeField] private Transform barrel = null;
	[SerializeField] private Transform cylinder = null;
	[SerializeField] private ParticleSystem muzzleFlash = null;
	[SerializeField] private ParticleSystem bulletShell = null;
	[SerializeField] private GameObject Laser;
	[SerializeField] private GameObject turretSmoke = null;
	[SerializeField, SyncVar] private Transform target = null;

	[Space]
	[SerializeField] private LayerMask unitLayer = 0;
	[SerializeField] private LayerMask LOSLayer = 0;

	private List<Collider> enemiesInSight = new List<Collider>();
	[SyncVar] private bool isDead;


	#region Coroutines
	// The turret tries to shoot at a fixed interval. The value of fireRate should be the desired rounds per minute (RPM).
	IEnumerator ShootIntervalCo;
	IEnumerator ShootInterval()
	{
		while (true)
		{
			TryShoot();
			yield return new WaitForSeconds(1 / (fireRate / 60));
		}
	}
	// The turret tries to find a target at a fixed interval
	IEnumerator SearchingCo;
	IEnumerator Searching()
	{
		while (true)
		{
			Svr_FindTarget();
			yield return new WaitForSeconds(searchInterval);
		}

	}

	// The RotateY coroutine controls the swivel, making it look left or right at the target.
	IEnumerator RotateYCo;
	IEnumerator RotateY()
	{
		while (true)
		{
			Quaternion lookRotation = Quaternion.LookRotation((target.position - swivel.position));
			swivel.rotation = Quaternion.RotateTowards(swivel.rotation, lookRotation, rotateSpeed);
			swivel.localEulerAngles = new Vector3(0, swivel.localEulerAngles.y, 0);
			yield return null;
		}
	}

	// The RotateX coroutine controls the pivot, making it look up or down at the target.
	IEnumerator RotateXCo;
	IEnumerator RotateX()
	{
		while (true)
		{
			Quaternion lookRotation = Quaternion.LookRotation(((target.position + target.GetComponent<BoxCollider>().center * 1.5f) - pivot.position));
			pivot.rotation = Quaternion.RotateTowards(pivot.rotation, lookRotation, rotateSpeed);
			pivot.localEulerAngles = new Vector3(pivot.localEulerAngles.x, 0, 0);
			yield return null;
		}
	}
	// Rotates the turret around at a fixed speed
	IEnumerator RotatePassiveCo;
	IEnumerator RotatePassive()
	{
		pivot.localEulerAngles = new Vector3(0, 0, 0);
		while (true)
		{
			swivel.Rotate(0, 1, 0);
			yield return new WaitForSeconds(0.025f);
		}
	}

	// Turret lifetime. Dies when time runs out.
	IEnumerator Duration()
	{
		while (duration > 0)
		{
			duration--;

			yield return new WaitForSeconds(1);
		}
		Svr_Die();
	}

	// Shooting animation. Played once per shot.
	bool barrelAnimation = false;
	IEnumerator BarrelCo;
	IEnumerator BarrelAnimation()
	{
		barrelAnimation = true;
		// Barrel standard position and post-shooting position is saved.
		Vector3 ogPosition = new Vector3(0, barrel.transform.localPosition.y, 0.3f);
		barrel.transform.localPosition = new Vector3(0, barrel.transform.localPosition.y, 0.2f);

		// Barrel position is incremently increased as long as its not in its standard position and its pointing at a damagable target.
		while (barrel.transform.localPosition.z < ogPosition.z)
		{
			yield return new WaitForSeconds(0.01f);
			barrel.transform.localPosition = new Vector3(0, barrel.transform.localPosition.y, barrel.transform.localPosition.z + 0.005f);
		}
	}

	IEnumerator StartUp()
	{
		float time = 0;
		float duration = 1;
		Vector3 startRot = new Vector3(50, 0 ,0);
		Vector3 targetRot = new Vector3(0, 0, 0);
		while (time < duration)
		{
			time += Time.deltaTime/duration;
			TurretStartUp(startRot, targetRot, time);
			//pivot.localRotation = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(targetRot), time);
			yield return null;
		}
		yield return new WaitForSeconds(0.2f);
		ShowLaser();
		yield return new WaitForSeconds(0.4f);
		Svr_StartSearching();
		StartCoroutine(Duration());
	}
	[ClientRpc]
	private void TurretStartUp(Vector3 startRot, Vector3 targetRot, float time)
	{
		pivot.localRotation = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(targetRot), time);
	}
	[ClientRpc]
	private void ShowLaser()
	{
		Laser.SetActive(true);
	}

	#endregion

	#region Methods

	[Server]
	private void Svr_Shoot(RaycastHit didHit)
	{
		Debug.DrawRay(barrel.position, barrel.forward * 10, Color.red, 0.1f);

		// A shooting animation coroutine is played.
		Rpc_Shoot();
		PhysicMaterial phyMat = didHit.collider.sharedMaterial;
		Rpc_Bullethole(didHit.point, didHit.normal, phyMat ? phyMat.name : "");
		Rpc_BulletTrail(didHit.point);
		// The turret uses a raycast to check if a damagable unit is in front of its barrel.
		// The turret will shoot at any unit that can be damaged, even if it's not the target.
		didHit.transform.GetComponent<IDamagable>()?.Svr_Damage(damage, Owner);

		// If the target is dead after the shot, the target is lost.
		if (target.GetComponent<IDamagable>().IsDead)
		{
			Svr_LostTarget();
		}
	}
	[ClientRpc]
	private void Rpc_Shoot()
	{
		BarrelCo = BarrelAnimation();
		if (barrelAnimation)
		{
			StopCoroutine(BarrelCo);
			barrelAnimation = false;
		}
		StartCoroutine(BarrelCo);
		muzzleFlash.Emit(50);
		bulletShell.Emit(1);

	}
	[ClientRpc]
	private void Rpc_Bullethole(Vector3 point, Vector3 normal, string phyMatName)
	{
		if (GlobalVariables.hallo.TryGetValue(phyMatName, out Tags fxTag))
		{
			GameObject bulletHole = ObjectPool.Instance.SpawnFromLocalPool(fxTag, point + normal * 0.01f, Quaternion.identity, 5);
			bulletHole.transform.LookAt(point + normal);
		}
	}
	[ClientRpc]
	private void Rpc_BulletTrail(Vector3 direction)
	{
		GameObject fx = ObjectPool.Instance.SpawnFromLocalPool(Tags.BulletTrail, barrel.position, Quaternion.identity, 1);
		fx.transform.forward = direction - barrel.position;
	}

	[Server]
	private void Svr_FindTarget()
	{
		enemiesInSight.Clear();
		// The turret checks for units in a sphere around it, and adds the found units to a list.
		Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, range, unitLayer);
		foreach (Collider item in enemiesInRange)
		{
			Debug.DrawLine(swivel.transform.position, (item.transform.position + item.GetComponent<BoxCollider>().center), Color.blue, 0.2f);

			// For each unit in range, it raycasts to the unit to check if there is a structure blocking its line of sight.
			Physics.Raycast(swivel.transform.position, ((item.transform.position + item.GetComponent<BoxCollider>().center) - transform.position), out RaycastHit hit, LOSLayer);

			// Every unit that is in line of sight will be added to a new list.
			if (hit.transform)
			{
				if (hit.transform.root == item.transform)
				{
					enemiesInSight.Add(item);
					print(item);
				}
			}
		}
		// The turret uses the list of enemies in sight to find a target.
		if (GetClosestEnemyCollider(enemiesInSight))
		{
			Svr_NewTarget(GetClosestEnemyCollider(enemiesInSight).transform);
		}
	}

	// When a target is found, the searching and passive rotation coroutines are stopped.
	// The coroutines that makes the turret look at the target is started, and the turret attempts to shoot at the target if able.
	[ClientRpc]
	private void Rpc_NewTarget(Transform newTarget)
	{
		target = newTarget;

		StopCoroutine(RotatePassiveCo);

		RotateYCo = RotateY();
		RotateXCo = RotateX();


		StartCoroutine(RotateXCo);
		StartCoroutine(RotateYCo);
	}
	[Server]
	private void Svr_NewTarget(Transform newTarget)
	{
		target = newTarget;
		Rpc_NewTarget(newTarget);

		StopCoroutine(SearchingCo);
		ShootIntervalCo = ShootInterval();
		StartCoroutine(ShootIntervalCo);

	}


	// Checks a list of colliders to see which one is closest to the turret.
	Collider GetClosestEnemyCollider(List<Collider> enemyColliders)
	{
		float currentClosestDitance = 99999f;
		Collider currentClosestEnemy = null;

		foreach (Collider enemy in enemyColliders)
		{
			float distance = Vector3.Distance(transform.position, enemy.transform.position);

			if (distance < currentClosestDitance)
			{
				currentClosestDitance = distance;
				currentClosestEnemy = enemy;
			}
		}
		return currentClosestEnemy;
	}

	// When the target is lost, the active coroutines are stopped and the turret starts searching again.
	[Server]
	private void Svr_LostTarget()
	{
		target = null;

		Rpc_LostTarget();
		StopCoroutine(ShootIntervalCo);

		Svr_StartSearching();


	}

	[ClientRpc]
	private void Rpc_LostTarget()
	{
		if (barrelAnimation)
		{
			StopCoroutine(BarrelCo);
			barrelAnimation = false;
		}
		StopCoroutine(RotateYCo);
		StopCoroutine(RotateXCo);
	}

	// Starts the passive rotation and searching coroutine
	[Server]
	private void Svr_StartSearching()
	{
		//Rpc_StartRotating();

		//SearchingCo = Searching();
		//StartCoroutine(SearchingCo);
		StartCoroutine(Wait());

	}
	IEnumerator Wait()
	{
		yield return new WaitForSeconds(0.1f);
		Rpc_StartRotating();

		SearchingCo = Searching();
		StartCoroutine(SearchingCo);
	}

	[ClientRpc]
	private void Rpc_StartRotating()
	{
		RotatePassiveCo = RotatePassive();
		StartCoroutine(RotatePassiveCo);
	}



	// Returns true if the transform that is hit by the raycast is damagable otherwise returns false.
	// If its not damagable, checks line of sight to the target. If it's not in sight, the target is lost.
	// This is to make sure that the turret doesn't lose its target just because it isn't currently hitting it or pointing at it.
	// Even if another unit is standing in front of the target, the turret will still try to hit the target, damaging the unit in front of it instead.
	[Server]
	private bool TryShoot()
	{
		Ray(out RaycastHit didHit, out bool lineOfSightCheck);
		if (didHit.transform)
		{
			if (didHit.transform.TryGetComponent(out IDamagable a))
			{
				Svr_Shoot(didHit);
				return true;
			}
			else if (!lineOfSightCheck)
			{
				//Debug.LogError(didHit.transform.name);
				Svr_LostTarget();
			}
		}
		return false;
	}

	// Sends out two variables that can be used if the method is called.
	private void Ray(out RaycastHit didHit, out bool lineOfSightCheck)
	{
		// A bool that is determined by a raycast that checks if there is a straight line between the turret and the target, without any structures in between.
		Physics.Raycast(transform.position, ((target.position + target.GetComponent<BoxCollider>().center) - transform.position), out RaycastHit hitLOS, LOSLayer);
		lineOfSightCheck = hitLOS.transform.root == target;

		// A transform that is determined by a raycast that points forward relative to the pivot.
		Physics.Raycast(pivot.position, pivot.forward, out RaycastHit hit, range, LOSLayer);
		didHit = hit;
	}


	// Stops all coroutines and destroys the turret.
	[Server]
	private void Svr_Die()
	{
		StopAllCoroutines();

		Rpc_Die();
		PlaySmoke();
		NetworkServer.Destroy(gameObject);
	}

	[ClientRpc]
	private void Rpc_Die()
	{
		StopAllCoroutines();
		PlaySmoke();

	}

	private void PlaySmoke()
	{
		turretSmoke.transform.parent = null;
		ParticleSystem[] particleSystems = turretSmoke.GetComponentsInChildren<ParticleSystem>();
		foreach (var item in particleSystems)
		{
			item.Play();
		}
	}


	[ClientRpc]
	private void ShowTurret()
	{
		swivel.gameObject.SetActive(true);
		cylinder.gameObject.SetActive(true);
	}

	#endregion

	#region IDamagable

	public Teams Team => throw new System.NotImplementedException();

	// The turret loses health and dies if its health is 0 or less.
	[Server]
	public void Svr_Damage(int damage, Transform target = null)
	{
		if (IsDead) return;
		health -= damage;
		if (health <= 0)
		{
			isDead = true;
			Svr_Die();
		}
	}



    public int GetHealth => health;
	public bool IsDead => isDead;





	#endregion

	#region Placeable
	public Transform Owner { get; set; }

	// Invoked when the turret is put down on the ground.
	// Starts the searching coroutine, the time until the turret dies and the cooldown on the engineers ability.
	[Server]
	public void Svr_OnPlaced()
	{
		Owner.GetComponentInParent<ActiveSClass>()?.Rpc_StartAbilityCooldown(Owner.GetComponent<NetworkIdentity>().connectionToClient, Owner);
		ShowTurret();
		StartCoroutine(StartUp());
	}

    #endregion

    private void OnDrawGizmosSelected()
	{
		if (target)
		{
			//Gizmos.color = TryShoot() ? Color.green : Color.red;
			Gizmos.DrawSphere(new Vector3(target.position.x, target.position.y + 4, target.position.z), 1);
		}

		Gizmos.DrawWireSphere(transform.position, range);
	}
}
