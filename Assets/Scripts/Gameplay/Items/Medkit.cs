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
			float healy = healAmount;

			// Change to heal method things.
			//activeSClass.Health = Mathf.Clamp(activeSClass.Health + healAmount, 0, 100);

			uses--;
			if (uses == 0)
			{
				UsedUp();
			}
		}

		print(uses);
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
