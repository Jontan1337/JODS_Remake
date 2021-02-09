using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;

public class NetworkTest : NetworkManager
{
    [SerializeField]
    private TMP_InputField addressInputField;

    public List<NetworkConnection> playerIds = new List<NetworkConnection>();
    public static Action<NetworkConnection> RelayOnServerAddPlayer;

    NetworkManager manager;

    public override void Start()
    {
        //ClientScene.AddPlayer(null);
        NetworkServer.SpawnObjects();
        manager = GetComponent<NetworkManager>();
    }

    // This is only called on the server.
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        print($"Player added {conn}");
        playerIds.Add(conn);
        print("Invoked RelayOnServerAddPlayer");
        if (conn.connectionId != 0)
        {
            //RelayOnServerAddPlayer?.Invoke(conn);
        }
    }

    // This is only called on the server.
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        playerIds.Remove(conn);
    }
}
