using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class RocketLauncher : ProjectileWeapon
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
		base.Unbind();
		Cmd_Destroy();
	}
	[Command]
	public void Cmd_Destroy()
	{
		NetworkServer.Destroy(gameObject);
	}

}
