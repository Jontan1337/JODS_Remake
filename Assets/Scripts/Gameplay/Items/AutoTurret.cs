using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTurret : NetworkBehaviour, IEquippable, IBindable, IInteractable
{
    private PlaceItem place;
    private Coroutine placeHolderActive;

    private void Start()
    {
        place = GetComponent<PlaceItem>();
    }

    public string Name => gameObject.name;

    public GameObject Item => gameObject;

    public EquipmentType EquipmentType => EquipmentType.None;

    private bool isInteractable = true;
    public bool IsInteractable
    {
        get => isInteractable;
        set => isInteractable = value;
    }
    public string ObjectName => throw new System.NotImplementedException();

	public void Bind()
    {
        throw new System.NotImplementedException();
    }
    public void UnBind()
    {
        throw new System.NotImplementedException();
    }

    [Server]
    public void Svr_GiveAuthority(NetworkConnection conn)
    {
        netIdentity.AssignClientAuthority(conn);
    }
    [Server]
    public void Svr_RemoveAuthority()
    {
        netIdentity.RemoveClientAuthority();
    }
    [Server]
    public void Svr_Interact(GameObject interacter)
	{
        if (!IsInteractable) return;

        // Equipment should be on a child object of the player.
        Equipment equipment = interacter.GetComponentInChildren<Equipment>();

        if (equipment != null)
        {
            Svr_GiveAuthority(interacter.GetComponent<NetworkIdentity>().connectionToClient);
            equipment?.Svr_Equip(gameObject, EquipmentType);
            IsInteractable = false;
        }
        else
        {
            // This should not be possible, but just to be absolutely sure.
            Debug.LogWarning($"{interacter} does not have an Equipment component", this);
        }
    }
}
