using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class LookController : NetworkBehaviour
{
	float minRotY = -75f;
	float maxRotY = 75F;
	public float rotY;
	public float rotX;
	public float sensitivity;
	private bool canLook = true;
	public Camera playerCamera;
	public Camera playerItemCamera;
	[SerializeField] private Transform rotateHorizontal = null;
	[SerializeField] private Transform rotateVertical = null;
	[SerializeField] private CameraSettings cameraSettings;

	private PlayerEquipment playerEquipment;

	public Transform RotateVertical => rotateVertical;

	#region NetworkBehaviour Callbacks

	public override void OnStartAuthority()
	{
		transform.root.GetComponent<SurvivorSetup>().onSpawnItem += GetInitialItems;
		Cursor.lockState = CursorLockMode.Locked;
	}
	public override void OnStartClient()
	{
        if (hasAuthority)
        {
			
        }
	}
	public override void OnStopAuthority()
	{
		transform.root.GetComponent<SurvivorSetup>().onSpawnItem -= GetInitialItems;

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
		// This is pointless since JODSInput is meant for toggling controls.
		if (!canLook)
		{
			return;
		}
		rotX += mouseDelta.x * sensitivity;
		rotY += mouseDelta.y * sensitivity;
		rotY = Mathf.Clamp(rotY, minRotY, maxRotY);
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

	private void CameraShake(float amount)
    {
		playerCamera.DOComplete();
        playerCamera.DOShakeRotation(0.1f, 0.2f * amount, 1, 0f, true);
    }

	private void GetImpacter(GameObject oldObject, GameObject newObject)
    {
        if (oldObject)
        {
            if (oldObject.TryGetComponent(out IImpacter impacter))
            {
				impacter.OnImpact -= CameraShake;
            }
        }
        if (newObject)
        {
			if (newObject.TryGetComponent(out IImpacter impacter))
			{
				impacter.OnImpact += CameraShake;
			}
		}
    }

	private void GetInitialItems(GameObject item)
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
					playerItemCamera = item.GetComponentInChildren<Camera>();
					cameraSettings = playerCamera.GetComponent<CameraSettings>();
					JODSInput.Controls.Survivor.Camera.performed += Look;
					Cmd_SetVirtualHead(rotateVertical);
					break;
				case ItemNames.Equipment:
					playerEquipment = item.GetComponent<PlayerEquipment>();
                    playerEquipment.onClientEquippedItemChange += GetImpacter;
					break;
				default:
					break;
			}
		}
	}

	[Command]
	private void Cmd_SetVirtualHead(Transform virtualHead)
    {
		rotateVertical = virtualHead;
    }
}
