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
			Cmd_Drop();
		}
	}

	
    public override void Unbind()
    {
		base.Unbind();
		Cmd_Destroy();
        //Cmd_Drop();
	}
	[Command]
	public void Cmd_Destroy()
	{
		NetworkServer.Destroy(gameObject);
	}

}
