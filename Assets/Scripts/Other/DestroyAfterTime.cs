using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DestroyAfterTime : Timer
{
    protected override void Finish()
    {
        NetworkServer.Destroy(gameObject);
    }

    protected override void Tick()
    {

    }
}
