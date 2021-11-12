﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class LookController : NetworkBehaviour
{
	public Camera playerCamera;
	public Camera playerItemCamera;
	[SerializeField] private FirstPersonLookController firstPersonLookController;
	[SerializeField] private Transform rotateHorizontal = null;
	[SerializeField] private Transform rotateVertical = null;
	[SerializeField] private CameraSettings cameraSettings;

	private PlayerEquipment playerEquipment;

	public Transform RotateVertical => rotateVertical;

	#region NetworkBehaviour Callbacks

	public override void OnStartAuthority()
	{
		transform.root.GetComponent<SurvivorSetup>().onClientSpawnItem += GetInitialItems;
	}
    public override void OnStartClient()
    {
        if (hasAuthority)
        {
			firstPersonLookController.Bind();
			firstPersonLookController.HideCursor();
        }
    }
    public override void OnStopAuthority()
	{
		transform.root.GetComponent<SurvivorSetup>().onClientSpawnItem -= GetInitialItems;
	}
	public override void OnStopClient()
	{
		if (hasAuthority)
		{
			firstPersonLookController.Unbind();
			firstPersonLookController.ShowCursor();
		}
	}
    #endregion

    private void LateUpdate()
    {
		if (!hasAuthority) return;
		firstPersonLookController.DoRotation();
    }

    private void CameraShake(float amount)
    {
        //playerCamera.DOComplete();
        //playerCamera.DOShakeRotation(0.1f, 0.2f * amount, 10, 10f, false);
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
					firstPersonLookController.rotateHorizontal = rotateHorizontal;
					firstPersonLookController.rotateVertical = rotateVertical;
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
