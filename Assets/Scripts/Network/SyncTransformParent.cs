﻿using UnityEngine;
using Mirror;

public class SyncTransformParent : NetworkBehaviour
{
    [SerializeField, SyncVar]
    private Transform parent;

    private void OnTransformParentChanged()
    {
        if (isServer)
        {
            parent = transform.parent;
        }
    }

    public override void OnStartServer()
    {
        NetworkTest.RelayOnServerAddPlayer += e => Rpc_UpdateParent(e, parent);
    }

    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if (!initialState)
        {
            writer.WriteTransform(transform.parent);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (!initialState)
        {
            parent = reader.ReadTransform();
            transform.parent = parent;
        }
    }

    [TargetRpc]
    private void Rpc_UpdateParent(NetworkConnection target, Transform newParent)
    {
        parent = newParent;
        transform.parent = newParent;
    }
}