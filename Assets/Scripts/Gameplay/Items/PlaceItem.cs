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
	private GameObject placeHolderPrefab;
	[SerializeField]
	private LayerMask ignoreLayer;

	private LookController look;
	private AuthorityController authController;
	private bool isInteractable = true;
	private GameObject placeHolder;
	private bool placeholderActive = true;


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
		look = GetComponentInParent<LookController>();
		StartCoroutine(PlaceHolderActive());
	}

	public IEnumerator PlaceHolderActive()
	{
		RaycastHit hit;
		placeHolder = Instantiate(placeHolderPrefab, gameObject.transform.position, transform.rotation);
		while (placeholderActive)
		{
			Physics.Raycast(look.playerCamera.transform.position, look.playerCamera.transform.forward, out hit, 5f, ignoreLayer);
			placeHolder.transform.position = hit.point;
			print(hit.point);
			print(hit.transform.name);
			yield return null;
		}
	}

	public void Place(GameObject thing)
	{
		RaycastHit hit;
		if (Physics.Raycast(look.playerCamera.transform.position, look.playerCamera.transform.forward, out hit, 5f, ignoreLayer))
		{
			//CmdPlaceItem(thing.name, placeHolder.transform.position, placeHolder.transform.rotation);
			gameObject.transform.parent = null;
			gameObject.transform.position = placeHolder.transform.position;
			gameObject.transform.rotation = placeHolder.transform.rotation;
			placeholderActive = false;
			Destroy(placeHolder);

		}
	}

	void CmdPlaceItem(string prefabName, Vector3 position, Quaternion rotation)
	{
		gameObject.transform.parent = null;
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
