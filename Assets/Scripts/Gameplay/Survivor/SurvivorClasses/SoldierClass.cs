using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierClass : SurvivorClass
{
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
		EquipROcketLauncher();
	}

    void EquipROcketLauncher()
	{
		GameObject rocketLauncher = Instantiate(abilityObject, transform.position, transform.rotation);
		NetworkServer.Spawn(rocketLauncher);
		print(rocketLauncher.transform.name);
		transform.parent.GetComponentInChildren<PlayerEquipment>()?.Svr_Equip(rocketLauncher, EquipmentType.None);
	}


}
