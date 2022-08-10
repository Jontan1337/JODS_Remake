using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Mine : NetworkBehaviour, IPlaceable
{
    [SerializeField] private bool armed = false;
    [SerializeField] private SphereCollider col;
    [SerializeField] private LayerMask mask = 0;
    [SerializeField] private BlinkingLight indicator = null;



    private void OnTriggerEnter(Collider other)
    {
        if (armed && mask == (mask | (1 << other.gameObject.layer)))
        {   
            if (!isServer) return;
            Svr_Explode();
        }
    }

    [Server]
    private void Svr_Explode()
    {
        GetComponent<LiveEntity>()?.Svr_DestroyEntity(transform);
    }

    public Transform Owner { get; set; }

    [Server]
    public void Svr_OnPlaced()
    {
        GetComponent<LiveEntity>().owner = transform.root;
        Rpc_StartBlinking();

        armed = true;
        col.enabled = true;
    }

    [ClientRpc]
    private void Rpc_StartBlinking()
    {
        indicator.StartBlinking();
    }


}
