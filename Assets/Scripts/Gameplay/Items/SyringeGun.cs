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
            GetComponentInParent<ActiveSClass>().Rpc_StartAbilityCooldown(transform.root.GetComponent<NetworkIdentity>().connectionToClient, transform.root);
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
        if (magazine < magazineSize)
        {
            GetComponentInParent<ActiveSClass>().Rpc_StartAbilityCooldown(transform.root.GetComponent<NetworkIdentity>().connectionToClient, transform.root);
        }
        base.Svr_Drop();
        StartCoroutine(DestroyWait());
    }

    //public override void Unbind()
    //{
    //    base.Unbind();
    //    Cmd_Destroy();
    //}

    [Command]
    public void Cmd_Destroy()
    {
        
    }
    IEnumerator DestroyWait()
    {
        yield return new WaitForSeconds(0.1f);
        NetworkServer.Destroy(gameObject);
    }
}
