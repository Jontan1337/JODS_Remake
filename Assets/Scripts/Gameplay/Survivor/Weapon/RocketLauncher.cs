using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class RocketLauncher : EquipmentItem
{
	public GameObject rocket;
	private int rocketSpeed = 5000;

	[Command]
	void CmdRocketLaunch()
	{
		GameObject currentRocket = Instantiate(rocket, transform.position, transform.rotation);
		NetworkServer.Spawn(currentRocket);
		RpcRocketLaunch(currentRocket);
	}

	[ClientRpc]
	void RpcRocketLaunch(GameObject currentRocket)
	{
		Rigidbody rb = currentRocket.GetComponent<Rigidbody>();
		rb.AddForce(transform.forward * rocketSpeed);
	}

	protected override void OnLMBPerformed(InputAction.CallbackContext obj)
	{
		CmdRocketLaunch();
		GetComponentInParent<ActiveSClass>()?.StartAbilityCooldownCo();
		Unbind();
		Invoke("Cmd_DestroyGameObject", 1f);
	}
	protected override void OnDropPerformed(InputAction.CallbackContext obj)
	{
		Unbind();
		Cmd_DestroyGameObject();
	}

	[Command]
	private void Cmd_DestroyGameObject()
	{
		NetworkServer.Destroy(gameObject);
	}
}
