using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActiveSClass : NetworkBehaviour, IDamagable
{
	[SyncVar(hook = nameof(SetSurvivorClassSettings))] public SurvivorClass sClass;
	private SurvivorController sController;

	[SerializeField] private SurvivorSO survivorSO;
	[SerializeField] private SkinnedMeshRenderer survivorRenderer = null;
	[Space]
	[Header("Stats")]
	[SerializeField] private int currentHealth = 100;
	[SerializeField] private int maxHealth = 100;
	[SerializeField] private int armor = 0;
	[SerializeField] private float abilityCooldown = 0;
	[SerializeField] private float abilityCooldownCount = 0;
	[SerializeField] private float movementSpeed = 0;

	[Space]
	[Header("Weapon Stats")]
	[SerializeField] private float reloadSpeed = 0;
	[SerializeField] private float accuracy = 0;
	[SerializeField] private float ammoCapacity = 0;

	[Header("UI References")]
	[SerializeField] private Slider healthBar;
	[SerializeField] private Slider armorBar;

	[Header("Events")]
	[SerializeField] private UnityEvent<float> onChangedHealth;

	private bool abilityIsReady = true;
	private bool isDead;

	public bool test;

	public int GetHealth => currentHealth;
	public bool IsDead => isDead;
	public int Health
    {
		get => currentHealth;
		private set
        {
            currentHealth = value;
			healthBar.value = currentHealth;
        }
    }
	public int Armor
	{
		get => armor;
		private set
		{
			armor = value;
			armorBar.value = armor;
		}
	}

	#region Serialization
	//public override bool OnSerialize(NetworkWriter writer, bool initialState)
	//{
	//	if (!initialState)
	//	{
	//		writer.WriteSurvivorClass(sClass);
	//	}
	//	else
	//	{
	//		writer.WriteSurvivorClass(sClass);
	//	}
	//	return true;
	//}
	//public override void OnDeserialize(NetworkReader reader, bool initialState)
	//{
	//	if (!initialState)
	//	{
	//		sClass = reader.ReadSurvivorClass();
	//	}
	//	else
	//	{
	//		sClass = reader.ReadSurvivorClass();
	//	}
	//}
	#endregion

	public override void OnStartAuthority()
	{
		if (test) SetSurvivorClass(survivorSO);
		JODSInput.Controls.Survivor.ActiveAbility.performed += ctx => Cmd_Ability();
	}

	#region ViewModel
	[TargetRpc]
	private void Rpc_InvokeOnChangedHealth(NetworkConnection target, float healthDifference)
    {
		onChangedHealth?.Invoke(healthDifference);
	}

	#endregion

	[Command]
	void Cmd_Ability()
	{
		if (abilityIsReady)
		{
			sClass.ActiveAbility();
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

	public void StartAbilityCooldownCo()
	{
		StartCoroutine(AbilityCooldown());
	}

	[ClientRpc]
	public void Rpc_SetSurvivorClass(string _class)
	{
		List<SurvivorSO> survivorSOList = PlayableCharactersManager.instance.GetAllSurvivors();
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
		if (survivorSO.abilityObject && newValue)
		{
			newValue.abilityObject = survivorSO.abilityObject;
		}

		maxHealth = survivorSO.maxHealth;
		currentHealth = maxHealth;
		armor = survivorSO.startingArmor;

		accuracy = survivorSO.accuracy;
		reloadSpeed = survivorSO.reloadSpeed;
		ammoCapacity = survivorSO.ammoCapacity;
		
		sController = GetComponent<SurvivorController>();
		movementSpeed = survivorSO.movementSpeed;
		sController.speed *= movementSpeed;
		
		abilityCooldown = survivorSO.abilityCooldown;
		abilityCooldownCount = abilityCooldown;
		
		survivorRenderer.material = survivorSO.survivorMaterial;
		survivorRenderer.sharedMesh = survivorSO.survivorMesh;

		healthBar.maxValue = maxHealth;
		healthBar.value = currentHealth;
		armorBar.value = armor;
	}

	[Command]
	private void Cmd_SpawnClass()
	{
		StartCoroutine(Spawnshit());
	}

	IEnumerator Spawnshit()
	{
		yield return new WaitForSeconds(0.2f);

		GameObject selectedClass = Instantiate(survivorSO.classScript);
		NetworkServer.Spawn(selectedClass, gameObject);
		selectedClass.transform.SetParent(gameObject.transform);

		sClass = selectedClass.GetComponent<SurvivorClass>();
	}


	public Teams Team => Teams.Player;
	[Server]
	public void Svr_Damage(int damage, Transform target = null)
	{
		if (armor > 0)
		{
			Health -= Mathf.RoundToInt(damage * 0.4f);
			Armor = Mathf.Clamp(Armor -= Mathf.RoundToInt(damage * 0.6f), 0, 100);
		}
		else
		{
			Health -= damage;
		}
		Health = Mathf.Clamp(Health, 0, maxHealth);
		if (Health <= 0)
		{
			isDead = true;
			Die();
		}
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		GetComponentInChildren<IHitter>()?.OnHit(hit);
	}

	void Die()
	{
		NetworkServer.Destroy(gameObject);
	}
}
