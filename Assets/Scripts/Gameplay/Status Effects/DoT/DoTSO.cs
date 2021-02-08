using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect/Debuff/DoT")]
public class DoTSO : StatusEffectSO
{
    [Header("Damge over Time Settings")]
    public int damagePerTick = 1;
    public override StatusEffect ApplyEffect(GameObject target)
    {
        return new DoT(this, target);
    }
}
