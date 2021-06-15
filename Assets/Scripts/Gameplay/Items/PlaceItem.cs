using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlaceItem : EquipmentItem
{

	[SerializeField] private UnityEvent OnPlaced;
	[SerializeField] private GameObject placeholder;
	[SerializeField] private LayerMask ignoreLayer;
	[SerializeField] private float maxPlaceRange = 3;
	[SerializeField, Range(0, 1)] private float maxSlopeAngle = 0.965f;

	private LookController look;

	// Called when item is picked up and ready to be placed
	public void Equipped()
	{
		placeholder.SetActive(true);
		look = GetComponentInParent<LookController>();
		PlaceHolderActiveCo = PlaceHolderActive();
		StartCoroutine(PlaceHolderActiveCo);
	}

	// This coroutine makes the placeholder appear in front of the player
	IEnumerator PlaceHolderActiveCo;
	public IEnumerator PlaceHolderActive()
	{

		while (true)
		{

			// If anything is within 'maxPlaceRange' meters in front of the player, 
			// the placeholder will be positioned at the object.
			// Otherwise the placeholder will be placed 'maxPlaceRange' meters in front of the player.
			if (Physics.Raycast(look.playerCamera.transform.position, look.playerCamera.transform.forward, out RaycastHit hit, maxPlaceRange, ~ignoreLayer))
			{
				placeholder.transform.position = PlaceholderPos(hit);
			}
			else
			{
				placeholder.transform.position = look.playerCamera.transform.position + look.playerCamera.transform.forward * maxPlaceRange;
			}

			// Raycast pointing downwards from the placeholder.
			// If the raycast hits anything the placeholder will be positioned at the object.
			// If downwards facing raycast doesn't hit anything, the placeholder is inactive. Obstructed is set to false, since it doesn't do OnTriggerExit.
			if (Physics.Raycast(placeholder.transform.position, -Vector3.up, out RaycastHit hitDown, maxPlaceRange, ~ignoreLayer))
			{
				placeholder.transform.position = PlaceholderPos(hitDown);
			}
			else
			{
				placeholder.GetComponent<ItemPlaceholder>().Obstructed(false);
			}
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
	public void Place()
	{
		if (!placeholder.GetComponent<ItemPlaceholder>().obstructed && placeholder.activeSelf)
		{
			OnPlaced?.Invoke();
			transform.position = placeholder.transform.position;
			transform.rotation = placeholder.transform.rotation;
			transform.parent = null;
			StopCoroutine(PlaceHolderActiveCo);
			authController.Svr_RemoveAuthority();
			Unbind();
			Destroy(placeholder);
		}
	}
	protected override void OnLMBPerformed(InputAction.CallbackContext obj)
	{
		Place();
	}

	protected override void OnDropPerformed(InputAction.CallbackContext obj)
	{
		StopAllCoroutines();
		Unbind();
		Cmd_DestroyGameObject();
	}

	[Command]
	private void Cmd_DestroyGameObject()
	{
		NetworkServer.Destroy(gameObject);

	}

	[Server]
	public override void Svr_Interact(GameObject interacter)
	{
		base.Svr_Interact(interacter);
		Equipped();
	}


	// Makes a child object with identical appearance. 
	[ContextMenu("Create Placeholder")]
	void CreatePlaceHolder()
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
