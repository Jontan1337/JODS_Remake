using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModifier : StatusEffect
{
    private ModifierManagerSurvivor survivorModifier;
    private StatModifierSO statModifier;
    public StatModifier(StatusEffectSO effect, GameObject obj) : base(effect, obj)
    {
        survivorModifier = obj.GetComponent<ModifierManagerSurvivor>();
        statModifier = (StatModifierSO)effect;
    }

    public override void Svr_ApplyEffect(int? amount)
    {

    }

    public override void Svr_End()
    {
        survivorModifier.data.MovementSpeed -= statModifier.amount;
        survivorModifier.data.Damage -= statModifier.amount;
        survivorModifier.data.DamageResistance -= statModifier.amount;
    }

    public override void Svr_OnEffectApplied()
    {
        survivorModifier.data.MovementSpeed += statModifier.amount;
        survivorModifier.data.Damage += statModifier.amount;
        survivorModifier.data.DamageResistance += statModifier.amount;
    }



}
