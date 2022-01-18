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


	protected override void Shoot()
	{
		base.Shoot();
		Rpc_Shoot();
		GameObject projectileToSpawn = Instantiate(projectile, shootOrigin.position, shootOrigin.rotation);
		NetworkServer.Spawn(projectileToSpawn);
		projectileToSpawn.GetComponent<Projectile>().owner = transform.root;
		//projectile = ObjectPool.Instance.SpawnFromLocalPool(bulletTag, shootOrigin.position, shootOrigin.rotation, timeToLive);
		ProjectileShoot(projectileToSpawn);
	}

	[ClientRpc]
	private void ProjectileShoot(GameObject projectile)
	{
		projectile.GetComponent<Rigidbody>().AddForce(shootOrigin.forward * projectileSpeed, ForceMode.Impulse);
	}
}
