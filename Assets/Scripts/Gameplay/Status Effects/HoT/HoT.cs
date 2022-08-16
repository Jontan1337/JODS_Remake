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
	public override void Svr_ApplyEffect(int? amount)
	{
		idmg.Svr_Damage(-hot.healOnApply);
	}

	public override void Svr_End()
	{
	}

	public override void Svr_OnEffectApplied()
	{
	}

	public override void Svr_Tick()
	{
		base.Svr_Tick();

		idmg.Svr_Damage(-hot.healPerTick);
	}
}


