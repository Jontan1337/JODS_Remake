using UnityEngine;
using Mirror;
using System.Collections;

public class EngineerClass : SurvivorClass
{

	private PlayerEquipment playerEquipment;
	private GameObject turret;
	private SurvivorController sController;
	private ActiveSClass sClass;
	private ModifierManager modifierManager;

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
			sClass = GetComponentInParent<ActiveSClass>();
			modifierManager = GetComponentInParent<ModifierManager>();
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

	[SerializeField] private float range = 30;
	[SerializeField] private LayerMask survivorLayer = 0;
	Collider[] survivorsInRange;
	IEnumerator StartRechargeCo;
	private IEnumerator StartRecharge()
    {
		survivorsInRange = Physics.OverlapSphere(transform.position, range, survivorLayer);
		sController.enabled = false;
		recharging = true;
		yield return new WaitForSeconds(2f);
        foreach (var item in survivorsInRange)
        {
			item.GetComponentInParent<ModifierManager>().Cooldown = 2;
			print(item.gameObject.name);
        }
		//modifierManager.Cooldown = 2;

		while (!sClass.AbilityIsReady)
        {
			yield return null;
		}
		StartCoroutine(StopRecharge());
	}

	IEnumerator StopRechargeCo;
	private IEnumerator StopRecharge()
	{
		foreach (var item in survivorsInRange)
		{
			item.GetComponentInParent<ModifierManager>().Cooldown = 1;
		}
		survivorsInRange = null;
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

        turret.GetComponent<IInteractable>().Svr_Interact(transform.root.gameObject);
        //turret.GetComponent<EquipmentItem>().Svr_Pickup(playerEquipment.playerHands, connectionToClient);
        //playerEquipment?.Svr_Equip(turret, EquipmentType.None);
    }

}
