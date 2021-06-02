using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlaceItem : NetworkBehaviour, IEquippable, IBindable, IInteractable
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
	private AuthorityController authController;
	private bool isInteractable = true;
	public bool placeholderActive = true;


	public string Name => throw new System.NotImplementedException();
	public string ObjectName => gameObject.name;
	public GameObject Item => gameObject;
	public EquipmentType EquipmentType => EquipmentType.None;
	public bool IsInteractable
	{
		get => isInteractable;
		set => isInteractable = value;
	}
	private void Start()
	{
		authController = GetComponent<AuthorityController>();
	}

	// Called when item is picked up and ready to be placed
	public void Equipped()
	{
		placeHolder.SetActive(true);
		look = GetComponentInParent<LookController>();
		PlaceHolderActiveCo = PlaceHolderActive();
		StartCoroutine(PlaceHolderActiveCo);
	}

	IEnumerator PlaceHolderActiveCo;
	public IEnumerator PlaceHolderActive()
	{
		while (true)
		{
			Physics.Raycast(look.playerCamera.transform.position, look.playerCamera.transform.forward, out RaycastHit hit, maxPlaceRange, ~ignoreLayer);
			placeHolder.transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
			if (!hit.transform)
			{
				placeHolder.transform.position = look.playerCamera.transform.position + look.playerCamera.transform.forward * maxPlaceRange;
			}
			else
			{
				placeHolder.transform.position = PlaceholderPos(hit);
			}
			Physics.Raycast(placeHolder.transform.position, -Vector3.up, out RaycastHit hitDown, maxPlaceRange, ~ignoreLayer);
			placeHolder.transform.position = PlaceholderPos(hitDown);
			yield return null;
		}
	}

	Vector3 PlaceholderPos(RaycastHit hit)
	{
		return new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z);
	}


	public void Place()
	{		
		if (!placeHolder.GetComponent<ItemPlaceholder>().obstructed)
		{
			OnPlaced?.Invoke();

			transform.position = placeHolder.transform.position;
			transform.rotation = placeHolder.transform.rotation;
			transform.parent = null;
			StopCoroutine(PlaceHolderActiveCo);
			//TEMP FIX
			authController.Svr_RemoveAuthority();
			UnBind();
			Destroy(placeHolder);

		}
	}

	public void Bind()
	{
		JODSInput.Controls.Survivor.LMB.performed += OnPlace;
	}
	public void UnBind()
	{
		JODSInput.Controls.Survivor.LMB.performed -= OnPlace;
	}

	private void OnPlace(InputAction.CallbackContext context) => Place();

	[Server]
	public void Svr_Interact(GameObject interacter)
	{
		if (!IsInteractable) return;

		// Equipment should be on a child object of the player.
		Equipment equipment = interacter.GetComponentInChildren<Equipment>();

		if (equipment != null)
		{
			authController.Svr_GiveAuthority(interacter.GetComponent<NetworkIdentity>().connectionToClient);
			equipment?.Svr_Equip(gameObject, EquipmentType);
			Equipped();
			IsInteractable = false;
		}
		else
		{
			// This should not be possible, but just to be absolutely sure.
			Debug.LogWarning($"{interacter} does not have an Equipment component", this);
		}
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
