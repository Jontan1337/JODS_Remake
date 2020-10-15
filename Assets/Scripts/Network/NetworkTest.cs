using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkTest : NetworkManager
{
    NetworkManager manager;

    public override void Start()
    {
        NetworkManager.singleton.StartHost();
        //ClientScene.AddPlayer(null);   
        NetworkServer.SpawnObjects();
        manager = GetComponent<NetworkManager>();
    }
}
