using Mirror;
using UnityEngine;

public class PhysicsToggler : NetworkBehaviour
{
    [SyncVar(hook = nameof(ToggleItemPhysics)), SerializeField] private bool hasPhysics = true;

    private Rigidbody _rigidbody;
    private Collider _collider;

    public bool HasPhysics { get => hasPhysics; }

    private void Awake()
    {
        TryGetComponent(out _rigidbody);
        TryGetComponent(out _collider);
    }

    //public override void OnStartServer()
    //{
    //    NetworkTest.RelayOnServerAddPlayer += Svr_UpdateVars;
    //}
    //public override void OnStopServer()
    //{
    //    NetworkTest.RelayOnServerAddPlayer -= Svr_UpdateVars;
    //}

    //#region Late Joiner Synchronization
    //[Server]
    //private void Svr_UpdateVars(NetworkConnection conn)
    //{
    //    if (HasPhysics)
    //    {
    //        Rpc_EnablePhysics(conn);
    //    }
    //    else
    //    {
    //        Rpc_DisablePhysics(conn);
    //    }
    //}

    //[TargetRpc]
    //private void Rpc_EnablePhysics(NetworkConnection target)
    //{
    //    _rigidbody.isKinematic = false;
    //    _collider.enabled = true;
    //}
    //[TargetRpc]
    //private void Rpc_DisablePhysics(NetworkConnection target)
    //{
    //    _rigidbody.isKinematic = false;
    //    _collider.enabled = true;
    //}
    //#endregion

    [Server]
    public void Svr_EnableItemPhysics()
    {
        hasPhysics = true;
        _rigidbody.isKinematic = false;
        _collider.enabled = true;
        Rpc_EnableItemPhysics();
    }
    [Server]
    public void Svr_DisableItemPhysics()
    {
        hasPhysics = false;
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
        Rpc_DisableItemPhysics();
    }
    [ClientRpc]
    private void Rpc_EnableItemPhysics()
    {
        _rigidbody.isKinematic = false;
        _collider.enabled = true;
    }
    [ClientRpc]
    private void Rpc_DisableItemPhysics()
    {
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
    }

    private void ToggleItemPhysics(bool oldValue, bool newValue)
    {
        if (newValue == true)
        {
            EnableItemPhysics();
        }
        else
        {
            DisableItemPhysics();
        }
    }
    private void EnableItemPhysics()
    {
        _rigidbody.isKinematic = false;
        _collider.enabled = true;
    }
    private void DisableItemPhysics()
    {
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
    }
}
