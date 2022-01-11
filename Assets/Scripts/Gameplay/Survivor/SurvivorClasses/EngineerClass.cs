﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EngineerClass : SurvivorClass
{

	private PlayerEquipment playerEquipment;
	private GameObject turret;

	#region Serialization

	public override bool OnSerialize(NetworkWriter writer, bool initialState)
	{
		if (!initialState)
		{
			return true;
		}
		else
		{
			return true;
		}
	}
	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		if (!initialState)
		{

		}
		else
		{

		}
	}
	#endregion


	public override void ActiveAbility()
	{
		if (!turret)
		{
			Cmd_EquipTurret();
		}
	}

	[Command]
	private void Cmd_EquipTurret()
    {
		EquipTurret();
	}

	[Server]
	private void EquipTurret()
	{
		print("Turret was instantiated. Change to object pool");
		turret = Instantiate(abilityObject, transform.position, transform.rotation);
		NetworkServer.Spawn(turret);

		playerEquipment = transform.parent.GetComponentInChildren<PlayerEquipment>();

		turret.GetComponent<EquipmentItem>().Svr_Pickup(playerEquipment.playerHands, connectionToClient);
		playerEquipment?.Svr_Equip(turret, EquipmentType.None);		
	}

}
