using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect/Debuff/Burn")]
public class BurnSO : StatusEffectSO
{
    [Header("Burn Settings")]
    public int damagePerTick = 2;
    public int onFireDamagePerTick = 5;
    [Space]
    public GameObject burnParticles;
    public GameObject onFireParticles;
    public override StatusEffect ApplyEffect(GameObject target)
    {
        return new Burn(this, target);
    }
}
