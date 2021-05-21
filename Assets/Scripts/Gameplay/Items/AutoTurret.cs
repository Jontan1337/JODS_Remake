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
	private float fireRate = 0.1f;
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

	private Collider[] enemiesInRange;
	private List<Collider> enemiesInSight;
	private bool isSearching = false;
	private bool isDead;


	private void Start()
	{
		StartSearching();
		StartCoroutine(Duration());
	}

	#region Coroutines
	Coroutine ShootIntervalCo;
	IEnumerator ShootInterval()
	{
		while (true)
		{
			if (CanShoot()) Shoot();
			yield return new WaitForSeconds(fireRate);
		}
	}

	IEnumerator SearchingCo;
	IEnumerator Searching()
	{
		isSearching = true;
		while (true)
		{
			FindTarget();
			yield return new WaitForSeconds(1f);
		}

	}

	Coroutine RotateYCo;
	IEnumerator RotateY()
	{
		while (true)
		{
			Quaternion lookRotation = Quaternion.LookRotation((target.position - swivel.position));
			swivel.rotation = Quaternion.Slerp(swivel.rotation, lookRotation, 3 * Time.deltaTime);
			swivel.localEulerAngles = new Vector3(0, swivel.localEulerAngles.y, 0);
			yield return null;
		}
	}

	//IEnumerator RotateX()
	//{
	//	Quaternion lookRotation = Quaternion.LookRotation((target.position - cylinder.position));
	//	swivel.rotation = Quaternion.Slerp(swivel.rotation, lookRotation, 3 * Time.deltaTime);
	//	swivel.localEulerAngles = new Vector3(0, swivel.localEulerAngles.y, 0);
	//	yield return null;
	//}

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
	void FindTarget()
	{
		enemiesInSight = new List<Collider>();
		enemiesInRange = Physics.OverlapSphere(transform.position, range, ignoreLayer);
		RaycastHit hit;
		foreach (Collider item in enemiesInRange)
		{
			Physics.Raycast(transform.position, (item.transform.position - transform.position), out hit);
			if (hit.transform == item.transform)
			{
				print(hit.transform.name);
				enemiesInSight.Add(item);
			}
		}
		if (enemiesInSight.Count > 0 && enemiesInSight[0])
		{
			NewTarget(enemiesInSight[0].transform);
		}
	}
	void Shoot()
	{
		target.GetComponent<IDamagable>()?.Svr_Damage(damage, transform);
		if (target.GetComponent<IDamagable>().IsDead())
		{
			LostTarget();
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
		RotateYCo = StartCoroutine(RotateY());
		ShootIntervalCo = StartCoroutine(ShootInterval());
	}

	void LostTarget()
	{
		target = null;
		StopCoroutine(RotateYCo);
		StopCoroutine(ShootIntervalCo);
		StartSearching();
	}

	void StartSearching()
	{
		SearchingCo = Searching();
		StartCoroutine(SearchingCo);

		//TO DO - TING DER SKER NÅR MAN STARTER MED AT SØGE
	}

	bool CanShoot()
	{
		RaycastHit hit;
		Debug.DrawRay(barrel.position, barrel.forward * 10, Color.red, 0.1f);
		Physics.Raycast(barrel.position, barrel.forward, out hit, range, ignoreLayer);
		if (hit.transform == target.transform)	return true;
		return false;
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
