using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerTool : NetworkBehaviour
{
    // Big mistake just ignore.
    //[Command]
    //public void Cmd_ToRpc(string methodName, object values)
    //{
    //    Rpc_ToLocal(methodName, values);
    //}

    //[ClientRpc]
    //public void Rpc_ToLocal(string methodName, object values)
    //{
    //    StartCoroutine(methodName, values);
    //}
}
