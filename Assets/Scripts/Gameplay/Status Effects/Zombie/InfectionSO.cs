using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect/Debuff/Infection")]
public class InfectionSO : StatusEffectSO
{

    public override StatusEffect ApplyEffect(GameObject target)
    {
        return new Infection(this, target);
    }
}
