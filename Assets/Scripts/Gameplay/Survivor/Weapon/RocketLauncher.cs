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


        GetComponentInParent<SurvivorClassStatManager>().Rpc_StartAbilityCooldown(transform.root.GetComponent<NetworkIdentity>().connectionToClient, transform.root);
        //Unbind();
    }

    //protected override void OnDropPerformed(InputAction.CallbackContext obj)    // TEMP - This is the old solution, which still works, but shouldnt be necessary
    //{
    //    Unbind();
    //}

    protected override void Svr_PostShoot()       // This is supposed to call unbind and destroy the Rocket Launcher, but it doesnt...
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
        Cmd_Destroy();
    }
	[Command(ignoreAuthority = true)] //Auth is lost before method is called, so this is the only sollution we know of right now. Not optimal.
	public void Cmd_Destroy()
	{
        StartCoroutine(DestroyWait());
	}
    IEnumerator DestroyWait()
	{
		yield return new WaitForSeconds(0.1f);
		NetworkServer.Destroy(gameObject);
	}

}
