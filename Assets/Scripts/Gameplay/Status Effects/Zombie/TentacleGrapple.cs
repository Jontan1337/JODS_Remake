using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleGrapple : StatusEffect
{
    private IDamagable idmg;
    private TentacleGrappleSO paralyze;
    public TentacleGrapple(StatusEffectSO effect, GameObject obj) : base(effect, obj)
    {
        idmg = obj.GetComponent<IDamagable>();
        paralyze = (TentacleGrappleSO)effect;
    }

    public override void ApplyEffect(int? amount)
    {

    }

    public override void End()
    {

    }

    public override void Tick()
    {
        base.Tick();
        //Do Damage
        idmg.Svr_Damage(paralyze.damagePerTick);
    }
}
