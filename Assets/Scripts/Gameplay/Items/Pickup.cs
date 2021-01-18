using Mirror;
using UnityEngine;

public class Pickup : NetworkBehaviour, IInteractable
{
    [SerializeField]
    private string objectName = "Object name";
    [SerializeField]
    private bool isInteractable = true;

    public bool IsInteractable { get; private set; }
    public string ObjectName { get => objectName; private set => objectName = value; }

    private void OnValidate()
    {
        ObjectName = gameObject.name;
    }

    [Server]
    public void Svr_Interact(object interacter)
    {
        throw new System.NotImplementedException();
    }
}
