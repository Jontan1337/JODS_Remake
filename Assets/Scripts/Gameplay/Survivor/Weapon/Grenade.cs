using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Grenade : Projectile
{
	[SerializeField] private float explosionTimer = 0;
	public override void Start()
	{
		base.Start();
		GetComponent<LiveEntity>().owner = owner;
		//StartCoroutine(Explode());
	}

	//public override void OnHit(Collision collision)
	//{
	//	if (!isServer) return;
	//	base.OnHit(collision);
	//}

	IEnumerator Explode()
    {
		yield return new WaitForSeconds(explosionTimer);
		Svr_Explode();
	}

	[Server]
	private void Svr_Explode()
	{
		GetComponent<LiveEntity>()?.Svr_DestroyEntity(transform);
	}
}
