using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSClass : NetworkBehaviour, IDamagable
{
	[SyncVar(hook = nameof(SetSurvivorClassSettings))] public SurvivorClass sClass;
	private SurvivorController sController;

	[SerializeField] private SurvivorSO survivorSO;
	[SerializeField] private SkinnedMeshRenderer survivorRenderer;
	[Space]
	[Header("Stats")]
	[SerializeField] private int health = 100;
	[SerializeField] private int armor = 0;
	[SerializeField] private float abilityCooldown = 0;
	[SerializeField] private float abilityCooldownCount = 0;
	[SerializeField] private float movementSpeed = 0;

	[Space]
	[Header("Weapon Stats")]
	[SerializeField] private float reloadSpeed = 0;
	[SerializeField] private float accuracy = 0;
	[SerializeField] private float ammoCapacity = 0;

	private bool abilityIsReady = true;
	private bool isDead;

	public bool test;
	/*
	#region Serialization

	public override bool OnSerialize(NetworkWriter writer, bool initialState)
	{
		if (!initialState)
		{
			writer.WriteSurvivorClass(sClass);
			return true;
		}
		else
		{
			writer.WriteSurvivorClass(sClass);
			return true;
		}
	}
	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		if (!initialState)
		{
			reader.ReadSurvivorClass();
		}
		else
		{
			reader.ReadSurvivorClass();
		}
	}
	#endregion
	*/


	private void Start()
	{
		if (test) SetSurvivorClass(survivorSO);

		JODSInput.Controls.Survivor.ActiveAbility.performed += ctx => Cmd_Ability();

	}

	[Command]
	void Cmd_Ability()
	{
		if (abilityIsReady)
		{
			if (!sClass.abilityIsToggled)
			{
				sClass.ActiveAbility();
				if (sClass.abilityActivatedSuccesfully)
				{
					StartCoroutine(AbilityCooldown());
                    sClass.abilityActivatedSuccesfully = false;
				}
			}
			else
			{
				sClass.ActiveAbility();
				abilityIsReady = false;
			}
		}
	}

	public IEnumerator AbilityCooldown()
	{
		abilityIsReady = false;
		while (abilityCooldownCount > 0)
		{
			abilityCooldownCount -= 1;
			yield return new WaitForSeconds(1);
		}
		abilityCooldownCount = abilityCooldown;
		abilityIsReady = true;
	}

	[ClientRpc]
	public void Rpc_SetSurvivorClass(string _class)
    {
		List<SurvivorSO> survivorSOList = PlayableCharactersManager.instance.survivorSOList;
		print("Rpc_SetSurvivorClass");
	    foreach (SurvivorSO survivor in survivorSOList)
        {
            if (survivor.name == _class)
            {
				print(survivor.name);
				SetSurvivorClass(survivor);
                break;
            }
        }
	}

	public void SetSurvivorClass(SurvivorSO survivorSO)
	{
		this.survivorSO = survivorSO;
        if (hasAuthority)
        {
			Cmd_SpawnClass();
        }
	}

	private void SetSurvivorClassSettings(SurvivorClass oldValue, SurvivorClass newValue)
    {
		if (survivorSO.abilityObject)
		{
			newValue.abilityObject = survivorSO.abilityObject;
		}

		armor = survivorSO.armor;
		health = survivorSO.health;
		accuracy = survivorSO.accuracy;
		reloadSpeed = survivorSO.reloadSpeed;
		ammoCapacity = survivorSO.ammoCapacity;
		movementSpeed = survivorSO.movementSpeed;
		abilityCooldown = survivorSO.abilityCooldown;
		survivorRenderer.material = survivorSO.survivorMaterial;
		survivorRenderer.sharedMesh = survivorSO.survivorMesh;

		abilityCooldownCount = abilityCooldown;

		sController = GetComponent<SurvivorController>();
		sController.speed *= movementSpeed;
	}

	[Command]
	private void Cmd_SpawnClass()
	{
		GameObject selectedClass = Instantiate(survivorSO.classScript);
		selectedClass.transform.SetParent(gameObject.transform);

		NetworkServer.Spawn(selectedClass, gameObject);
		sClass = selectedClass.GetComponent<SurvivorClass>();
	}


	public Teams Team => Teams.Player;
	[Server]
	public void Svr_Damage(int damage, Transform target = null)
	{
		if (armor > 0) armor -= damage;
		else health -= damage;
		if (health <= 0)
		{
			isDead = true;
			Die();
		}
	}

	void Die()
	{
		NetworkServer.Destroy(gameObject);
	}

	public int GetHealth() => health;

	public bool IsDead() => isDead;
}
