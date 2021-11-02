using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DoctorClass : SurvivorClass
{
	private PlayerEquipment playerEquipment;
	private GameObject item;

	public override void ActiveAbility()
	{
		if (!item)
		{
			EquipSyringeGun();
		}
	}

	[Server]
	private void EquipSyringeGun()
	{
		item = Instantiate(abilityObject, transform.position, transform.rotation);
		NetworkServer.Spawn(item);
		playerEquipment = transform.parent.GetComponentInChildren<PlayerEquipment>();

		item.GetComponent<EquipmentItem>().Svr_Pickup(playerEquipment.playerHands, connectionToClient);
		playerEquipment?.Svr_Equip(item, EquipmentType.None);
	}
}
