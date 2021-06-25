using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class SoldierRocket : NetworkBehaviour
{
	[Server]
	private void OnCollisionEnter(Collision collision)
	{
		if (!isServer) return;

		if (collision.gameObject.tag != "Player")
		{
			Rpc_Explode();
		}
	}
	[ClientRpc]
	private void Rpc_Explode()
	{
		GetComponent<LiveEntity>()?.DestroyEntity(transform);
	}
}
