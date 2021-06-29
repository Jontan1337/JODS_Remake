using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorMedkit : EquipmentItem
{

	[SyncVar] private int uses = 4;
	[SerializeField] private GameObject medKit;

	//public override void Svr_Drop()
	//{
	//	base.Svr_Drop();

	//}

	[Server]
	public override void Svr_Interact(GameObject interacter)
	{
		PlayerEquipment playerEquipment = interacter.GetComponentInChildren<PlayerEquipment>();

		if (uses > 0)
		{
			GameObject newMedKit = Instantiate(medKit, transform.position, transform.rotation);
			NetworkServer.Spawn(newMedKit);
			//newMedKit.GetComponent<EquipmentItem>().Svr_Pickup(playerEquipment.playerHands, interacter.GetComponent<NetworkIdentity>().connectionToClient);
			playerEquipment?.Svr_Equip(newMedKit, EquipmentType.Meds);
			uses--;
		}

	}
}
