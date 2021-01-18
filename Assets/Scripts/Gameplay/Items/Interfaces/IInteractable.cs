using Mirror;

public interface IInteractable
{
    bool IsInteractable { get; }
    string ObjectName { get; }

    [Server]
    void Svr_Interact(object interacter);
}
