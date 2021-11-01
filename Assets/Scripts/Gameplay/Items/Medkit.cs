using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class Medkit : EquipmentItem
{
	[SerializeField] private int healAmount = 69;
	[SerializeField] private int uses = 3;

	protected override void OnLMBPerformed(InputAction.CallbackContext obj) => UseMedkit();


	public void UseMedkit()
	{
		ActiveSClass activeSClass = GetComponentInParent<ActiveSClass>();
		if (activeSClass.Health != 100)
		{
			GetComponentInParent<ActiveSClass>().Svr_Damage(-healAmount);

			uses--;
			if (uses == 0)
			{
				UsedUp();
			}
		}
	}

	private void UsedUp()
	{
		Unbind();
		Cmd_InvokeOnDrop();
		Cmd_DestroyGameObject();
	}

	[Command]
	private void Cmd_DestroyGameObject()
	{
		NetworkServer.Destroy(gameObject);
	}

}
