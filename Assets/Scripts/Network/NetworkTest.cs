using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;

public class NetworkTest : NetworkManager
{
    [SerializeField]
    private bool hostOnly = false;
    [SerializeField]
    private GameObject canvas;

    public List<NetworkConnection> playerIds = new List<NetworkConnection>();
    public static Action<NetworkConnection> RelayOnServerAddPlayer;

    NetworkManager manager;

    public override void Start()
    {
        if (hostOnly)
        {
            singleton.StartHost();
            canvas.SetActive(false);
        }
        //ClientScene.AddPlayer(null);
        NetworkServer.SpawnObjects();
        manager = GetComponent<NetworkManager>();
    }

    // This is only called on the server.
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        playerIds.Add(conn);
        StartCoroutine(DispatchNewConnection(conn));
    }

    private IEnumerator DispatchNewConnection(NetworkConnection conn)
    {
        // Wait for a little bit, so networked objects are ready.
        yield return new WaitForSeconds(0.15f);
        // If the host is the new player, don't synchronize anything.
        if (conn.connectionId != 0)
        {
            RelayOnServerAddPlayer?.Invoke(conn);
        }
    }

    // This is only called on the server.
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        playerIds.Remove(conn);
    }
}
