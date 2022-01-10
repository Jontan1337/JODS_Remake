using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitSync : NetworkBehaviour
{
    [SerializeField] private float unitSyncInterval = 0.02f;
    void Start()
    {
        if (isServer)
        {
            StartCoroutine(IESync());
        }
    }

    private IEnumerator IESync()
    {
        while (true)
        {
            yield return new WaitForSeconds(unitSyncInterval);

            Rpc_SyncPos(transform.position, transform.rotation);
        }
    }

    [ClientRpc]
    private void Rpc_SyncPos(Vector3 currentPosition, Quaternion currentRotation)
    {
        transform.position = currentPosition;
        transform.rotation = currentRotation;
    }
}
