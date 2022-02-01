using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class SoldierRocket : Projectile
{
    public override void Start()
    {
		base.Start();
		GetComponent<LiveEntity>().owner = owner;
	}

	public override void OnHit(Collision collision)
	{
		if (!isServer) return;
		Svr_Explode();
		base.OnHit(collision);
	}
	[Server]
	private void Svr_Explode()
	{
		GetComponent<LiveEntity>()?.Svr_DestroyEntity(transform);
	}
}
