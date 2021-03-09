using UnityEngine;
using Mirror;

public class SyncGameObjectVisuals : NetworkBehaviour
{
    [SerializeField, SyncVar]
    private Transform parent;
    [SerializeField, SyncVar]
    private Vector3 positionData;
    [SerializeField, SyncVar]
    private Quaternion rotationData;

    private void OnTransformParentChanged()
    {
        if (isServer)
        {
            parent = transform.parent;
            positionData = transform.position;
            rotationData = transform.rotation;
        }
    }

    #region NetworkBehaviour Callbacks
    public override void OnStartServer()
    {
        if (isServer)
        {
            NetworkTest.RelayOnServerAddPlayer += Svr_UpdateVars;
        }
    }

    public override void OnStopServer()
    {
        if (isServer)
        {
            NetworkTest.RelayOnServerAddPlayer -= Svr_UpdateVars;
        }
    }
    #endregion

    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if (!initialState)
        {
            writer.WriteTransform(transform.parent);
            writer.WriteVector3(transform.position);
            writer.WriteQuaternion(transform.rotation);
            return true;
        }
        return false;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (!initialState)
        {
            parent = reader.ReadTransform();
            transform.parent = parent;
            positionData = reader.ReadVector3();
            transform.position = positionData;
            rotationData = reader.ReadQuaternion();
            transform.rotation = rotationData;
        }
    }

    [Server]
    private void Svr_UpdateVars(NetworkConnection conn)
    {
        Rpc_UpdateParent(conn, parent);
        Rpc_UpdateTransform(conn, positionData, rotationData);
    }

    [TargetRpc]
    private void Rpc_UpdateParent(NetworkConnection target, Transform newParent)
    {
        parent = newParent;
        transform.parent = newParent;
    }
    [TargetRpc]
    private void Rpc_UpdateTransform(NetworkConnection target, Vector3 positionData, Quaternion rotationData)
    {
        transform.position = positionData;
        transform.rotation = rotationData;
    }
}