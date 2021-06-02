using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EngineerClass : SurvivorClass
{

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
        GetComponentInChildren<Equipment>()?.Svr_Equip(turret, EquipmentType.None);
        turret.GetComponent<PlaceItem>().Equipped();
    }

}
