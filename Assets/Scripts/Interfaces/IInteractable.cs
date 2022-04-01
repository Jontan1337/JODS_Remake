using Mirror;
using UnityEngine;

public interface IInteractable
{
    bool IsInteractable { get; set; }

    [Server]
    void Svr_Interact(GameObject interacter);
}
