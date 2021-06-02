using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSClass : NetworkBehaviour, IDamagable
{
	public SurvivorClass sClass;
	private SurvivorController sController;
	private Object classScript;

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

	//public GameObject starterWeapon;

	private void Start()
	{
		SetSurvivorClass(survivorSO);
		JODSInput.Controls.Survivor.ActiveAbility.performed += ctx => Cmd_Ability();

	}

	[Command]
	void Cmd_Ability()
	{
		if (abilityIsReady)
		{
			if (!sClass.abilityIsToggled)
			{
				print(sClass.abilityActivatedSuccesfully);
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
				StartCoroutine(ToggleAbility());
			}
		}
	}

	IEnumerator ToggleAbility()
	{
		print(sClass.abilityActivatedSuccesfully);
		while (!sClass.abilityActivatedSuccesfully)
		{
			yield return null;
		}
		StartCoroutine(AbilityCooldown());
	}

	IEnumerator AbilityCooldown()
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

	public void SetSurvivorClass(SurvivorSO survivorSO)
	{
		print(survivorSO);
		this.survivorSO = survivorSO;
		sClass = SelectedClass();
		if (survivorSO.abilityObject)
		{
			sClass.abilityObject = survivorSO.abilityObject;
		}
		armor = survivorSO.armor;
		health = survivorSO.health;
		accuracy = survivorSO.accuracy;
		reloadSpeed = survivorSO.reloadSpeed;
		ammoCapacity = survivorSO.ammoCapacity;
		movementSpeed = survivorSO.movementSpeed;
		abilityCooldown = survivorSO.abilityCooldown;
		abilityCooldownCount = abilityCooldown;

		sController = GetComponent<SurvivorController>();
		sController.speed *= movementSpeed;
	}

	SurvivorClass SelectedClass()
	{
		System.Type selectedClass = System.Type.GetType(survivorSO.classScript.name + ",Assembly-CSharp");

		survivorRenderer.material = survivorSO.survivorMaterial;
		survivorRenderer.sharedMesh = survivorSO.survivorMesh;
		return sClass = (SurvivorClass)gameObject.AddComponent(selectedClass);
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
