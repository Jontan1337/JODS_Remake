using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TentacleGrapple : StatusEffect
{
    private GameObject target;
    private IDamagable idmg;
    private TentacleGrappleSO grapple;
    public TentacleGrapple(StatusEffectSO effect, GameObject obj) : base(effect, obj)
    {
        target = obj;
        idmg = obj.GetComponent<IDamagable>();
        grapple = (TentacleGrappleSO)effect;
    }

    [Server]
    public override void Svr_ApplyEffect(int? amount)
    {

    }

    [Server]
    public override void Svr_End()
    {
        PlayerManager.Instance.Rpc_EnableEverythingButMenuAndCamera(target.GetComponent<NetworkIdentity>().connectionToClient);
    }

    [Server]
    public override void Svr_Tick()
    {
        base.Svr_Tick();
        //Do Damage
        idmg.Svr_Damage(grapple.damagePerTick);
    }

    [Server]
    public override void Svr_OnEffectApplied()
    {
        PlayerManager.Instance.Rpc_DisableEverythingButMenuAndCamera(target.GetComponent<NetworkIdentity>().connectionToClient);
    }

}
