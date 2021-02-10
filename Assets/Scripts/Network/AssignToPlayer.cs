using UnityEngine;
using Mirror;

public class AssignToPlayer : NetworkBehaviour
{
    private void Start()
    {
        Debug.Log("START", this);
        if (!isServer) return;

        print("OnStartServer");
        netIdentity.AssignClientAuthority(GetComponentInParent<NetworkIdentity>().connectionToClient);
    }

    public override void OnStartAuthority()
    {
        Debug.Log($"Assigned authority to {GetComponentInParent<NetworkIdentity>().connectionToClient}");
    }
}
