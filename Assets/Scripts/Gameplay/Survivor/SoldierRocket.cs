using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class SoldierRocket : NetworkBehaviour
{
    private void Start()
    {
        Destroy(gameObject ,10f);
    }
    private void OnCollisionEnter(Collision collision)
	{
        if (!isServer) return;

		if (collision.gameObject.tag != "Player")
		{
			Svr_Explode();
		}
	}
    [Server]
	void Svr_Explode()
	{
        //Debug.Log("yeet");
        GetComponent<IDamagable>()?.Svr_Damage(50);
	}
}
