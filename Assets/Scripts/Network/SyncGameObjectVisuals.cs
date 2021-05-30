using UnityEngine;
using Mirror;
using System.Collections;
using System;

public class SyncGameObjectVisuals : NetworkBehaviour
{
    [SerializeField, SyncVar]
    private Transform parentData;
    [SerializeField]
    private bool syncParent = false;
    [SerializeField, SyncVar]
    private Vector3 positionData;
    [SerializeField]
    private bool syncPosition = false;
    [SerializeField, SyncVar]
    private Quaternion rotationData;
    [SerializeField]
    private bool syncRotation = false;

    private void OnTransformParentChanged()
    {
        if (isServer)
        {
            Initialize();
        }
    }

    private void Initialize()
    {
        if (syncParent) parentData = transform.parent;
        if (syncPosition) positionData = transform.position;
        if (syncRotation) rotationData = transform.rotation;
    }

    #region NetworkBehaviour Callbacks
    public override void OnStartServer()
    {
        //NetworkTest.RelayOnServerAddPlayer += Svr_UpdateVars;
    }

    public override void OnStopServer()
    {
        //NetworkTest.RelayOnServerAddPlayer -= Svr_UpdateVars;
    }
    #endregion

    #region Serialization

    //public override bool OnSerialize(NetworkWriter writer, bool initialState)
    //{
    //    try
    //    {
    //        if (!initialState)
    //        {
    //            writer.WriteTransform(transform.parent);
    //            writer.WriteVector3(transform.position);
    //            writer.WriteQuaternion(transform.rotation);
    //        }
    //        else
    //        {
    //            writer.WriteTransform(parentData);
    //            writer.WriteBoolean(syncParent);
    //            writer.WriteVector3(positionData);
    //            writer.WriteBoolean(syncPosition);
    //            writer.WriteQuaternion(rotationData);
    //            writer.WriteBoolean(syncRotation);
    //        }
    //        return true;
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError($"Error: {e.Message}", this);
    //        return false;
    //    }
    //}

    //public override void OnDeserialize(NetworkReader reader, bool initialState)
    //{
    //    if (!initialState)
    //    {
    //        parentData = reader.ReadTransform();
    //        transform.parent = parentData;
    //        positionData = reader.ReadVector3();
    //        transform.position = positionData;
    //        rotationData = reader.ReadQuaternion();
    //        transform.rotation = rotationData;
    //    }
    //    try
    //    {
    //        if (!initialState)
    //        {
    //            parentData = reader.ReadTransform();
    //            transform.parent = parentData;
    //            positionData = reader.ReadVector3();
    //            transform.position = positionData;
    //            rotationData = reader.ReadQuaternion();
    //            transform.rotation = rotationData;
    //        }
    //        else
    //        {
    //            parentData = reader.ReadTransform();
    //            transform.parent = parentData;
    //            syncParent = reader.ReadBoolean();

    //            positionData = reader.ReadVector3();
    //            transform.position = positionData;
    //            syncPosition = reader.ReadBoolean();

    //            rotationData = reader.ReadQuaternion();
    //            transform.rotation = rotationData;
    //            syncRotation = reader.ReadBoolean();
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError($"Error: {e.Message}", this);
    //    }
    //}

    #endregion

    [Server]
    private void Svr_UpdateVars(NetworkConnection conn)
    {
        Initialize();
        Rpc_UpdateParent(conn, parentData);
        Rpc_UpdateTransform(conn, positionData, rotationData);
    }

    [TargetRpc]
    private void Rpc_UpdateParent(NetworkConnection target, Transform newParent)
    {
        parentData = newParent;
        transform.parent = newParent;
    }
    [TargetRpc]
    private void Rpc_UpdateTransform(NetworkConnection target, Vector3 positionData, Quaternion rotationData)
    {
        transform.position = positionData;
        transform.rotation = rotationData;
    }
}