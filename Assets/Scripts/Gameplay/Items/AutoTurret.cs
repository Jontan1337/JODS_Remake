using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AutoTurret : NetworkBehaviour, IDamagable
{

	[Header("Stats")]
	[SerializeField]
	private float range = 10000;
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
	private LayerMask ignoreLayer;
	[SerializeField]
	private LayerMask ignoreLayerLOS;

	private Collider[] enemiesInRange;
	private List<Collider> enemiesInSight = new List<Collider>();
	private bool isSearching = false;
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
		isSearching = true;
		while (true)
		{
			print("Searching...");
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
		while (true)
		{
			//swivel.rotation = Quaternion.Euler(0, 1, 0);
			//print("rotate");
			yield return new WaitForSeconds(0.1f);
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
		Ray(out bool didHit, out RaycastHit hit, out bool lineOfSightCheck);

		Debug.DrawRay(barrel.position, barrel.forward * 10, Color.red, 0.1f);

		hit.transform.GetComponent<IDamagable>()?.Svr_Damage(damage, transform);
		if (target.GetComponent<IDamagable>().IsDead())
		{
			LostTarget();
		}
	}
	void FindTarget()
	{
		enemiesInSight.Clear();
		enemiesInRange = Physics.OverlapSphere(transform.position, range, ignoreLayer);
		RaycastHit hit;
		foreach (Collider item in enemiesInRange)
		{
			Physics.Raycast(swivel.transform.position, ((item.transform.position + item.GetComponent<BoxCollider>().center) - transform.position), out hit);
			Debug.DrawLine(swivel.transform.position, (item.transform.position + item.GetComponent<BoxCollider>().center), Color.blue, 0.2f);

			if (hit.transform == item.transform)
			{
				print(hit.transform.name);
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
		if (isSearching)
		{
			StopCoroutine(SearchingCo);
			isSearching = false;
		}
		RotateYCo = RotateY();
		RotateXCo = RotateX();
		ShootIntervalCo = ShootInterval();

		StartCoroutine(RotateXCo);
		StartCoroutine(RotateYCo);
		StartCoroutine(ShootIntervalCo);
		StopCoroutine(RotatePassiveCo);
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

		//TO DO - TING DER SKER NÅR MAN STARTER MED AT SØGE
	}

	Collider GetClosestEnemyCollider(List<Collider> enemyColliders)
	{
		float bestDistance = 99999.0f;
		Collider bestCollider = null;

		foreach (Collider enemy in enemyColliders)
		{
			float distance = Vector3.Distance(transform.position, enemy.transform.position);

			if (distance < bestDistance)
			{
				bestDistance = distance;
				bestCollider = enemy;
			}
		}
		return bestCollider;
	}

	bool CanShoot()
	{
		Ray(out bool didHit, out RaycastHit hit, out bool lineOfSightCheck);
		if (didHit)
		{
			if (hit.transform.TryGetComponent(out IDamagable a))
			{
				if (lineOfSightCheck)
				{
					return true;
				}
				else
				{
					print("Lost target...");

					LostTarget();
				}
			}
			else if (!lineOfSightCheck)
			{
				print("Lost target...");
				LostTarget();
			}
		}
		return false;
	}

	void Ray(out bool didHit, out RaycastHit hit, out bool lineOfSightCheck)
	{
		RaycastHit hitLOS;
		Physics.Raycast(transform.position, ((target.transform.position + target.GetComponent<BoxCollider>().center) - transform.position), out hitLOS, ignoreLayerLOS);

		// LINE OF SIGHT BROKEN BY OTHER ZOMBIES - FIX???
		lineOfSightCheck = hitLOS.transform == target.transform;


		Physics.Raycast(barrel.position, barrel.forward, out hit, range);
		didHit = hit.transform;
	}

	void Die()
	{
		StopAllCoroutines();
		Destroy(gameObject);
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
