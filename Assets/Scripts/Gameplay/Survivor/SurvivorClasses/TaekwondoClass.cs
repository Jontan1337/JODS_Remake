﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RootMotion.FinalIK;

public class TaekwondoClass : SurvivorClass, IHitter
{
	private CharacterController cController;
	private SurvivorController sController;
	private LookController lController;

	[SerializeField, SyncVar] private float flyingKickStart = 0;
	[SerializeField, SyncVar] private float flyingKickEnd = 100;
	[SerializeField, SyncVar] private float flyingKickSpeed = 1.5f;
	[SerializeField, SyncVar] private int flyingKickDamage = 100;
	[SerializeField, SyncVar] private int kickDamage = 50;

	[SerializeField] private Collider lowerLeg;
	[SyncVar] private bool flyingKick = false;
	[SyncVar] private bool kicking = false;


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

			//lowerLeg = transform.parent.Find("Armature").Find("R.Hip").Find("R.UpperLeg").Find("R.LowerLeg").AddComponent<Collider>();

			lowerLeg = transform.parent.Find("Armature").GetComponentInChildren<Collider>();

		}
	}

	public override void ActiveAbility()
	{
		if (CanFlyKick())
		{
			StartCoroutine(FlyingKick());
			GetComponentInParent<ActiveSClass>().StartAbilityCooldownCo();
		}
		else if (!kicking)
		{
			StartCoroutine(Kick());
		}
	}

	#region FlyingKick

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
		JODSInput.DisableCamera();
	}
	private void FlyingKickEnd()
	{
		JODSInput.EnableCamera();
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
	}

	private bool CanFlyKick()
	{
		return !sController.isGrounded && sController.isSprinting && sController.IsMoving();
	}

	public void OnFlyingKickHit(ControllerColliderHit hit)
	{
		if (!hasAuthority) return;

		if (hit.gameObject.layer == 9 && flyingKick)
		{
			unitsHit.Add(hit.collider);
			Physics.IgnoreCollision(hit.collider, cController);
			Cmd_OnHit(hit.transform, flyingKickDamage);
		}
		else if (hit.gameObject.layer == 0 && flyingKick) flyingKickStart = flyingKickEnd;
	}

	#endregion

	#region Kick

	public void OnKickHit(Collider hit)
	{
		if (!hasAuthority) return;
		//print(hit.transform.root.name + " " + hit.transform.root.gameObject.layer);
		if (hit.gameObject.layer == 9 && kicking || hit.gameObject.layer == 10 && kicking)
		{
			Cmd_OnHit(hit.transform.root, kickDamage);
		}
	}

	private IEnumerator Kick()
	{
		unitsHit.Clear();
		kicking = true;
		sController.enabled = false;
		GetComponentInParent<SurvivorAnimationManager>().anim.SetBool("Kicking", true);
		GetComponentInParent<FullBodyBipedIK>().enabled = false;
		lowerLeg.enabled = true;

		yield return new WaitForSeconds(0.2f);

		kicking = false;
		sController.enabled = true;
		GetComponentInParent<SurvivorAnimationManager>().anim.SetBool("Kicking", false);
		GetComponentInParent<FullBodyBipedIK>().enabled = true;
		lowerLeg.enabled = false;
	}

	#endregion


	[Command]
	private void Cmd_OnHit(Transform hitObject, int dmg)
	{
		hitObject.GetComponent<IDamagable>()?.Svr_Damage(dmg);
	}
}