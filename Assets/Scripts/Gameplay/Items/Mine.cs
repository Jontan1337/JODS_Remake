using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Mine : NetworkBehaviour, IPlaceable
{
    [SerializeField] private bool armed = false;
    [SerializeField] private SphereCollider col;

    private void OnTriggerEnter(Collider other)
    {
        if (armed && other.gameObject.layer == 10)
        {
            if (!isServer) return;
            Svr_Explode();
        }
    }

    [Server]
    private void Svr_Explode()
    {
        GetComponent<LiveEntity>()?.DestroyEntity(transform);
    }

    public Transform Owner { get; set; }

    public void Svr_OnPlaced()
    {
        GetComponent<LiveEntity>().owner = transform.root;
        GetComponent<BlinkingLight>().StartBlinking();

        armed = true;
        col.enabled = true;
    }


}
