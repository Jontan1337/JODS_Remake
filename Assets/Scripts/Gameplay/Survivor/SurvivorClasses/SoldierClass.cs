﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierClass : SurvivorClass
{
    PlayerEquipment playerEquipment;

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

    private void Start()
    {
        abilityIsToggled = true;
    }

    public override void ActiveAbility()
    {
		EquipRocketLauncher();
	}

    void EquipRocketLauncher()
	{
		GameObject rocketLauncher = Instantiate(abilityObject, transform.position, transform.rotation);
		NetworkServer.Spawn(rocketLauncher);

        playerEquipment = transform.parent.GetComponentInChildren<PlayerEquipment>();

        playerEquipment?.Svr_Equip(rocketLauncher, EquipmentType.None);
        rocketLauncher.GetComponent<EquipmentItem>().Svr_Pickup(playerEquipment.playerHands, connectionToClient);
    }


}
