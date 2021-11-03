using UnityEngine;
using Mirror;

public class EquipmentControl : NetworkBehaviour
{
	private PlayerEquipment playerEquipment;

	public override void OnStopServer()
	{
		playerEquipment.onServerEquippedItemChange -= Svr_ChangeControlBind;
	}
	public override void OnStartAuthority()
	{
		transform.root.GetComponent<SurvivorSetup>().onSpawnItem += GetEquipment;
	}
	public override void OnStopAuthority()
	{
		transform.root.GetComponent<SurvivorSetup>().onSpawnItem -= GetEquipment;
	}

	public void GetEquipment(GameObject item)
	{
		if (item.TryGetComponent(out ItemName itemName))
		{
			switch (itemName.itemName)
			{
				case ItemNames.Equipment:
					Cmd_SetEquipment(item);
					break;
				default:
					break;
			}
		}
	}

	[Command]
	private void Cmd_SetEquipment(GameObject item)
	{
		playerEquipment = item.GetComponent<PlayerEquipment>();
		playerEquipment.onServerEquippedItemChange += Svr_ChangeControlBind;
	}

	[Server]
	private void Svr_ChangeControlBind(GameObject oldItem, GameObject newItem)
	{
		if (oldItem)
			Rpc_UnBind(connectionToClient, oldItem);

		if (newItem)
			Rpc_Bind(connectionToClient, newItem);
	}
	[TargetRpc]
	private void Rpc_Bind(NetworkConnection target, GameObject item)
	{
		if (item)
		{
			item.GetComponent<IBindable>().Bind();
		}
	}
	[TargetRpc]
	private void Rpc_UnBind(NetworkConnection target, GameObject item)
	{
		if (item)
		{
			item.GetComponent<IBindable>().Unbind();
		}
	}
}
