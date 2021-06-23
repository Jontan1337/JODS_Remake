using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class SoldierRocket : NetworkBehaviour
{
	private void Start()
	{
		//Destroy(gameObject, 10f);
	}
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
