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
	private void Equipped()
	{
		print(placeHolder);
		placeHolder.SetActive(true);
		look = GetComponentInParent<LookController>();
		StartCoroutine(PlaceHolderActive());
	}

	public IEnumerator PlaceHolderActive()
	{
		RaycastHit hit;
		while (placeholderActive)
		{
			Physics.Raycast(look.playerCamera.transform.position, look.playerCamera.transform.forward, out hit, 5f, ~ignoreLayer);


			placeHolder.transform.eulerAngles = new Vector3(0, transform.rotation.y, transform.rotation.z);
			placeHolder.transform.position = new Vector3(hit.point.x, 0.01f, hit.point.z);
			yield return null;
		}
	}

	public void Place(GameObject thing)
	{
		RaycastHit hit;
		if (Physics.Raycast(look.playerCamera.transform.position, look.playerCamera.transform.forward, out hit, 5f, ~ignoreLayer))
		{
			print(hit.transform.name);
			transform.position = placeHolder.transform.position;
			transform.rotation = placeHolder.transform.rotation;
			transform.parent = null;
			placeholderActive = false;
			Destroy(placeHolder);

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

	public void Bind()
	{
		JODSInput.Controls.Survivor.LMB.performed += OnPlace;
	}
	public void UnBind()
	{
		JODSInput.Controls.Survivor.LMB.performed -= OnPlace;
	}

	private void OnPlace(InputAction.CallbackContext context) => Place(gameObject);

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
}
