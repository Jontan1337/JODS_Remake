using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SyringeGun : ProjectileWeapon
{
	[SerializeField] private GameObject syringe1;
	[SerializeField] private GameObject syringe2;
	protected override void Shoot()
	{
		base.Shoot();

		switch (currentAmmunition)
		{
			case 0:
				GetComponentInParent<ActiveSClass>().StartAbilityCooldownCo();
				Unbind();
				break;
			case 1:
				syringe1.SetActive(false);
				break;
			case 2:
				syringe2.SetActive(false);
				break;
			default:
				break;
		}
		//if (currentAmmunition == 0 && extraAmmunition == 0)
		//{
		//	GetComponentInParent<ActiveSClass>().StartAbilityCooldownCo();
		//	Unbind();
		//}
	}
	protected override void OnDropPerformed(InputAction.CallbackContext obj)
	{
		Unbind();
	}

	public override void Unbind()
	{
		if (currentAmmunition < maxCurrentAmmunition)
		{
			GetComponentInParent<ActiveSClass>().StartAbilityCooldownCo();
		}
		base.Unbind();
		Cmd_Destroy();
	}

	[Command]
	public void Cmd_Destroy()
	{
		StartCoroutine(DestroyWait());
		//NetworkServer.Destroy(gameObject);
	}
	IEnumerator DestroyWait()
	{
		yield return new WaitForSeconds(0.1f);
		NetworkServer.Destroy(gameObject);
	}
}
