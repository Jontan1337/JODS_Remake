using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierClass : SurvivorClass
{
	private PlayerEquipment playerEquipment;
	private GameObject rocketLauncher;

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
		if (!rocketLauncher)
		{
			Cmd_EquipRocketLauncher();
		}
	}

	[Command]
	private void Cmd_EquipRocketLauncher()
	{
		if (!rocketLauncher)
		{
			EquipRocketLauncher();
		}
	}

	[Server]
	private void EquipRocketLauncher()
	{
		print("Rocket launcher was instantiated. Change to object pool");

		rocketLauncher = Instantiate(abilityObject, transform.position, transform.rotation);
		NetworkServer.Spawn(rocketLauncher);

		playerEquipment = transform.parent.GetComponentInChildren<PlayerEquipment>();

		rocketLauncher.GetComponent<EquipmentItem>().Svr_Pickup(playerEquipment.playerHands, connectionToClient);
		playerEquipment?.Svr_Equip(rocketLauncher, EquipmentType.None);
	}
}
