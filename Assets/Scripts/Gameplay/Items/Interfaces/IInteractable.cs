using Mirror;
using UnityEngine;

public interface IInteractable
{
    bool IsInteractable { get; }
    string ObjectName { get; }

    [Server]
    void Interact(GameObject interacter);
}
