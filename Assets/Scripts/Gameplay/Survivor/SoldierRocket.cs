using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class SoldierRocket : Projectile
{
	[Server]
	public override void OnHit(Collision collision)
	{
		if (!isServer) return;

		//if (collision.gameObject.GetComponent<IDamagable>()?.Team != Teams.Player)
		//{
		//}
		Svr_Explode();
	}
	[Server]
	private void Svr_Explode()
	{
		print(4);

		GetComponent<LiveEntity>()?.DestroyEntity(transform);
	}
}
