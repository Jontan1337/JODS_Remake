using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RootMotion.FinalIK;

public class NetworkFullBodyIK : NetworkBehaviour
{
    private FullBodyBipedIK fullBodyBipedIK;
    [SyncVar(hook = nameof(ToggleFullBodyBipedIK))] private bool isEnabled;

    private void Awake()
    {
        fullBodyBipedIK = GetComponent<FullBodyBipedIK>();
    }

    [Server]
    private void Update()
    {
        if (!isServer) return;
        if (isEnabled != fullBodyBipedIK.enabled)
        {
            isEnabled = fullBodyBipedIK.enabled;
        }
    }

    private void ToggleFullBodyBipedIK(bool oldVal, bool newVal)
    {
        fullBodyBipedIK.enabled = newVal;
    }
}
