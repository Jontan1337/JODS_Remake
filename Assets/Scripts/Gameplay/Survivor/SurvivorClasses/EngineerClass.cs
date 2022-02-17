using UnityEngine;
using Mirror;
using System.Collections;

public class EngineerClass : SurvivorClass
{

	private PlayerEquipment playerEquipment;
	private GameObject turret;
	private SurvivorController sController;

	private bool recharging = false;


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

	private void OnTransformParentChanged()
	{
		if (hasAuthority || isServer)
		{
			sController = GetComponentInParent<SurvivorController>();
		}
	}
	public override void ActiveAbility()
	{
		if (!turret)
		{
			Cmd_EquipTurret();
		}
	}

    public override void ActiveAbilitySecondary()
    {
        if (!recharging)
        {
            StartRechargeCo = StartRecharge();
            StartCoroutine(StartRechargeCo);
        }
        else
        {
            StopCoroutine(StartRechargeCo);
            StopRechargeCo = StopRecharge();
            StartCoroutine(StopRechargeCo);
        }
    }

    IEnumerator StartRechargeCo;
	private IEnumerator StartRecharge()
    {
		sController.enabled = false;
		recharging = true;
		yield return new WaitForSeconds(2f);
        while (true)
        {
			yield return new WaitForSeconds(1f);
			GetComponentInParent<ActiveSClass>().abilityCooldownCount += 1;
		}
	}

	IEnumerator StopRechargeCo;
	private IEnumerator StopRecharge()
	{
		yield return new WaitForSeconds(2f);
		sController.enabled = true;
		recharging = false;
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

        //turret.GetComponent<IInteractable>().Svr_Interact(transform.root.gameObject);
        turret.GetComponent<EquipmentItem>().Svr_Pickup(playerEquipment.playerHands, connectionToClient);
        playerEquipment?.Svr_Equip(turret, EquipmentType.None);
    }

}
