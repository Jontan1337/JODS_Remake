using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SyringeGun : ProjectileWeapon
{
	[SerializeField]
	private List<GameObject> syringes = new List<GameObject>();
	//[SerializeField] private GameObject syringe1;
	//[SerializeField] private GameObject syringe2;
	protected override void Shoot(Vector2 aimPoint)
	{
		base.Shoot(aimPoint);

		if(syringes.Count == 0)
        {
			GetComponentInParent<ActiveSClass>().Rpc_StartAbilityCooldown(transform.root.GetComponent<NetworkIdentity>().connectionToClient, transform.root);
			Unbind();
			return;
		}

		syringes[0].SetActive(false);
		syringes.RemoveAt(0);
	}
	protected override void OnDropPerformed(InputAction.CallbackContext obj)
	{
		Unbind();
	}

	public override void Unbind()
	{
		if (currentAmmunition < maxCurrentAmmunition)
		{
			GetComponentInParent<ActiveSClass>().Rpc_StartAbilityCooldown(transform.root.GetComponent<NetworkIdentity>().connectionToClient, transform.root);
		}
		base.Unbind();
		Cmd_Destroy();
	}

	[Command]
	public void Cmd_Destroy()
	{
		StartCoroutine(DestroyWait());
	}
	IEnumerator DestroyWait()
	{
		yield return new WaitForSeconds(0.1f);
		NetworkServer.Destroy(gameObject);
	}
}
