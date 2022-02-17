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
		if (!turret)
		{
			EquipTurret();
		}
	}

	[Server]
	private void EquipTurret()
	{
		turret = Instantiate(abilityObject, transform.position, transform.rotation);
		NetworkServer.Spawn(turret);
		playerEquipment = transform.parent.GetComponentInChildren<PlayerEquipment>();

		turret.GetComponent<IInteractable>().Svr_Interact(transform.root.gameObject);
		//turret.GetComponent<EquipmentItem>().Svr_Pickup(playerEquipment.playerHands, connectionToClient);
		//playerEquipment?.Svr_Equip(turret, EquipmentType.None);		
	}

}
