using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TaekwondoClass : SurvivorClass, IHitter
{
	private CharacterController cController;
	private SurvivorController sController;
	private LookController lController;

	[SerializeField, SyncVar] private float flyingKickStart = 0;
	[SerializeField, SyncVar] private float flyingKickEnd = 100;
	[SerializeField, SyncVar] private float flyingKickSpeed = 1.5f;
	[SerializeField, SyncVar] private int flyingKickDamage = 100;
	[SyncVar] private bool flyingKick = false;

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

	private void OnTransformParentChanged()
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
		FlyingKickStart();
		while (flyingKickStart < flyingKickEnd)
		{
			flyingKickStart += Time.deltaTime;

			MoveForward();
			yield return null;
		};
		FlyingKickEnd();

	}

	private void MoveForward()
	{
		cController.Move(transform.forward * flyingKickSpeed * Time.deltaTime);
	}

	private void FlyingKickStart()
	{
		unitsHit.Clear();
		flyingKick = true;
		sController.enabled = false;
		lController.DisableLook();
	}
	private void FlyingKickEnd()
	{
		lController.EnableLook();
		sController.enabled = true;

		flyingKickStart = 0;
		flyingKick = false;

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
		if (!hasAuthority) return;

		if (hit.gameObject.layer == 9 && flyingKick)
		{
			unitsHit.Add(hit.collider);
			Physics.IgnoreCollision(hit.collider, cController);
			Cmd_OnHit(hit.gameObject);
		}
		else if (hit.gameObject.layer == 0 && flyingKick) flyingKickStart = flyingKickEnd;
	}

	[Command]
	private void Cmd_OnHit(GameObject hitObject)
	{
		hitObject.GetComponent<IDamagable>()?.Svr_Damage(flyingKickDamage);
	}
}