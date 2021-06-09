using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlaceItem : EquipmentItem
{

	[SerializeField]
	private UnityEvent OnPlaced;
	[SerializeField]
	private GameObject placeHolder;
	[SerializeField]
	private LayerMask ignoreLayer;
	[SerializeField]
	private float maxPlaceRange = 3;

	private LookController look;
	public bool placeholderActive = true;


	// Called when item is picked up and ready to be placed
	public void Equipped()
	{
		placeHolder.SetActive(true);
		//look = transform.root.GetComponent<LookController>();
		look = GetComponentInParent<LookController>();
		print(look.transform.name);
		PlaceHolderActiveCo = PlaceHolderActive();
		StartCoroutine(PlaceHolderActiveCo);
	}


	IEnumerator PlaceHolderActiveCo;

	public IEnumerator PlaceHolderActive()
	{
		while (true)
		{
			// Rotates the placeholder to always stand upright
			placeHolder.transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);

			// If anything is within 3 meters in front of the player, 
			// the placeholder will be positioned at the object
			// otherwise the placeholder will be placed on the ground, 3 meters in front of the player
			if (Physics.Raycast(look.playerCamera.transform.position, look.playerCamera.transform.forward, out RaycastHit hit, maxPlaceRange, ~ignoreLayer))
			{
				placeHolder.transform.position = PlaceholderPos(hit);
			}
			else
			{
				placeHolder.transform.position = look.playerCamera.transform.position + look.playerCamera.transform.forward * maxPlaceRange;
			}


			if (Physics.Raycast(placeHolder.transform.position, -Vector3.up, out RaycastHit hitDown, maxPlaceRange, ~ignoreLayer))
			{

				placeHolder.transform.position = PlaceholderPos(hitDown);
			}
			else
			{
				placeHolder.gameObject.SetActive(false);
			}

			// TO DO - DEACTIVATE GAMEOBJECT WHEN LOOKING UP??

			yield return null;
		}
	}

	Vector3 PlaceholderPos(RaycastHit hit)
	{
		return new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z);
	}


	public void Place()
	{
		print("placed");
		if (!placeHolder.GetComponent<ItemPlaceholder>().obstructed)
		{
			OnPlaced?.Invoke();

			transform.position = placeHolder.transform.position;
			transform.rotation = placeHolder.transform.rotation;
			transform.parent = null;
			StopCoroutine(PlaceHolderActiveCo);
			//TEMP FIX
			authController.Svr_RemoveAuthority();
			Unbind();
			Destroy(placeHolder);

		}
	}
	protected override void OnLMBPerformed(InputAction.CallbackContext obj)
	{
		Place();
	}


	protected override void OnDropPerformed(InputAction.CallbackContext obj)
	{
		NetworkServer.Destroy(gameObject);
		//Cmd_DestroyGameObject();
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
