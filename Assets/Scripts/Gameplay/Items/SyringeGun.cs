using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SyringeGun : ProjectileWeapon
{
    [SerializeField]
    private List<GameObject> syringes = new List<GameObject>();

    protected override void Svr_Shoot(Vector2 aimPoint)
    {
        base.Svr_Shoot(aimPoint);

        syringes[0].SetActive(false);
        syringes.RemoveAt(0);
    }

    protected override void Svr_PostShoot()
    {
        base.Svr_PostShoot();
        if (syringes.Count == 0)
        {
            GetComponentInParent<SurvivorClassStatManager>().Rpc_StartAbilityCooldown(owner.GetComponent<NetworkIdentity>().connectionToClient, owner);
            StartCoroutine(IEDrop());
        }
    }

    private IEnumerator IEDrop()
    {
        yield return new WaitForSeconds(0.2f);
        Svr_Drop();
    }

    public override void Svr_Drop()
    {
        if (magazine < magazineSize && magazine > 0)
        {
            GetComponentInParent<SurvivorClassStatManager>().Rpc_StartAbilityCooldown(owner.GetComponent<NetworkIdentity>().connectionToClient, owner);
        }
        base.Svr_Drop();
    }

    public override void Unbind()
    {
        Cmd_StartCooldown();
        base.Unbind();
        Cmd_Destroy();
    }

    [Command]
    private void Cmd_StartCooldown()
    {
        if (magazine < magazineSize && magazine > 0)
        {
            GetComponentInParent<SurvivorClassStatManager>().Rpc_StartAbilityCooldown(owner.GetComponent<NetworkIdentity>().connectionToClient, owner);
        }
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
