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

    public override void ApplyEffect(int? amount)
    {
        
    }

    public override void End()
    {
        
    }

    public override void OnEffectApplied()
    {
        
    }

    public override void Tick()
    {
        base.Tick();
        //Do Damage
        idmg.Svr_Damage(dot.damagePerTick);
    }
}
