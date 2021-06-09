using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EngineerClass : SurvivorClass
{

    PlayerEquipment playerEquipment;

    private void Start()
	{
        abilityIsToggled = true;
	}

    public override void ActiveAbility()
    {
        EquipTurret();
    }

    private void EquipTurret()
	{
        GameObject turret = Instantiate(abilityObject, transform.position, transform.rotation);
        NetworkServer.Spawn(turret);

        playerEquipment = transform.parent.GetComponentInChildren<PlayerEquipment>();


        playerEquipment?.Svr_Equip(turret, EquipmentType.None);
        turret.GetComponent<EquipmentItem>().Svr_Pickup(playerEquipment.playerHands, connectionToClient);
		turret.GetComponent<PlaceItem>().Equipped();
    }

}
