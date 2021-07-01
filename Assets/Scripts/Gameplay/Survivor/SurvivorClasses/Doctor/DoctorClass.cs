using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DoctorClass : SurvivorClass
{
	private PlayerEquipment playerEquipment;
	private GameObject medKit;

	public override void ActiveAbility()
	{
		EquipMedKit();
		if (!medKit)
		{
		}

		// Equipment slot 4 empty?
		//if ()
		//{
		//}
	}

	[Server]
	private void EquipMedKit()
	{
		medKit = Instantiate(abilityObject, transform.position, transform.rotation);
		NetworkServer.Spawn(medKit);
		playerEquipment = transform.parent.GetComponentInChildren<PlayerEquipment>();

		medKit.GetComponent<EquipmentItem>().Svr_Pickup(playerEquipment.playerHands, connectionToClient);
		playerEquipment?.Svr_Equip(medKit, EquipmentType.Meds);
		GetComponentInParent<ActiveSClass>().StartAbilityCooldownCo();
	}
}
