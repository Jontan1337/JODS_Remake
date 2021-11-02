using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SyringeGun : ProjectileWeapon
{
	protected override void Shoot()
	{
		base.Shoot();
		if (currentAmmunition == 0 && extraAmmunition == 0)
		{
			GetComponentInParent<ActiveSClass>().StartAbilityCooldownCo();
			Unbind();
		}
	}
	protected override void OnDropPerformed(InputAction.CallbackContext obj)
	{
		Unbind();
	}

	public override void Unbind()
	{
		if (currentAmmunition < maxCurrentAmmunition)
		{
			GetComponentInParent<ActiveSClass>().StartAbilityCooldownCo();
		}
		base.Unbind();
		Cmd_Destroy();
	}
	[Command]
	public void Cmd_Destroy()
	{
		NetworkServer.Destroy(gameObject);
	}
}
