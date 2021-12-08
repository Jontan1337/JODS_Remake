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
	[SerializeField] private SkinnedMeshRenderer bodyRenderer = null;
	[SerializeField] private SkinnedMeshRenderer headRenderer = null;
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
	[SerializeField] private GameObject starterWeapon;

	[Header("UI References")]
	[SerializeField] private Slider healthBar = null;
	[SerializeField] private Slider healthLossBar = null;
	[SerializeField] private Slider armorBar = null;
	[SerializeField] private Image abilityCooldownUI = null;

	[Header("Events")]
	[SerializeField] private UnityEvent<float> onChangedHealth = null;

	private bool abilityIsReady = true;
	private bool isDead;

	public bool test;

	public bool IsDead => isDead;

	public float MovementSpeed => movementSpeed;

	public int GetHealth => currentHealth;
	public int Health
	{
		get => currentHealth;
		private set
		{
			int prevHealth = currentHealth;
			currentHealth = Mathf.Clamp(value, 0, maxHealth);
			healthBar.value = currentHealth;
			if (!healthLossBool)
			{
				StartCoroutine(HealthLossCo(prevHealth));
			}
			if (currentHealth <= 0)
			{
				isDead = true;
				Die();
			}
			if (isServer)
			{
				Rpc_SyncStats(connectionToClient, currentHealth, armor);
			}
		}
	}
	public int Armor
	{
		get => armor;
		private set
		{
			armor = value;
			armorBar.value = armor;
			if (isServer)
			{
				Rpc_SyncStats(connectionToClient, currentHealth, armor);
			}
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

	#region Ability Stuff

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
		abilityCooldownUI.fillAmount = 0;
		while (abilityCooldownCount > 0)
		{
			abilityCooldownCount -= Time.deltaTime;
			abilityCooldownUI.fillAmount += Time.deltaTime / abilityCooldown;
			yield return null;
		}
		abilityCooldownCount = abilityCooldown;
		abilityIsReady = true;
	}

	public void StartAbilityCooldownCo()
	{
		StartCoroutine(AbilityCooldown());
	}

	#endregion

	#region Class Stuff


	[ClientRpc]
	public void Rpc_SetSurvivorClass(string _class)
	{
		List<SurvivorSO> survivorSOList = PlayableCharactersManager.instance.GetAllSurvivors();
		foreach (SurvivorSO survivor in survivorSOList)
		{
			if (survivor.name == _class)
			{
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
		sController.walkSpeedMultiplier *= movementSpeed;
		sController.sprintSpeedMultiplier = sController.walkSpeedMultiplier * 2;
		GetComponent<SurvivorAnimationManager>().anim.speed = movementSpeed;

		abilityCooldown = survivorSO.abilityCooldown;
		abilityCooldownCount = abilityCooldown;

		bodyRenderer.sharedMesh = survivorSO.bodyMesh;
		headRenderer.sharedMesh = survivorSO.headMesh;

		bodyRenderer.material = survivorSO.characterMaterial;
		headRenderer.material = survivorSO.characterMaterial;

		healthBar.maxValue = maxHealth;
		healthBar.value = currentHealth;
		healthLossBar.maxValue = maxHealth;
		armorBar.value = armor;
	}

	[Command]
	private void Cmd_SpawnClass()
	{
		StartCoroutine(SpawnClassObjects());
	}

	IEnumerator SpawnClassObjects()
	{
		yield return new WaitForSeconds(0.2f);

		GameObject selectedClass = Instantiate(survivorSO.classScript);
		NetworkServer.Spawn(selectedClass, gameObject);
		selectedClass.transform.SetParent(gameObject.transform);

		sClass = selectedClass.GetComponent<SurvivorClass>();

		if (survivorSO.starterWeapon)
		{
			starterWeapon = Instantiate(survivorSO.starterWeapon, transform.position, transform.rotation);
			NetworkServer.Spawn(starterWeapon);
			yield return new WaitForSeconds(0.35f);
			starterWeapon.GetComponent<EquipmentItem>().Svr_Interact(gameObject);
		}
	}
	#endregion

	#region Health Stuff

	private bool healthLossBool = false;

	private IEnumerator HealthLossCo(int prevHealth)
	{
		healthLossBool = true;
		healthLossBar.value = prevHealth;
		yield return new WaitForSeconds(1);
		while (healthLossBar.value > Health)
		{
			healthLossBar.value -= Time.deltaTime * 15;

			yield return null;
		}
		healthLossBool = false;
	}


	public Teams Team => Teams.Player;
	[Server]
	public void Svr_Damage(int damage, Transform target = null)
	{
		if (armor > 0)
		{
			float armorPercent = (float)armor / 100;

			int armorLoss = Mathf.Clamp(Mathf.RoundToInt(damage * armorPercent), 1, 100);
			int healthLoss = damage - armorLoss;

			Health -= healthLoss;
			Armor = Mathf.Clamp(Armor -= armorLoss, 0, 100);
		}
		else
		{
			Health -= damage;
		}
	}

	[TargetRpc]
	public void Rpc_SyncStats(NetworkConnection target, int newHealth, int newArmor)
	{
		if (isServer) return;
		Health = newHealth;
		Armor = newArmor;
	}

	#endregion


	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		GetComponentInChildren<IHitter>()?.OnFlyingKickHit(hit);
	}

	void Die()
	{
		NetworkServer.Destroy(gameObject);
	}

	private void OnGUI()
	{
		if (test)
		{
			GUI.TextField(new Rect(20, 20, 150, 20), "Active S Class Test ON");
		}
	}
}
