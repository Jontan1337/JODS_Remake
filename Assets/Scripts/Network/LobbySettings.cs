using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbySettings : NetworkBehaviour
{
    #region Singleton
    public static LobbySettings instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion


    [SyncVar] public string masterName;
    [SyncVar] public int masterIndex;
}
