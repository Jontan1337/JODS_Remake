using Mirror;
using UnityEngine;

public class PhysicsToggler : NetworkBehaviour
{
    [SyncVar(hook = nameof(ToggleItemPhysics)), SerializeField] private bool hasPhysics = true;

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _collider;

    public bool HasPhysics { get => hasPhysics; }

    private void Awake()
    {
        TryGetComponent(out _rigidbody);
    }

    public override void OnStartServer()
    {
        ToggleItemPhysics(false, hasPhysics);
    }

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
        EnableItemPhysics();
    }
    [ClientRpc]
    private void Rpc_DisableItemPhysics()
    {
        DisableItemPhysics();
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
