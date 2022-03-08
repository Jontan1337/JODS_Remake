using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class RocketLauncher : ProjectileWeapon
{
	protected override void Svr_Shoot(Vector2 aimPoint)
	{
		base.Svr_Shoot(aimPoint);


        GetComponentInParent<ActiveSClass>().Rpc_StartAbilityCooldown(transform.root.GetComponent<NetworkIdentity>().connectionToClient, transform.root);
    }

    protected override void Svr_PostShoot()
    {
        base.Svr_PostShoot();
        StartCoroutine(IEDrop());
    }

    private IEnumerator IEDrop()
    {
        yield return new WaitForSeconds(0.2f);
        Svr_Drop();
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
