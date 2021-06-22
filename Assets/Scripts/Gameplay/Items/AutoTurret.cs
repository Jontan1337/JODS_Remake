using Mirror;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;



public class AutoTurret : NetworkBehaviour, IDamagable
{
	[Header("Stats")]
	[SerializeField] private float range = 30;
	[SerializeField] private float fireRate = 50f;
	[SerializeField] private float rotateSpeed = 0.2f;
	[SerializeField] private float searchInterval = 1f;
	[SerializeField] private int damage = 20;
	[SerializeField, SyncVar] private int duration = 20;
	[SerializeField, SyncVar] private int health = 200;


	[Header("References")]
	[SerializeField] private Transform swivel = null;
	[SerializeField] private Transform pivot = null;
	[SerializeField] private Transform barrel = null;
	[SerializeField] private ParticleSystem muzzleFlash = null;
	[SerializeField] private ParticleSystem bulletShell = null;
	[SerializeField] private GameObject turretSmoke = null;
	[SerializeField, SyncVar] private Transform target = null;

	[Space]
	[SerializeField] private LayerMask unitLayer;
	[SerializeField] private LayerMask LOSLayer;

	private List<Collider> enemiesInSight = new List<Collider>();
	[SyncVar] private bool isDead;

	// The turret tries to shoot at a fixed interval. The value of fireRate should be the desired rounds per minute (RPM).
	#region Coroutines
	IEnumerator ShootIntervalCo;
	IEnumerator ShootInterval()
	{
		while (true)
		{
			if (CanShoot()) Svr_Shoot();
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
	IEnumerator BarrelAnimation(Transform hit)
	{
		barrelAnimation = true;
		// Barrel standard position and post-shooting position is saved.
		Vector3 ogPosition = new Vector3(0, barrel.transform.localPosition.y, 0.3f);
		barrel.transform.localPosition = new Vector3(0, barrel.transform.localPosition.y, 0.2f);

		// Barrel position is incremently increased as long as its not in its standard position and its pointing at a damagable target.
		while (barrel.transform.localPosition != ogPosition && hit.TryGetComponent(out IDamagable a))
		{
			yield return new WaitForSeconds(0.01f);
			barrel.transform.localPosition = new Vector3(0, barrel.transform.localPosition.y, barrel.transform.localPosition.z + 0.005f);
		}
	}

	#endregion

	#region Methods

	[Server]
	void Svr_Shoot()
	{
		Ray(out Transform didHit, out bool lineOfSightCheck);
		Debug.DrawRay(barrel.position, barrel.forward * 10, Color.red, 0.1f);

		// A shooting animation coroutine is played.
		BarrelCo = BarrelAnimation(didHit);
		if (barrelAnimation)
		{
			StopCoroutine(BarrelCo);
		}
		StartCoroutine(BarrelCo);
		muzzleFlash.Emit(50);
		bulletShell.Emit(1);

		// The turret uses a raycast to check if a damagable unit is in front of its barrel.
		// The turret will shoot at any unit that can be damaged, even if it's not the target.
		didHit.GetComponent<IDamagable>()?.Svr_Damage(damage, transform);

		// If the target is dead after the shot, the target is lost.
		if (target.GetComponent<IDamagable>().IsDead())
		{
			Svr_LostTarget();
		}
	}



	[Server]
	void Svr_FindTarget()
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
	[Server]
	void Svr_NewTarget(Transform newTarget)
	{
		target = newTarget;

		StopCoroutine(SearchingCo);
		StopCoroutine(RotatePassiveCo);

		RotateYCo = RotateY();
		RotateXCo = RotateX();
		ShootIntervalCo = ShootInterval();

		StartCoroutine(RotateXCo);
		StartCoroutine(RotateYCo);
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
	void Svr_LostTarget()
	{
		target = null;

		StopCoroutine(BarrelCo);
		StopCoroutine(RotateYCo);
		StopCoroutine(RotateXCo);
		StopCoroutine(ShootIntervalCo);
		Svr_StartSearching();
	}

	// Starts the passive rotation and searching coroutine
	[Server]
	void Svr_StartSearching()
	{
		RotatePassiveCo = RotatePassive();
		StartCoroutine(RotatePassiveCo);

		SearchingCo = Searching();
		StartCoroutine(SearchingCo);

		//TO DO - WHATEVER HAPPENS WHEN TURRET STARTS SEARCHING
	}

	// Returns true if the transform that is hit by the raycast is damagable otherwise returns false.
	// If its not damagable, checks line of sight to the target. If it's not in sight, the target is lost.
	// This is to make sure that the turret doesn't lose its target just because it isn't currently hitting it or pointing at it.
	// Even if another unit is standing in front of the target, the turret will still try to hit the target, damaging the unit in front of it instead.
	bool CanShoot()
	{
		Ray(out Transform didHit, out bool lineOfSightCheck);
		if (didHit)
		{
			if (didHit.TryGetComponent(out IDamagable a))
			{
				return true;
			}
			else if (!lineOfSightCheck)
			{
				Svr_LostTarget();
			}
		}
		return false;
	}

	// Sends out two variables that can be used if the method is called.
	void Ray(out Transform didHit, out bool lineOfSightCheck)
	{
		// A bool that is determined by a raycast that checks if there is a straight line between the turret and the target, without any structures in between.
		Physics.Raycast(transform.position, ((target.position + target.GetComponent<BoxCollider>().center) - transform.position), out RaycastHit hitLOS, LOSLayer);
		lineOfSightCheck = hitLOS.transform == target;

		// A transform that is determined by a raycast that points forward relative to the pivot.
		Physics.Raycast(pivot.position, pivot.forward, out RaycastHit hit, range, LOSLayer);
		didHit = hit.transform;
	}


	// Stops all coroutines and destroys the turret.
	[Server]
	void Svr_Die()
	{
		StopAllCoroutines();

		turretSmoke.transform.parent = null;

		// RPC SKER IKKE - FIX
		Rpc_Explosion();
		turretSmoke.GetComponent<DestroyAfterTime>().Svr_Destroy(5f);

		NetworkServer.Destroy(gameObject);
	}

	[ClientRpc]
	private void Rpc_Explosion()
	{
		ParticleSystem[] particleSystems = turretSmoke.GetComponentsInChildren<ParticleSystem>();
		foreach (var item in particleSystems)
		{
			item.Play();
		}
	}

	// Invoked when the turret is put down on the ground.
	// Starts the searching coroutine, the time until the turret dies and the cooldown on the engineers ability.
	[Server]
	public void Svr_OnPlaced()
	{
		Svr_StartSearching();
		StartCoroutine(Duration());
		GetComponentInParent<ActiveSClass>()?.StartAbilityCooldownCo();
	}

	private void OnDrawGizmos()
	{
		if (target)
		{
			Gizmos.color = CanShoot() ? Color.green : Color.red;
			Gizmos.DrawSphere(new Vector3(target.position.x, target.position.y + 4, target.position.z), 1);
		}
	}


	#endregion

	#region IDamagable

	public Teams Team => throw new System.NotImplementedException();

	// The turret loses health and dies if its health is 0 or less.
	public void Svr_Damage(int damage, Transform target = null)
	{
		if (IsDead()) return;
		health -= damage;
		if (health <= 0)
		{
			isDead = true;
			Svr_Die();
		}
	}

	public int GetHealth()
	{
		throw new System.NotImplementedException();
	}

	public bool IsDead() => isDead;



	#endregion
}
