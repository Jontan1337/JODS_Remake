using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect/Debuff/Zombie/Tentacle Grapple")]
public class TentacleGrappleSO : StatusEffectSO
{
    [Header("Grapple DoT Settings")]
    public int damagePerTick = 2;
    public override StatusEffect ApplyEffect(GameObject target)
    {
        return new TentacleGrapple(this, target);
    }
}
