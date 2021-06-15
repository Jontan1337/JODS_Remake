using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class SoldierRocket : NetworkBehaviour
{
	[SerializeField] private ParticleSystem explosionEffect = null;

	private void Start()
	{
		Destroy(gameObject, 10f);
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (!isServer) return;

		if (collision.gameObject.tag != "Player")
		{
			StartExplosionEffect();
			Svr_Explode();
		}
	}
	[Server]
	void Svr_Explode()
	{
		GetComponent<IDamagable>()?.Svr_Damage(50);
	}
	private void StartExplosionEffect()
	{
		explosionEffect.Play();
		explosionEffect.gameObject.transform.parent = null;
		explosionEffect.GetComponent<SFXPlayer>()?.PlaySFX();
	}
}
