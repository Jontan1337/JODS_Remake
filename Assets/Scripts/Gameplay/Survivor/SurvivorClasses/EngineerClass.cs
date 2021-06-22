using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EngineerClass : SurvivorClass
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


    public override void ActiveAbility()
    {
        EquipTurret();
    }

    [Server]
    private void EquipTurret()
	{

        print("Turret was instantiated. Change to object pool");
        GameObject turret = Instantiate(abilityObject, transform.position, transform.rotation);
        NetworkServer.Spawn(turret);

        playerEquipment = transform.parent.GetComponentInChildren<PlayerEquipment>();


        playerEquipment?.Svr_Equip(turret, EquipmentType.None);
        turret.GetComponent<EquipmentItem>().Svr_Pickup(playerEquipment.playerHands, connectionToClient);
		turret.GetComponent<PlaceItem>().Equipped(connectionToClient);
    }

}
