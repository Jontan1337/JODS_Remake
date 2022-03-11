using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect/Debuff/Survivor Hit")]
public class SurvivorHitSO : StatusEffectSO
{
    public float speedModifier = -0.1f;
    public override StatusEffect ApplyEffect(GameObject target)
    {
        return new SurvivorHit(this, target);
    }
}
