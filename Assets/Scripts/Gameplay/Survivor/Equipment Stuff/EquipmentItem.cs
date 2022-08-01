using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Events;

[RequireComponent(typeof(PhysicsToggler), typeof(Rigidbody), typeof(BoxCollider)),
 RequireComponent(typeof(AuthorityController), typeof(SyncGameObjectVisuals), typeof(Outline)),
 RequireComponent(typeof(MeshCollider))]
public abstract class EquipmentItem : NetworkBehaviour, IInteractable, IEquippable, IBindable
{
	[Title("EQUIPMENT ITEM", "", TitleAlignments.Centered)]
	[Header("Basic info")]
	[SerializeField] protected string itemName = "Item name";
	[SerializeField] protected EquipmentType equipmentType = EquipmentType.None;

	[Header("Other info")]
	[SerializeField] private int unequippedLayer = 14;
	[SerializeField] private int equippedLayer = 15;
	[SerializeField] private MeshRenderer[] objectMeshes;
	[SerializeField, SyncVar] protected bool isInteractable = true;

	[Header("References")]
	public Sprite UISilhouette = null;
	[SerializeField] protected AuthorityController authController = null;
	[SerializeField] protected SyncGameObjectVisuals objectVisuals = null;
	[SerializeField] protected Transform owner = null;
	[SerializeField] private Rigidbody rb = null;
	[SerializeField] private Outline outline = null;

	public Action<GameObject> onServerDropItem;
    private ActiveSClass playerClass = null;

    public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
	public string Name => itemName;
	public GameObject Item => gameObject;
	public EquipmentType EquipmentType => equipmentType;

    protected virtual void Awake()
	{
		// GetComponentsInChildren also includes the parent apparently.
		objectMeshes = transform.GetComponentsInChildren<MeshRenderer>();

		if (authController == null)
			TryGetComponent(out authController);
		if (objectVisuals == null)
			TryGetComponent(out objectVisuals);
		if (rb == null)
			TryGetComponent(out rb);
		if (outline == null)
			TryGetComponent(out outline);
	}

    private void OnTransformParentChanged()
    {
		if (transform.parent == null) return;

        if (isServer)
        {
			Svr_ParentChanged();
        }
		if (!isServer && hasAuthority)
        {
			ParentChanged();
        }
    }

    [Server]
	public virtual void Svr_PerformInteract(GameObject interacter)
	{
		if (!IsInteractable) return;
		Rpc_PerformInteract(interacter.GetComponent<NetworkIdentity>().connectionToClient, interacter);
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
    [Server]
	public virtual void Svr_CancelInteract(GameObject interacter)
	{
		if (!IsInteractable) return;
		Rpc_CancelInteract(interacter.GetComponent<NetworkIdentity>().connectionToClient, interacter);
	}

	[TargetRpc]
	public virtual void Rpc_PerformInteract(NetworkConnection target, GameObject interacter)
	{
		playerClass = interacter.GetComponent<ActiveSClass>();
	}
	[TargetRpc]
	public virtual void Rpc_CancelInteract(NetworkConnection target, GameObject interacter)
	{
		playerClass = null;
	}

	[Server]
	public virtual void Svr_ParentChanged()
	{
	}
	[Client]
	public virtual void ParentChanged()
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
        playerClass.onDied.AddListener(delegate () { Unbind(); });
    }
	public virtual void Unbind()
	{
		// This method is the last thing that happens when dropping an item.
		JODSInput.Controls.Survivor.LMB.performed -= OnLMBPerformed;
		JODSInput.Controls.Survivor.LMB.canceled -= OnLMBCanceled;
		JODSInput.Controls.Survivor.RMB.performed -= OnRMBPerformed;
		JODSInput.Controls.Survivor.RMB.canceled -= OnRMBCanceled;
        JODSInput.Controls.Survivor.Drop.performed -= OnDropPerformed;
		if (playerClass)
        {
			playerClass.onDied.RemoveListener(delegate () { Unbind(); });
        }
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
		authController.Svr_GiveAuthority(conn);
		Svr_DisablePhysics();
		transform.parent = newParent;
		IsInteractable = false;
		if (outline != null)
		{
			Rpc_ToggleOutline(false);
		}
		Rpc_Pickup(connectionToClient);
	}
	[TargetRpc]
	public virtual void Rpc_Pickup(NetworkConnection target)
	{
		onPickupEvents.Invoke();
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
		Svr_ShowItem(true);
		Svr_Release();
		IsInteractable = true;
		if (outline != null)
		{
			Rpc_ToggleOutline(true);
		}
		Rpc_Drop(connectionToClient);
		authController.Svr_RemoveAuthority();
	}
	[TargetRpc]
	public virtual void Rpc_Drop(NetworkConnection target)
	{
		onDropEvents.Invoke();
	}

	[Server]
	public virtual void Svr_Equip()
	{
		Rpc_SetLayer(connectionToClient, true);
		Svr_ShowItem(true);
		Svr_DisablePhysics();
	}
	[Server]
	public virtual void Svr_Unequip()
	{
		Rpc_SetLayer(connectionToClient, false);
		Svr_ShowItem(false);
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
	public void Cmd_ShowItem(bool show)
	{
		Svr_ShowItem(show);
	}
	[Server]
	public void Svr_ShowItem(bool show)
	{
		ShowItem(show);
		Rpc_ShowItem(show);
	}
	[ClientRpc]
	public void Rpc_ShowItem(bool show)
	{
		ShowItem(show);
	}
	private void ShowItem(bool show)
    {
		foreach (MeshRenderer item in objectMeshes)
		{
			item.enabled = show;
		}
	}
	#endregion

	[TargetRpc]
	private void Rpc_SetLayer(NetworkConnection target, bool isEquipped)
	{
        if (isEquipped)
        {
            foreach (MeshRenderer item in objectMeshes)
            {
                item.gameObject.layer = equippedLayer;
            }
        }
        else
        {
            foreach (MeshRenderer item in objectMeshes)
            {
                item.gameObject.layer = unequippedLayer;
            }
        }
    }

	[ClientRpc]
	private void Rpc_ToggleOutline(bool hasOutline)
	{
		outline.enabled = hasOutline;
	}

	[Header("On Pickup & Drop (LOCAL)")]
	[SerializeField] private UnityEvent onPickupEvents;
	[SerializeField] private UnityEvent onDropEvents;

}
