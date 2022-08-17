using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class Throwable : EquipmentItem
{
	[Header("Throwable Settings")]
	[SerializeField] private int projectileSpeed = 20;


	protected override void OnLMBPerformed(InputAction.CallbackContext obj)
	{
		GetComponent<Projectile>().owner = transform.root;
		
		Cmd_Throw();		
	}

	[Command]
    public void Cmd_Throw()
    {
		transform.SetParent(null);
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.AddForce(transform.forward * (projectileSpeed * rb.mass), ForceMode.Impulse);
		rb.AddTorque(new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100)));
		Rpc_SetLayer(connectionToClient, false);

		GetComponent<Projectile>().Activate();
		Svr_InvokeOnDrop();
	}
}
