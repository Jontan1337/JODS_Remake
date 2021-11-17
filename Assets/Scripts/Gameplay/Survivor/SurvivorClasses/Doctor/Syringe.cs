﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Syringe : Projectile
{
	public override void Start()
	{
		base.Start();
		objectPoolTag = Tags.Syringe;
		transform.Rotate(new Vector3(90, 0, 0));
		StartCoroutine(LifeTime());
	}
	[Server]
	public override void OnHit(Collision hit)
	{
		base.OnHit(hit);
		IDamagable idmg = hit.collider.GetComponent<IDamagable>();
		if (idmg?.Team == Teams.Player)
		{
			hit.collider.transform.root.gameObject.GetComponent<StatusEffectManager>()?.ApplyStatusEffect(statusEffectToApply.ApplyEffect(hit.collider.transform.root.gameObject));
		}
		else
		{
			idmg?.Svr_Damage(2);
		}
	}

	IEnumerator LifeTime()
	{
		yield return new WaitForSeconds(5f);
		Destroy();
	}
}

