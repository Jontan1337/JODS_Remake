using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SyncTransformParent : NetworkBehaviour
{
    [SerializeField, SyncVar(hook=nameof(UpdateTransformParent))]
    private GameObject parent;

    private void UpdateTransformParent(GameObject oldParent, GameObject newParent)
    {
        parent = newParent;
        transform.parent = parent.transform;
    }

    private void OnTransformParentChanged()
    {
        if (isServer)
        {
            parent = transform.parent.gameObject;
        }
    }
}
