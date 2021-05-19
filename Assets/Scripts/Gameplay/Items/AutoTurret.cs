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
	private Collider[] targets;
	private GameObject target;

	private void Start()
	{
		StartCoroutine(TurretShoot());
	}

	IEnumerator TurretShoot()
	{
		print("CMON");
		if (!target)
		{
			FindTargets();
		}
		Debug.DrawRay(transform.position, transform.forward * range);
		print("yeeee1");
		while (active)
		{
			target.GetComponent<IDamagable>()?.Svr_Damage(2);
			print("yeeee2");
			yield return new WaitForSeconds(0.1f);
		}

	}

	void FindTargets()
	{
		targets = Physics.OverlapSphere(transform.position, range, ~ignoreLayer);
		for (int i = 0; i < targets.Length; i++)
		{

			if (Vector3.Distance(target.transform.position, gameObject.transform.position) > Vector3.Distance(targets[i].transform.position, gameObject.transform.position))
			{
				target = targets[i].gameObject;
			}
		}
	}
	IEnumerator Searching()
	{
		while (!target)
		{
			FindTargets();
			yield return new WaitForSeconds(1);
		}
	}
}
