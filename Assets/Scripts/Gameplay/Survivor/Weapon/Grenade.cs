using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Grenade : Projectile
{
	[SerializeField] private float explosionTimer = 0;

    public override void Activate() 
	{
		base.Activate(); 
		GetComponent<LiveEntity>().owner = owner;
		StartCoroutine(Explode()); 
	}

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
