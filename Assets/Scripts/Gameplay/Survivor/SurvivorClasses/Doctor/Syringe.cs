using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syringe : Projectile
{
	public override void Start()
	{
		base.Start();
		transform.Rotate(new Vector3(90, 0, 0));
	}

	public override void OnHit(Collider hit)
	{
		if (!hasHit)
		{
			hasHit = true; //Prevents the projectile from hitting multiple times
			hit.transform.root.gameObject.GetComponent<StatusEffectManager>()?.ApplyStatusEffect(statusEffectToApply.ApplyEffect(hit.transform.root.gameObject));
			transform.SetParent(hit.transform);
			rb.isKinematic = true;
		}
	}
}
