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

    public override void OnStartServer()
    {
        NetworkTest.RelayOnServerAddPlayer += Svr_UpdateVars;
    }
    public override void OnStopServer()
    {
        NetworkTest.RelayOnServerAddPlayer -= Svr_UpdateVars;
    }

    [Server]
    private void Svr_UpdateVars(NetworkConnection conn)
    {
        Rpc_UpdateParent(conn, transform.parent);
        Rpc_UpdatePosition(conn, transform.position);
        Rpc_UpdateRotation(conn, transform.rotation);
    }

    private void Initialize()
    {
        if (syncParent) parentData = transform.parent;
        if (syncPosition) positionData = transform.position;
        if (syncRotation) rotationData = transform.rotation;
    }

    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if (!initialState)
        {
            writer.WriteTransform(parentData);
            writer.WriteVector3(positionData);
            writer.WriteQuaternion(rotationData);
        }
        else
        {
            writer.WriteTransform(parentData);
            writer.WriteBoolean(syncParent);
            writer.WriteVector3(positionData);
            writer.WriteBoolean(syncPosition);
            writer.WriteQuaternion(rotationData);
            writer.WriteBoolean(syncRotation);
        }
        return true;
    }
    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (!initialState)
        {
            parentData = reader.ReadTransform();
            transform.parent = parentData;
            positionData = reader.ReadVector3();
            transform.position = positionData;
            rotationData = reader.ReadQuaternion();
            transform.rotation = rotationData;
        }
        else
        {
            parentData = reader.ReadTransform();
            transform.parent = parentData;
            syncParent = reader.ReadBoolean();

            positionData = reader.ReadVector3();
            transform.position = positionData;
            syncPosition = reader.ReadBoolean();

            rotationData = reader.ReadQuaternion();
            transform.rotation = rotationData;
            syncRotation = reader.ReadBoolean();
        }
    }

    [TargetRpc]
    private void Rpc_UpdateParent(NetworkConnection target, Transform newParentData)
    {
        transform.parent = newParentData;
    }
    [TargetRpc]
    private void Rpc_UpdatePosition(NetworkConnection target, Vector3 newPositionData)
    {
        if (name.Contains("PlayerHands"))
        {
            Debug.Log(newPositionData, this);
        }
        transform.position = newPositionData;
        if (name.Contains("PlayerHands"))
        {
            Debug.Log(transform.localPosition, this);
        }
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
        //if (name.Contains("PlayerHands"))
        //{
        //    Debug.Log(oldPositionData, this);
        //    Debug.Log(newPositionData, this);
        //}
        transform.position = newPositionData;
        //if (name.Contains("PlayerHands"))
        //{
        //    Debug.Log(transform.localPosition, this);
        //}
    }
    private void UpdateRotation(Quaternion oldRotationData, Quaternion newRotationData)
    {
        transform.rotation = newRotationData;
    }
}