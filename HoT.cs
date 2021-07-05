using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoT : StatusEffect
{
	private IDamagable idmg;
	private HoTSO hot;

	public HoT(StatusEffectSO effect, GameObject obj) : base(effect, obj)
	{
		idmg = obj.GetComponent<IDamagable>();
		hot = (HoTSO)effect;
	}
	public override void ApplyEffect(int? amount)
	{
		throw new System.NotImplementedException();
	}

	public override void End()
	{
		throw new System.NotImplementedException();
	}

	public override void OnEffectApplied()
	{
		throw new System.NotImplementedException();
	}
}
