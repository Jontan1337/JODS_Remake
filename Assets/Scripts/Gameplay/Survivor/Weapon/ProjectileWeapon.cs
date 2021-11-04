using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : RangedWeapon
{
	[Header("Projectile Settings")]
	[SerializeField] private int projectileSpeed = 30;
	[SerializeField] private float timeToLive = 5f;
	[SerializeField] private Tags bulletTag = Tags.Rocket;


	protected override void Shoot()
	{
		base.Shoot();
		Rpc_ShootFX();
		GameObject projectile = ObjectPool.Instance.SpawnFromLocalPool(bulletTag, shootOrigin.position, shootOrigin.rotation, timeToLive);
		ProjectileShoot(projectile);
	}

	[ClientRpc]
	private void ProjectileShoot(GameObject projectile)
	{
		projectile.GetComponent<Rigidbody>().AddForce(shootOrigin.forward * projectileSpeed, ForceMode.Impulse);
	}
}
