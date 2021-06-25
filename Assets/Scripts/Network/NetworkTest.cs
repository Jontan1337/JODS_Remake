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
    private GameObject canvas = null;

    public List<NetworkConnection> playerIds = new List<NetworkConnection>();
    public static Action<NetworkConnection> RelayOnServerAddPlayer;

    NetworkManager manager;

    private static NetworkTest instance;

    public static NetworkTest Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    public override void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (hostOnly)
        {
            singleton.StartHost();
            canvas.SetActive(false);
        }
        //ClientScene.AddPlayer(null);
        NetworkServer.SpawnObjects();
        manager = GetComponent<NetworkManager>();
    }

    public static List<object[]> bufferList = new List<object[]>();
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


        foreach (var item in bufferList)
        {
            if (item.Length > 2)
            {
                if (item[2].ToString() == "NetworkConnection")
                {
                    item[2] = conn;
                }
            }
            object[] tempArgs = new object[item.Length-2];
            for (int i = 0; i < tempArgs.Length; i++)
            {
                tempArgs[i] = item[i+2];
            }
            object type = item[0];
            string method = item[1].ToString();
            type.GetType().GetMethod(method, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(type, tempArgs);
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
