using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AutoTurret : NetworkBehaviour, IDamagable
{
	[SerializeField]
	private float range = 10000;
	[SerializeField]
	private int damage = 2;
	[SerializeField]
	private LayerMask ignoreLayer;

	private bool active = true;
	private Collider[] enemiesInRange;
	[SerializeField]
	private Transform target;
	[SerializeField]
	private Transform swivel;
	[SerializeField]
	private Transform pivot;
	[SerializeField]
	private Transform barrel;

	bool isSearching;




	private void Start()
	{
		StartSearching();
		print(SearchingCo);
	}

	Coroutine TurretCo;
	IEnumerator Turret()
	{
		while (true)
		{
			if (CanShoot()) Shoot();
			yield return new WaitForSeconds(0.1f);
		}
	}

	Coroutine SearchingCo;
	IEnumerator Searching()
	{
		isSearching = true;
		print("searching");
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
	void FindTarget()
	{
		enemiesInRange = Physics.OverlapSphere(transform.position, range, ignoreLayer);
		if (enemiesInRange[0])
		{
			NewTarget(enemiesInRange[0].transform);
		}



	}
	void Shoot()
	{
		print("shoot");
		target.GetComponent<IDamagable>()?.Svr_Damage(damage);
		if (target.GetComponent<IDamagable>().IsDead())
		{
			LostTarget();
		}
	}

	void NewTarget(Transform newTarget)
	{
		print(SearchingCo);
		target = newTarget;
		if (isSearching)
		{
			StopCoroutine(SearchingCo);
			isSearching = false;
		}
		RotateYCo = StartCoroutine(RotateY());
		TurretCo = StartCoroutine(Turret());
	}

	void LostTarget()
	{
		target = null;
		StartSearching();
		StopCoroutine(RotateYCo);
		StopCoroutine(TurretCo);
	}

	void StartSearching()
	{
		SearchingCo = StartCoroutine(Searching());
		print(SearchingCo);
		//TO DO - TING DER SKER NÅR MAN STARTER MED AT SØGE
	}

	bool CanShoot()
	{
		return true;
	}
	#region IDamagable

	public Teams Team => throw new System.NotImplementedException();
	public void Svr_Damage(int damage, Transform target = null)
	{
		throw new System.NotImplementedException();
	}

	public int GetHealth()
	{
		throw new System.NotImplementedException();
	}

	public bool IsDead()
	{
		throw new System.NotImplementedException();
	}
	#endregion
}
