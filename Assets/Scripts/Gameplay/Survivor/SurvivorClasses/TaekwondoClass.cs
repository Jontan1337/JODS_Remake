using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TaekwondoClass : SurvivorClass, IHitter
{
	CharacterController cController;
	SurvivorController sController;
	LookController lController;

	[SyncVar] public float flyingKickStart = 0;
	[SyncVar] public float flyingKickEnd = 100;
	[SyncVar] public float flyingKickSpeed = 1.5f;
	[SyncVar] public int flyingKickDamage = 100;
	[SyncVar] bool flyingKick = false;

	public List<Collider> unitsHit = new List<Collider>();

	#region Serialization

	public override bool OnSerialize(NetworkWriter writer, bool initialState)
	{
		if (!initialState)
		{
			writer.WriteBoolean(flyingKick);
			return true;
		}
		else
		{
			writer.WriteSingle(flyingKickStart);
			writer.WriteSingle(flyingKickEnd);
			writer.WriteSingle(flyingKickSpeed);

			writer.WriteInt32(flyingKickDamage);

			writer.WriteBoolean(flyingKick);
			return true;
		}
	}
	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		if (!initialState)
		{
			flyingKick = reader.ReadBoolean();
		}
		else
		{
			flyingKickStart = reader.ReadSingle();
			flyingKickEnd = reader.ReadSingle();
			flyingKickSpeed = reader.ReadSingle();

			flyingKickDamage = reader.ReadInt32();

			flyingKick = reader.ReadBoolean();
		}
	}
	#endregion
	public override void OnStartClient()
	{
		if (hasAuthority || isServer)
		{
			cController = GetComponentInParent<CharacterController>();
			sController = GetComponentInParent<SurvivorController>();
			lController = GetComponentInParent<LookController>();
		}
	}

	public override void ActiveAbility()
	{
		if (CanFlyKick())
		{
			StartCoroutine(FlyingKick());
		}
	}

	private IEnumerator FlyingKick()
	{
		unitsHit.Clear();
		flyingKick = true;
		sController.enabled = false;
		lController.DisableLook();
		while (flyingKickStart < flyingKickEnd)
		{
			flyingKickStart += Time.deltaTime;
			cController.Move(transform.forward * flyingKickSpeed * Time.deltaTime);

			yield return null;
		};

		lController.EnableLook();
		sController.enabled = true;

		flyingKickStart = 0;
		flyingKick = false;

		// UNITS COLLIDERS NOT IGNORED - FIX

		foreach (Collider item in unitsHit)
		{

			if (item)
			{
				Physics.IgnoreCollision(item, cController, false);
			}
		}
		GetComponentInParent<ActiveSClass>().StartAbilityCooldownCo();
	}

	// TO DO - ONLY FLY KICK WHEN SPRINTING FORWARD

	private bool CanFlyKick()
	{
		return !sController.isGrounded && sController.isSprinting && sController.IsMoving();
	}

	public void OnHit(ControllerColliderHit hit)
	{

		if (!isServer) return;

		if (hit.gameObject.layer == 9 && flyingKick)
		{
			print(hit.gameObject.name);
			unitsHit.Add(hit.collider);
			Physics.IgnoreCollision(hit.collider, cController);
			hit.gameObject.GetComponent<IDamagable>()?.Svr_Damage(flyingKickDamage);
		}
		else if (hit.gameObject.layer == 0 && flyingKick)
		{
			flyingKickStart = flyingKickEnd;
		}
	}
}