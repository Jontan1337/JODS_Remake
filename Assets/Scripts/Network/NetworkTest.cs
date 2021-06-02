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
        yield return new WaitForSeconds(0.2f);
        // If the host is the new player, don't synchronize anything.
        if (conn.connectionId != 0)
        {
            RelayOnServerAddPlayer?.Invoke(conn);
        }
    }

    public GameObject dummyPlayer;
    // This is only called on the server.
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        HashSet<NetworkIdentity> copyOfClientOwnedObject = conn.clientOwnedObjects;

        //GameObject dummy = Instantiate(dummyPlayer);
        //NetworkServer.ReplacePlayerForConnection(conn, dummy);

        conn.identity.gameObject.GetComponentInChildren<PlayerEquipment>().Svr_DropAllItems();

        //foreach (NetworkIdentity identity in copyOfClientOwnedObject)
        //{
        //    if (identity != conn.identity)
        //    {
        //        //identity.RemoveClientAuthority();
        //        if (identity.TryGetComponent(out PlayerEquipment equipment))
        //        {
        //            equipment.Svr_DropAllItems();
        //        }
        //    }
        //}
        playerIds.Remove(conn);
        base.OnServerDisconnect(conn);
    }
}
