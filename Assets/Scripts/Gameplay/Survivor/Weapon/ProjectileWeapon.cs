using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : RangedWeapon
{
	[Header("Projectile Settings")]
	[SerializeField] private int projectileSpeed = 30;
	[SerializeField] private float timeToLive = 5f;
	//[SerializeField] private Tags bulletTag;
	[SerializeField] private GameObject projectile;


	protected override void Svr_Shoot(Vector2 aimPoint)
	{
		base.Svr_Shoot(aimPoint);
		Rpc_Shoot(Vector2.zero);
		GameObject projectileToSpawn = Instantiate(projectile, shootOrigin.position, shootOrigin.rotation);
		NetworkServer.Spawn(projectileToSpawn);
		projectileToSpawn.GetComponent<Projectile>().owner = transform.root;
		//projectile = ObjectPool.Instance.SpawnFromLocalPool(bulletTag, shootOrigin.position, shootOrigin.rotation, timeToLive);
		ProjectileShoot(projectileToSpawn);
	}

	[ClientRpc]
	private void ProjectileShoot(GameObject projectile)
	{
		Rigidbody rb = projectile.GetComponent<Rigidbody>();
		rb.AddForce(shootOrigin.forward * (projectileSpeed * rb.mass), ForceMode.Impulse);
	}
}
