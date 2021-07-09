using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RocketLauncher : ProjectileWeapon
{
	protected override void Shoot()
	{
		base.Shoot();
		if (currentAmmunition == 0 && extraAmmunition == 0)
		{
			GetComponentInParent<ActiveSClass>().StartAbilityCooldownCo();
			Unbind();
			base.Svr_Drop();
		}
	}
}
