using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AuthorityController : NetworkBehaviour
{
    [Server]
    public void Svr_GiveAuthority(NetworkConnection conn)
    {
        netIdentity.AssignClientAuthority(conn);
    }
    [Server]
    public void Svr_RemoveAuthority()
    {
        netIdentity.RemoveClientAuthority();
    }
}
