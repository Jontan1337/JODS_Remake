using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AutoTurret : NetworkBehaviour
{
	[SerializeField]
	private float range = 10000;
	[SerializeField]
	private LayerMask ignoreLayer;

	private bool active = true;
	private Collider[] enemiesInRange;
	private GameObject target;

	private void Start()
	{
		StartCoroutine(Searching());
	}

	IEnumerator TurretShoot()
	{
		print("Start shooting...");
		IDamagable targetHealth = target.GetComponent<IDamagable>();
		while (target)
		{
			targetHealth?.Svr_Damage(2);
			if (targetHealth.IsDead())
			{
				target = null;
			}
			//print("Shooting...");
			yield return new WaitForSeconds(0.1f);
		}
		StartCoroutine(Searching());
	}

	void FindTarget()
	{
		enemiesInRange = Physics.OverlapSphere(transform.position, range, ~ignoreLayer);
		target = enemiesInRange[0].gameObject;
		print(enemiesInRange.Length);
		for (int i = 0; i < enemiesInRange.Length; i++)
		{
			if (Vector3.Distance(target.transform.position, gameObject.transform.position) > Vector3.Distance(enemiesInRange[i].transform.position, gameObject.transform.position))
			{
				target = enemiesInRange[i].gameObject;
				print(target.name);
			}
		}
	}
	IEnumerator Searching()
	{
		while (!target)
		{
			print("Searching...");
			FindTarget();
			yield return new WaitForSeconds(0.1f);
		}

		print("Target found: " + target.name);
		StartCoroutine(TurretShoot());
	}
}
