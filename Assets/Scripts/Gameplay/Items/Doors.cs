using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Doors : NetworkBehaviour
{
    public bool toggleOpen;
    [SyncVar] public bool locked;
    private Animator ani;

    void Start() { ani = GetComponent<Animator>(); }

    void Update() { ani.SetBool("Toggle", (toggleOpen)); }

    [ClientRpc]
    public void RpcToggleDoor()
    {
        if (!locked)
        {
            toggleOpen = !toggleOpen;
        }
    }
    [ClientRpc]
    public void RpcLockDoor(bool open)
    {
        if (!locked)
        {
            if (open && !toggleOpen)
            {
                toggleOpen = !toggleOpen;
            }
            else if (!open && toggleOpen)
            {
                toggleOpen = !toggleOpen;
            }
            locked = true;
            Invoke("RpcUnlockDoor", 10f);
        }
    }
    [ClientRpc]
    public void RpcUnlockDoor()
    {
        locked = false;
    }
}
