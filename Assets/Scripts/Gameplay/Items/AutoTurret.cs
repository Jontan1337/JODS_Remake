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

	private void Start()
	{
		StartCoroutine(TurretShoot());
	}

	IEnumerator TurretShoot()
	{
		Debug.DrawRay(transform.position, transform.forward * range);

		print("yeeee1");
		while (active)
		{
			targets = Physics.OverlapSphere(transform.position, range, ~ignoreLayer);
			for (int i = 0; i < targets.Length; i++)
			{



				//print(Vector3.Distance(targets[i].transform.position, gameObject.transform.position));
				//print(targets[i].name);

			}
			yield return new WaitForSeconds(1);
		}

	}
}
