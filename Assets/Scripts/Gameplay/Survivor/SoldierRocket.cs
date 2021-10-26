using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class SoldierRocket : Projectile
{
	public override void OnHit(Collision collision)
	{
		if (!isServer) return;
		Svr_Explode();
		base.OnHit(collision);
	}
	[Server]
	private void Svr_Explode()
	{
		GetComponent<LiveEntity>()?.DestroyEntity(transform);
	}
}
