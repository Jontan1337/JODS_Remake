using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Door : NetworkBehaviour, IInteractable
{
    [SerializeField] private DoorMain doorMain = null;

    public bool IsInteractable { get => true; set => throw new System.NotImplementedException(); }

    [Server]
    public void Svr_Interact(GameObject interacter)
    {
        doorMain.Svr_Interact(interacter);
    }
}
