using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect/Buff/HoT")]
public class HoTSO : StatusEffectSO
{
	[Header("Heal Settings")]
	public int healPerTick = 3;
	public int healOnApply = 12;

    public override StatusEffect ApplyEffect(GameObject target)
    {
        return new HoT(this, target);
    }
}
