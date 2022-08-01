using UnityEngine;
using Mirror;

public class DoctorClass : SurvivorClass
{
	private PlayerEquipment playerEquipment;
	private GameObject syringeGun;

	public override void ActiveAbility()
	{
		if (!syringeGun)
		{
			Cmd_EquipSyringeGun();
		}
	}

	[Command]
	private void Cmd_EquipSyringeGun()
	{
		if (!syringeGun)
		{
			EquipSyringeGun();
		}
	}

	[Server]
	private void EquipSyringeGun()
	{
		syringeGun = Instantiate(abilityObject, transform.position, transform.rotation);
		NetworkServer.Spawn(syringeGun);
		playerEquipment = transform.parent.GetComponentInChildren<PlayerEquipment>();

		syringeGun.GetComponent<IInteractable>().Svr_PerformInteract(transform.root.gameObject);
		//playerEquipment?.Svr_Equip(syringeGun, EquipmentType.None);
	}
}
