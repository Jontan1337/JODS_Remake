using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
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

    public void Interact()
    {
    }
}
