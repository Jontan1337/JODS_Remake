using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syringe : Projectile
{
	public override void OnHit(Collider hit)
	{
		hit.transform.root.gameObject.GetComponent<StatusEffectManager>().ApplyStatusEffect(statusEffectToApply.ApplyEffect(hit.transform.root.gameObject));
		transform.SetParent(hit.transform);
		base.OnHit(hit);
	}
}
