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

    private void Initialize()
    {
        if (syncParent) parentData = transform.parent;
        if (syncPosition) positionData = transform.position;
        if (syncRotation) rotationData = transform.rotation;
    }

    private void UpdateParent(Transform oldParentData, Transform newParentData)
    {
        transform.parent = newParentData;
    }
    private void UpdatePosition(Vector3 oldPositionData, Vector3 newPositionData)
    {
        if (name.Contains("PlayerHands"))
        {
            print(oldPositionData);
            print(newPositionData);
        }
        transform.position = newPositionData;
    }
    private void UpdateRotation(Quaternion oldRotationData, Quaternion newRotationData)
    {
        transform.rotation = newRotationData;
    }
}