using Mirror;
using UnityEngine;

public interface IInteractable
{
    bool IsInteractable { get; }
    string ObjectName { get; }

    [Server]
    void Svr_Interact(GameObject interacter);
}
