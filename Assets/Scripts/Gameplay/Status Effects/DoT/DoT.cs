using UnityEngine;

public class DoT : StatusEffect
{
    private IDamagable idmg;
    private DoTSO dot;
    public DoT(StatusEffectSO effect, GameObject obj) : base(effect, obj)
    {
        idmg = obj.GetComponent<IDamagable>();
        dot = (DoTSO)effect;
    }

    public override void Svr_ApplyEffect(int? amount)
    {
        
    }

    public override void Svr_End()
    {
        
    }

    public override void Svr_OnEffectApplied()
    {
        
    }

    public override void Svr_Tick()
    {
        base.Svr_Tick();
        //Do Damage
        idmg.Svr_Damage(dot.damagePerTick);
    }
}
