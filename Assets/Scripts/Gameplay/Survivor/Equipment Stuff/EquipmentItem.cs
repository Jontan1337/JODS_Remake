using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(PhysicsToggler), typeof(Rigidbody), typeof(BoxCollider)),
 RequireComponent(typeof(AuthorityController), typeof(SyncGameObjectVisuals), typeof(Outline)),
 RequireComponent(typeof(MeshCollider))]
public abstract class EquipmentItem : NetworkBehaviour, IInteractable, IEquippable, IBindable
{
	[Header("Basic info")]
	[SerializeField] protected string itemName = "Item name";
	[SerializeField] protected EquipmentType equipmentType = EquipmentType.None;

	[Header("Settings")]
	//[SerializeField] private bool enablePhysicsOnDrop = true;
	//[SerializeField] private bool disablePhysicsOnPickup = true;

	[Header("Other info")]
	[SerializeField] private int unequippedLayer = 14;
	[SerializeField] private int equippedLayer = 15;
	[SerializeField, SyncVar] protected bool isInteractable = true;

	[SerializeField] protected AuthorityController authController = null;
	[SerializeField] protected SyncGameObjectVisuals objectVisuals = null;
	[SerializeField] protected Transform owner;
	[SerializeField] private Rigidbody rb;
	[SerializeField] private Outline outline;

	public Action<GameObject> onServerDropItem;

	public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
	public string ObjectName => gameObject.name;
	public string Name => itemName;
	public GameObject Item => gameObject;
	public EquipmentType EquipmentType => equipmentType;

	private void Awake()
	{
		if (authController == null)
			TryGetComponent(out authController);
		if (objectVisuals == null)
			TryGetComponent(out objectVisuals);
		if (rb == null)
			TryGetComponent(out rb);
		if (outline == null)
			TryGetComponent(out outline);
	}

	[Server]
	public virtual void Svr_Interact(GameObject interacter)
	{
		if (!IsInteractable) return;
		Rpc_Interact(interacter.GetComponent<NetworkIdentity>().connectionToClient, interacter);
		// Equipment should be on a child object of the player.
		PlayerEquipment equipment = interacter.GetComponentInChildren<PlayerEquipment>();
		owner = interacter.transform;
		if (equipment != null)
		{
			equipment?.Svr_Equip(gameObject, equipmentType);
		}
		else
		{
			// This should not be possible, but just to be absolutely sure.
			Debug.LogWarning($"{interacter} does not have a PlayerEquipment component", this);
		}
	}

	[TargetRpc]
	public virtual void Rpc_Interact(NetworkConnection target, GameObject interacter)
	{

	}

	public virtual void Bind()
	{
		// This method is the last thing that happens when equipping an item.
		JODSInput.Controls.Survivor.LMB.performed += OnLMBPerformed;
		JODSInput.Controls.Survivor.LMB.canceled += OnLMBCanceled;
		JODSInput.Controls.Survivor.RMB.performed += OnRMBPerformed;
		JODSInput.Controls.Survivor.RMB.canceled += OnRMBCanceled;
        JODSInput.Controls.Survivor.Drop.performed += OnDropPerformed;
    }
	public virtual void Unbind()
	{
		// This method is the last thing that happens when dropping an item.
		JODSInput.Controls.Survivor.LMB.performed -= OnLMBPerformed;
		JODSInput.Controls.Survivor.LMB.canceled -= OnLMBCanceled;
		JODSInput.Controls.Survivor.RMB.performed -= OnRMBPerformed;
		JODSInput.Controls.Survivor.RMB.canceled -= OnRMBCanceled;
        JODSInput.Controls.Survivor.Drop.performed -= OnDropPerformed;
    }
	[TargetRpc]
	protected void Rpc_Bind(NetworkConnection target)
	{
		Bind();
	}
	[TargetRpc]
	protected void Rpc_Unbind(NetworkConnection target)
	{
		Unbind();
	}

	protected virtual void OnLMBPerformed(InputAction.CallbackContext obj)
	{

	}

	protected virtual void OnLMBCanceled(InputAction.CallbackContext obj)
	{

	}

	protected virtual void OnRMBPerformed(InputAction.CallbackContext obj)
	{

	}

	protected virtual void OnRMBCanceled(InputAction.CallbackContext obj)
	{

	}

	protected virtual void OnDropPerformed(InputAction.CallbackContext obj)
	{
		Cmd_Drop();
	}

	//[Command] Perhaps remove since unused.
	//private void Cmd_Pickup(Transform newParent, NetworkConnection conn)
	//{
	//    Svr_Pickup(newParent, conn);
	//}
	[Server]
	public virtual void Svr_Pickup(Transform newParent, NetworkConnection conn)
	{
		Debug.Log("Svr_Pickup");
		authController.Svr_GiveAuthority(conn);
		Svr_DisablePhysics();
		transform.parent = newParent;
		IsInteractable = false;
		if (outline != null)
		{
			Rpc_ToggleOutline(false);
		}
		Rpc_Pickup();
	}
	[ClientRpc]
	public virtual void Rpc_Pickup()
	{

	}

	[Command]
	public void Cmd_Drop()
	{
		Svr_Drop();
	}
	[Server]
	public virtual void Svr_Drop()
	{
		if (connectionToClient != null)
		{
			Rpc_SetLayer(connectionToClient, false);
		}
		Svr_InvokeOnDrop();
		Svr_ShowItem();
		Svr_Release();
		IsInteractable = true;
		if (outline != null)
		{
			Rpc_ToggleOutline(true);
		}
		authController.Svr_RemoveAuthority();
		Rpc_Drop();
	}
	[ClientRpc]
	public virtual void Rpc_Drop()
	{

	}

	[Server]
	public virtual void Svr_Equip()
	{
		// Apparently connecionToClient is null even
		// though authority is given long before this on line 136???
		Debug.Log("Svr_Equip");
		Rpc_SetLayer(connectionToClient, true);
		Svr_ShowItem();
		Svr_DisablePhysics();
	}
	[Server]
	public virtual void Svr_Unequip()
	{
		Rpc_SetLayer(connectionToClient, false);
		Svr_HideItem();
	}

	[Server]
	public void Svr_Release()
    {
		Svr_EnablePhysics();
		transform.parent = null;
		if (rb != null)
		{
			Svr_Throw(1.5f);
			Svr_Spin(2);
		}
	}

	#region Physics
	[Server]
	private void Svr_Throw(float force)
	{
		rb.AddForce(transform.forward * force, ForceMode.Impulse);
		Rpc_Throw(force);
	}
	[ClientRpc]
	private void Rpc_Throw(float force)
	{
		rb.AddForce(transform.forward * force, ForceMode.Impulse);
	}
	[Server]
	private void Svr_Spin(float force)
	{
		rb.AddRelativeTorque(new Vector3(3f, 2f, 1f) * force, ForceMode.Impulse);
		Rpc_Spin(force);
	}
	[ClientRpc]
	private void Rpc_Spin(float force)
	{
		rb.AddRelativeTorque(new Vector3(3f, 2f, 1f) * force, ForceMode.Impulse);
	}
	[Command]
	public void Cmd_EnablePhysics()
	{
		Svr_EnablePhysics();
	}
	[Command]
	public void Cmd_DisablePhysics()
	{
		Svr_DisablePhysics();
	}
	[Server]
	public void Svr_EnablePhysics()
	{
		if (TryGetComponent(out PhysicsToggler pt))
		{
			pt.Svr_EnableItemPhysics();
		}
	}
	[Server]
	public void Svr_DisablePhysics()
	{
		if (TryGetComponent(out PhysicsToggler pt))
		{
			pt.Svr_DisableItemPhysics();
		}
	}
	#endregion

	[Command]
	protected virtual void Cmd_InvokeOnDrop()
	{
        OnServerDropItem();
	}
	[Server]
	protected virtual void Svr_InvokeOnDrop()
    {
        OnServerDropItem();
    }
    private void OnServerDropItem()
    {
        onServerDropItem?.Invoke(gameObject);
    }

    #region Toggle Show Hide
    [Command]
	public void Cmd_ShowItem()
	{
		Svr_ShowItem();
	}
	[Command]
	public void Cmd_HideItem()
	{
		Svr_HideItem();
	}
	[Server]
	public void Svr_ShowItem()
	{
		GetComponent<Renderer>().enabled = true;
		Rpc_ShowItem();
	}
	[Server]
	public void Svr_HideItem()
	{
		GetComponent<Renderer>().enabled = false;
		Rpc_HideItem();
	}
	[ClientRpc]
	public void Rpc_ShowItem()
	{
		GetComponent<Renderer>().enabled = true;
	}
	[ClientRpc]
	public void Rpc_HideItem()
	{
		GetComponent<Renderer>().enabled = false;
	}
	#endregion

	[TargetRpc]
	private void Rpc_SetLayer(NetworkConnection target, bool isEquipped)
	{
		if (isEquipped)
		{
			gameObject.layer = equippedLayer;
		}
		else
		{
			gameObject.layer = unequippedLayer;
		}
	}

	[ClientRpc]
	private void Rpc_ToggleOutline(bool hasOutline)
	{
		outline.enabled = hasOutline;
	}
}
