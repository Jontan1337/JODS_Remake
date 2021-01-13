public interface IInteractable
{
    bool IsInteractable { get; }
    string ObjectName { get; }

    void Interact();
}
