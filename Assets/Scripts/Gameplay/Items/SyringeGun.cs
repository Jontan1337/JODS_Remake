using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyringeGun : ProjectileWeapon
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

	public override void Svr_Drop()
	{
		if (currentAmmunition < maxCurrentAmmunition)
		{
			GetComponentInParent<ActiveSClass>().StartAbilityCooldownCo();
		}
		Unbind();
		base.Svr_Drop();
	}
}
