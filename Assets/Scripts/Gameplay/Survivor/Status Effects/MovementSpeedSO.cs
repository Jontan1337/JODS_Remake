using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect/Debuff/Slow")]
public class MovementSpeedSO : StatusEffectSO
{
    public float speedModifier = 0.1f;
    public override StatusEffect ApplyEffect(GameObject target)
    {
        return new MovementSpeed(this, target);
    }
}
