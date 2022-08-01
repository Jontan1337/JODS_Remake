using Mirror;
using UnityEngine;

public interface IInteractable
{
    bool IsInteractable { get; set; }

    [Server]
    void Svr_PerformInteract(GameObject interacter);
    [Server]
    void Svr_CancelInteract(GameObject interacter);
}
