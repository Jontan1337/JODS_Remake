using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlaceItem : EquipmentItem
{

	[SerializeField] private UnityEvent OnPlaced = null;
	[SerializeField] private GameObject placeholder = null;
	[SerializeField] private LayerMask ignoreLayer = 0;
	[SerializeField] private float maxPlaceRange = 3;
	[SerializeField, Range(0, 1)] private float maxSlopeAngle = 0.965f;

	private LookController look;

	// Called when item is picked up and ready to be placed
	[TargetRpc]
	public void Equipped(NetworkConnection target, GameObject player)
	{
		placeholder.SetActive(true);
		look = player.GetComponent<LookController>();
		PlaceHolderActiveCo = PlaceHolderActive();
		StartCoroutine(PlaceHolderActiveCo);
	}

	// This coroutine makes the placeholder appear in front of the player
	IEnumerator PlaceHolderActiveCo;
	public IEnumerator PlaceHolderActive()
	{
		placeholder.GetComponent<ItemPlaceholder>().Obstructed(false);
		while (true)
		{
			// Raycast pointing forward relative to the players camera.
			if (Physics.Raycast(look.playerCamera.transform.position, look.playerCamera.transform.forward, out RaycastHit hit, maxPlaceRange, ~ignoreLayer))
			{
				// If anything is within 'maxPlaceRange' meters in front of the player, the placeholder will be positioned at the object.
				placeholder.transform.position = PlaceholderPos(hit);
			}
			else
			{
				// Otherwise the placeholder will be placed 'maxPlaceRange' meters in front of the player.
				placeholder.transform.position = look.playerCamera.transform.position + look.playerCamera.transform.forward * maxPlaceRange;
			}

			// Raycast pointing downwards from the placeholder.
			if (Physics.Raycast(placeholder.transform.position, -Vector3.up, out RaycastHit hitDown, maxPlaceRange, ~ignoreLayer))
			{
				// If the downwards facing raycast hits anything, the placeholder will be positioned at the object.
				placeholder.transform.position = PlaceholderPos(hitDown);
			}
			else
			{
				// if the raycast doesn't hit anything, obstructed is set to false here since OnTriggerExit isn't triggered.
				placeholder.GetComponent<ItemPlaceholder>().Obstructed(false);
			}
			// If downwards facing raycast doesn't hit anything, the placeholder is inactive.
			placeholder.gameObject.SetActive(hitDown.transform);

			// Rotates the placeholder to always stand upright, even on slopes.
			placeholder.transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
			if (hitDown.normal.y > maxSlopeAngle)
			{
				placeholder.transform.rotation = Quaternion.FromToRotation(placeholder.transform.up, hitDown.normal) * placeholder.transform.rotation;
			}
			yield return null;
		}
	}

	// The y position is raised slightly to avoid the placeholder overlapping with the ground
	Vector3 PlaceholderPos(RaycastHit hit)
	{
		return new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z);
	}


	// If the placeholder isn't obstructed, replaces the placeholder with the item, and removes the placeholder.
	[Command]
	public void Cmd_Place(bool obstructed, bool placeholderActive, Vector3 placeholderPos, Quaternion placeholderRot)
	{
		if (!obstructed && placeholderActive)
		{
			Rpc_Cleanup(connectionToClient);
			OnPlaced?.Invoke();
			transform.position = placeholderPos;
			transform.rotation = placeholderRot;
			transform.parent = null;
			//Unbind();
			Svr_EnablePhysics();			

			authController.Svr_RemoveAuthority();
			Svr_InvokeOnDrop();
		}
	}

	[TargetRpc]
	private void Rpc_Cleanup(NetworkConnection target)
	{
		StopCoroutine(PlaceHolderActiveCo);
		placeholder.SetActive(false);
	}

	protected override void OnLMBPerformed(InputAction.CallbackContext obj)
	{
		Cmd_Place(placeholder.GetComponent<ItemPlaceholder>().obstructed, placeholder.activeSelf, placeholder.transform.position, placeholder.transform.rotation);
	}

	// When the item is dropped, all coroutines are stopped, the placeholder is deactivated and the item is destroyed or dropped, depending on a bool in EquipmentItem.
	protected override void OnDropPerformed(InputAction.CallbackContext obj)
	{
		// Kalder Cmd_Drop, som kalder Svr_Unequip. Det resulterer i at 'Special' bliver kørt 2 gange... Skulle blive fixet via ændring i equipment
		Drop(true);
	}
	public override void Svr_Unequip()
	{
		Drop(false);
	}

	public void Drop(bool dropItem)
	{
		if (connectionToClient != null)
		{
			Rpc_Cleanup(connectionToClient);
		}
		switch (equipmentType)
		{
			case EquipmentType.None:
				base.Unbind();
				Cmd_DestroyGameObject();
				break;
			case EquipmentType.Special:
				if (dropItem)
				{
					Cmd_Drop();
				}
				else
				{
					//base.Svr_Unequip();
				}
				break;
			default:
				break;
		}
	}

	[Command]
	private void Cmd_DestroyGameObject()
	{
		NetworkServer.Destroy(gameObject);
	}
	public override void Svr_Equip()
	{
		base.Svr_Equip();
		Equipped(connectionToClient, transform.root.gameObject);
	}

	// Makes a child object with identical appearance. 
	[ContextMenu("Create Placeholder")]
	private void CreatePlaceHolder()
	{
		GameObject placeHolderChildParentObject = new GameObject();
		placeHolderChildParentObject.transform.SetParent(transform);
		placeHolderChildParentObject.name = "PlaceholderParent";
		placeHolderChildParentObject.AddComponent<ItemPlaceholder>();
		placeHolderChildParentObject.AddComponent<BoxCollider>().isTrigger = true;
		placeHolderChildParentObject.GetComponent<BoxCollider>().size = GetComponent<BoxCollider>().size;
		placeHolderChildParentObject.GetComponent<BoxCollider>().center = GetComponent<BoxCollider>().center;
		placeHolderChildParentObject.AddComponent<Rigidbody>().isKinematic = true;

		MeshFilter[] childFilters = GetComponentsInChildren<MeshFilter>();
		MeshRenderer[] childRenderes = GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < childFilters.Length; i++)
		{
			GameObject placeHolderChildObject = new GameObject();
			placeHolderChildObject.transform.SetParent(placeHolderChildParentObject.transform);
			placeHolderChildObject.AddComponent<MeshFilter>().sharedMesh = childFilters[i].sharedMesh;
			placeHolderChildObject.AddComponent<MeshRenderer>().sharedMaterials = childRenderes[i].sharedMaterials;
			placeHolderChildObject.transform.position = childFilters[i].transform.position;
			placeHolderChildObject.name = childRenderes[i].name + "_Mesh";
		}
	}
}
