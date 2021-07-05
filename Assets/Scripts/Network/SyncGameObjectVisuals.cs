using UnityEngine;
using Mirror;
using System.Collections;
using System;

public class SyncGameObjectVisuals : NetworkBehaviour
{
    [SerializeField, SyncVar(hook = nameof(UpdateParent))] private Transform parentData;
    [SerializeField, SyncVar(hook = nameof(UpdatePosition))] private Vector3 positionData;
    [SerializeField, SyncVar(hook = nameof(UpdateLocalPosition))] private Vector3 localPositionData;
    [SerializeField, SyncVar(hook = nameof(UpdateRotation))] private Quaternion rotationData;
    [SerializeField] private bool syncParent = false;
    [SerializeField] private bool syncPosition = false;
    [SerializeField] private bool syncRotation = false;


    private void OnTransformParentChanged()
    {
        if (isServer)
        {
            Svr_Initialize();
        }
    }

    public override void OnStartServer()
    {
        if (NetworkTest.Instance != null)
        {
            NetworkTest.RelayOnServerAddPlayer += Svr_UpdateVars;
        }
        else
        {
            Lobby.RelayOnServerSynchronize += Svr_UpdateVars;
        }
    }
    public override void OnStopServer()
    {
        if (NetworkTest.Instance != null)
        {
            NetworkTest.RelayOnServerAddPlayer -= Svr_UpdateVars;
        }
        else
        {
            Lobby.RelayOnServerSynchronize -= Svr_UpdateVars;
        }
    }

    [Server]
    private void Svr_UpdateVars(NetworkConnection conn)
    {
        if (syncParent)
        {
            if (transform.parent != null)
            {
                parentData = transform.parent;
            }
            Rpc_UpdateParent(conn, transform.parent);
        }
        if (syncPosition)
        {
            Rpc_UpdatePosition(conn, transform.position);
            Rpc_UpdateLocalPosition(conn, transform.localPosition);
        }
        if (syncRotation)
            Rpc_UpdateRotation(conn, transform.rotation);
    }

    [Server]
    private void Svr_Initialize()
    {
        if (syncParent) parentData = transform.parent;
        if (syncPosition)
        {
            positionData = transform.position;
            localPositionData = transform.localPosition;
        }
        if (syncRotation) rotationData = transform.rotation;
    }

    #region Serialization
    //public override bool OnSerialize(NetworkWriter writer, bool initialState)
    //{
    //    if (!initialState)
    //    {
    //        writer.WriteTransform(parentData);
    //        writer.WriteVector3(positionData);
    //        writer.WriteQuaternion(rotationData);
    //    }
    //    else
    //    {
    //        writer.WriteTransform(parentData);
    //        writer.WriteBoolean(syncParent);
    //        writer.WriteVector3(positionData);
    //        writer.WriteBoolean(syncPosition);
    //        writer.WriteQuaternion(rotationData);
    //        writer.WriteBoolean(syncRotation);
    //    }
    //    return true;
    //}
    //public override void OnDeserialize(NetworkReader reader, bool initialState)
    //{
    //    if (!initialState)
    //    {
    //        parentData = reader.ReadTransform();
    //        //transform.parent = parentData;
    //        positionData = reader.ReadVector3();
    //        //transform.position = positionData;
    //        rotationData = reader.ReadQuaternion();
    //        //transform.rotation = rotationData;
    //    }
    //    else
    //    {
    //        parentData = reader.ReadTransform();
    //        //transform.parent = parentData;
    //        syncParent = reader.ReadBoolean();

    //        positionData = reader.ReadVector3();
    //        //transform.position = positionData;
    //        syncPosition = reader.ReadBoolean();

    //        rotationData = reader.ReadQuaternion();
    //        //transform.rotation = rotationData;
    //        syncRotation = reader.ReadBoolean();
    //    }
    //}
    #endregion

    [TargetRpc]
    private void Rpc_UpdateParent(NetworkConnection target, Transform newParentData)
    {
        parentData = newParentData;
        transform.parent = newParentData;
    }
    [TargetRpc]
    private void Rpc_UpdatePosition(NetworkConnection target, Vector3 newPositionData)
    {
        transform.position = newPositionData;
    }
    [TargetRpc]
    private void Rpc_UpdateLocalPosition(NetworkConnection target, Vector3 newLocalPositionData)
    {
        transform.localPosition = newLocalPositionData;
    }
    [TargetRpc]
    private void Rpc_UpdateRotation(NetworkConnection target, Quaternion newRotationData)
    {
        transform.rotation = newRotationData;
    }

    private void UpdateParent(Transform oldParentData, Transform newParentData)
    {
        if (oldParentData == newParentData) return;

        transform.parent = newParentData;
    }
    private void UpdatePosition(Vector3 oldPositionData, Vector3 newPositionData)
    {
        transform.position = newPositionData;
    }
    private void UpdateLocalPosition(Vector3 oldLocalPositionData, Vector3 newLocalPositionData)
    {
        transform.localPosition = newLocalPositionData;
    }
    private void UpdateRotation(Quaternion oldRotationData, Quaternion newRotationData)
    {
        transform.rotation = newRotationData;
    }
}