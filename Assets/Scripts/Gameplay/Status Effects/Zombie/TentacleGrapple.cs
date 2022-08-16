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
        Debug.LogWarning("Tentacle Grapple Status Effect has not been implemented. " +
            "It has no effect yet. " +
            "It needs to: " +
            "Stop the target from being able to move and " +
            "disable their ability to use weapons of any kind.");
    }

    [Server]
    public override void Svr_End()
    {
        Debug.Log("UNGRAB");
        Rpc_EnableMovement(target.GetComponent<NetworkIdentity>().connectionToClient);
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
        Rpc_DisableMovement(target.GetComponent<NetworkIdentity>().connectionToClient);
    }

    [TargetRpc]
    private void Rpc_EnableMovement(NetworkConnection target)
    {
        JODSInput.EnableMovement();
        JODSInput.EnableJump();
        JODSInput.EnableDrop();
        JODSInput.EnableInteract();
        JODSInput.EnableReload();
        JODSInput.EnableLMB();
        JODSInput.EnableRMB();
    }
    [TargetRpc]
    private void Rpc_DisableMovement(NetworkConnection target)
    {
        JODSInput.DisableMovement();
        JODSInput.DisableJump();
        JODSInput.DisableDrop();
        JODSInput.DisableInteract();
        JODSInput.DisableReload();
        JODSInput.DisableLMB();
        JODSInput.DisableRMB();
    }
}
