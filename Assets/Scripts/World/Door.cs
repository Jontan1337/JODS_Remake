using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Door : NetworkBehaviour, IInteractable
{
    [SerializeField] private DoorMain doorMain = null;

    public bool IsInteractable { get => true; set => throw new System.NotImplementedException(); }

    [Server]
    public void Svr_PerformInteract(GameObject interacter)
    {
        doorMain.Svr_PerformInteract(interacter);
    }
    [Server]
    public void Svr_CancelInteract(GameObject interacter)
    {
    }
}
