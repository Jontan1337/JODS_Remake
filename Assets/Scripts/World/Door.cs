using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Door : NetworkBehaviour, IInteractable
{
    [SerializeField] private DoorMain doorMain = null;

    public bool IsInteractable { get => true; set => throw new System.NotImplementedException(); }

    public string ObjectName => gameObject.name;

    [Server]
    public void Svr_Interact(GameObject interacter)
    {
        doorMain.Svr_Interact(interacter);
    }
}
