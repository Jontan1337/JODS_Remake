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
        Debug.LogError("OTHER EXPLOSIONS AFFECT MINE! " +
            "IF YOU SHOOT ROCKET NEAR DEPLOYED MINE, MINE GOES FLYING.");
        if (armed && other.gameObject.layer == 10) // Why check layer? ONLY Layer 10 should even be able to interact with this, so there is no need to check.
        {   //This onTriggerEnter triggers when it is deployed, meaning it interacts with something that ISN'T layer 10. This should not happen.
            //Fix.
            //If the issue is that the mine will fall through the world if it can only interact with layer 10, then find a workaround (1 world collider 1 trigger?). Don't check layer.
            
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

    public void Svr_OnPlaced()
    {
        GetComponent<LiveEntity>().owner = transform.root;
        GetComponent<BlinkingLight>().StartBlinking();

        armed = true;
        col.enabled = true;
    }


}
