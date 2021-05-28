using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AutoTurret : NetworkBehaviour, IDamagable
{

	[Header("Stats")]
	[SerializeField]
	private float range = 30;
	[SerializeField]
	private float fireRate = 240f;
	[SerializeField]
	private float rotateSpeed = 1f;
	[SerializeField]
	private float searchInterval = 1f;
	[SerializeField]
	private int damage = 20;
	[SerializeField]
	private int duration = 20;
	[SerializeField]
	private int health = 200;



	[Header("References")]
	[SerializeField]
	private Transform target = null;
	[SerializeField]
	private Transform swivel = null;
	[SerializeField]
	private Transform pivot = null;
	[SerializeField]
	private Transform barrel = null;

	[Space]
	[SerializeField]
	private LayerMask unitLayer;
	[SerializeField]
	private LayerMask LOSLayer;

	private List<Collider> enemiesInSight = new List<Collider>();
	private bool isDead;


	private void Start()
	{
		StartSearching();
		StartCoroutine(Duration());
	}

	#region Coroutines
	IEnumerator ShootIntervalCo;
	IEnumerator ShootInterval()
	{
		while (true)
		{
			if (CanShoot()) Shoot();
			yield return new WaitForSeconds(1 / (fireRate / 60));
		}
	}

	IEnumerator SearchingCo;
	IEnumerator Searching()
	{
		while (true)
		{
			FindTarget();
			yield return new WaitForSeconds(searchInterval);
		}

	}

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

	IEnumerator Duration()
	{
		while (duration > 0)
		{
			duration--;

			yield return new WaitForSeconds(1);
		}
		Die();
	}

	#endregion

	#region Methods
	void Shoot()
	{
		Ray(out Transform didHit, out bool lineOfSightCheck);

		Debug.DrawRay(barrel.position, barrel.forward * 10, Color.red, 0.1f);

		didHit.GetComponent<IDamagable>()?.Svr_Damage(damage, transform);
		if (target.GetComponent<IDamagable>().IsDead())
		{
			LostTarget();
		}
	}
	void FindTarget()
	{
		enemiesInSight.Clear();
		Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, range, unitLayer);
		foreach (Collider item in enemiesInRange)
		{
			Debug.DrawLine(swivel.transform.position, (item.transform.position + item.GetComponent<BoxCollider>().center), Color.blue, 0.2f);
			Physics.Raycast(swivel.transform.position, ((item.transform.position + item.GetComponent<BoxCollider>().center) - transform.position), out RaycastHit hit, LOSLayer);
			if (hit.transform.root == item.transform || hit.transform.IsChildOf(item.transform))
			{
				enemiesInSight.Add(item);
			}
		}
		if (GetClosestEnemyCollider(enemiesInSight))
		{
			NewTarget(GetClosestEnemyCollider(enemiesInSight).transform);
		}
	}

	void NewTarget(Transform newTarget)
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

	void LostTarget()
	{
		target = null;
		StopCoroutine(RotateYCo);
		StopCoroutine(RotateXCo);
		StopCoroutine(ShootIntervalCo);
		StartSearching();
	}

	void StartSearching()
	{
		RotatePassiveCo = RotatePassive();
		StartCoroutine(RotatePassiveCo);

		SearchingCo = Searching();
		StartCoroutine(SearchingCo);

		//TO DO - WHATEVER HAPPENS WHEN TURRET STARTS SEARCHING
	}


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
				LostTarget();
			}
		}
		return false;
	}

	void Ray(out Transform didHit, out bool lineOfSightCheck)
	{
		Physics.Raycast(transform.position, ((target.position + target.GetComponent<BoxCollider>().center) - transform.position), out RaycastHit hitLOS, LOSLayer);
		lineOfSightCheck = hitLOS.transform == target;

		Physics.Raycast(pivot.position, pivot.forward, out RaycastHit hit, range, LOSLayer);
		didHit = hit.transform;
	}

	void Die()
	{
		StopAllCoroutines();

		// TO DO - WHATEVER HAPPENS WHEN TURRET DIES
		Destroy(gameObject);
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
	public void Svr_Damage(int damage, Transform target = null)
	{
		if (IsDead()) return;
		health -= damage;
		if (health <= 0)
		{
			isDead = true;
			Die();
		}
	}

	public int GetHealth()
	{
		throw new System.NotImplementedException();
	}

	public bool IsDead() => isDead;



	#endregion
}
