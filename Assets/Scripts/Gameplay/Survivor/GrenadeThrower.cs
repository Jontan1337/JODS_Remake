using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class GrenadeThrower : ProjectileWeapon
{
    protected override void Svr_Shoot(Vector2 aimPoint)
    {
        base.Svr_Shoot(aimPoint);
    }

    protected override void Svr_PostShoot()
    {
        base.Svr_PostShoot();
        if (hasAuthority)
        {
            Cmd_Destroy();
        }
        else Svr_Destroy();
    }

    [Command]
    public void Cmd_Destroy()
    {
        StartCoroutine(DestroyWait());
    }
    [Server]
    public void Svr_Destroy()
    {
        StartCoroutine(DestroyWait());
    }
    IEnumerator DestroyWait()
    {
        yield return new WaitForSeconds(0.1f);
        NetworkServer.Destroy(gameObject);
    }
}
