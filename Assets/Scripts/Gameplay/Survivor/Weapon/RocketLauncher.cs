﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class RocketLauncher : ProjectileWeapon
{
	protected override void Shoot(Vector2 aimPoint)
	{
		base.Shoot(aimPoint);
		if (currentAmmunition == 0 && extraAmmunition == 0)
		{
            GetComponentInParent<ActiveSClass>().Rpc_StartAbilityCooldown(transform.root.GetComponent<NetworkIdentity>().connectionToClient, transform.root);
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

        //epik fix
        if (hasAuthority) Cmd_Destroy();
		else Svr_Destroy();
	}
	[Command]
	public void Cmd_Destroy()
	{
		StartCoroutine(DestroyWait());
		//NetworkServer.Destroy(gameObject);
	}
    [Server]
    public void Svr_Destroy()
    {
        StartCoroutine(DestroyWait());
        //NetworkServer.Destroy(gameObject);
    }
    IEnumerator DestroyWait()
	{
		yield return new WaitForSeconds(0.1f);
		NetworkServer.Destroy(gameObject);
	}

}
