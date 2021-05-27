using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleGrapple : StatusEffect
{
    private IDamagable idmg;
    private TentacleGrappleSO grapple;
    public TentacleGrapple(StatusEffectSO effect, GameObject obj) : base(effect, obj)
    {
        idmg = obj.GetComponent<IDamagable>();
        grapple = (TentacleGrappleSO)effect;
    }

    public override void ApplyEffect(int? amount)
    {
        Debug.LogError("Tentacle Grapple Status Effect has not been implemented. " +
            "It has no effect yet. " +
            "It needs to: " +
            "Stop the target from being able to move and " +
            "disable their ability to use weapons of any kind.");
    }

    public override void End()
    {
        Debug.Log("UNGRAB");
    }

    public override void Tick()
    {
        base.Tick();
        //Do Damage
        idmg.Svr_Damage(grapple.damagePerTick);
    }
}
