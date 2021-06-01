using UnityEngine;
using Mirror;
using System.Collections;
using System;

public class SyncGameObjectVisuals : NetworkBehaviour
{
    [SerializeField, SyncVar(hook = nameof(UpdateParent))]
    private Transform parentData;
    [SerializeField]
    private bool syncParent = false;
    [SerializeField, SyncVar(hook = nameof(UpdatePosition))]
    private Vector3 positionData;
    [SerializeField]
    private bool syncPosition = false;
    [SerializeField, SyncVar(hook = nameof(UpdateRotation))]
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