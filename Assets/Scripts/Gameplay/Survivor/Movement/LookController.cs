using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookController : NetworkBehaviour
{
	float minRotY = -75f;
	float maxRotY = 75F;
	public float rotY;
	public float rotX;
	public float sensitivity;
	private bool canLook = true;
	public Camera playerCamera;
	[SerializeField]
	private Transform rotateHorizontal = null;
	[SerializeField]
	private Transform rotateVertical = null;

	#region NetworkBehaviour Callbacks

	public override void OnStartAuthority()
	{
		transform.root.GetComponent<PlayerSetup>().onSpawnItem += GetCamera;
		Cursor.lockState = CursorLockMode.Locked;
	}
	public override void OnStartClient()
	{
        if (hasAuthority)
        {
			JODSInput.Controls.Survivor.Camera.performed += Look;
        }
	}
	public override void OnStopAuthority()
	{
		transform.root.GetComponent<PlayerSetup>().onSpawnItem -= GetCamera;

	}
	public override void OnStopClient()
	{
        if (hasAuthority)
        {
			JODSInput.Controls.Survivor.Camera.performed -= Look;
        }
	}
	#endregion

	void Look(InputAction.CallbackContext context)
	{
		Vector2 mouseDelta = context.ReadValue<Vector2>();
		if (!canLook)
		{
			return;
		}
		rotX += mouseDelta.x * sensitivity * Time.deltaTime;
		rotY += mouseDelta.y * sensitivity * Time.deltaTime;
		rotY = Mathf.Clamp(rotY, minRotY, maxRotY);
		//transform.Rotate(0, rotX, 0);
		rotateHorizontal.rotation = Quaternion.Euler(0, rotX, 0);
		rotateVertical.rotation = Quaternion.Euler(-rotY, rotX, 0f);
	}

	public void EnableLook()
	{
		canLook = true;
	}
	public void DisableLook()
	{
		rotY = 0;
		rotateVertical.rotation = Quaternion.Euler(0f, rotX, 0f);
		canLook = false;
	}

	private void GetCamera(GameObject item)
	{
		if (item.TryGetComponent(out ItemName itemName))
		{
			switch (itemName.itemName)
			{
				case ItemNames.VirtualHead:
					rotateVertical = item.transform;
					break;
				case ItemNames.Camera:
					playerCamera = item.GetComponent<Camera>();
					break;
				default:
					break;
			}
		}
	}
}
