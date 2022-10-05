using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect/Buff/StatModifier")]
public class StatModifierSO : StatusEffectSO
{
    public float amount; 

    public override StatusEffect ApplyEffect(GameObject target)
    {
        return new StatModifier(this, target);
    }
}
