using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;

public class NetworkTest : NetworkManager
{
    [Space]
    [SerializeField] private GameObject masterPrefab = null;
    [SerializeField] private GameObject survivorPrefab = null;
    [Space]
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

        canvas.SetActive(true);
        if (hostOnly)
        {
            singleton.StartHost();
            canvas.SetActive(false);
        }
        //ClientScene.AddPlayer(null);
        NetworkServer.SpawnObjects();
        manager = GetComponent<NetworkManager>();
    }

    public static List<NetworkBufferItem> networkBufferList = new List<NetworkBufferItem>();
    public static void AddBuffer(MonoBehaviour type, string methodName, object[] args)
    {
        NetworkBufferItem bufferItem = new NetworkBufferItem(type, methodName, args);

        networkBufferList.Add(bufferItem);
    }
    public static void RemoveBuffer(MonoBehaviour type, string methodName, object[] args)
    {
        NetworkBufferItem bufferItem = new NetworkBufferItem(type, methodName, args);

        networkBufferList.Remove(bufferItem);
    }

    // This is only called on the server.
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        bool master = playerPrefab == masterPrefab;
        GamemodeBase.Instance.Svr_AddPlayer(conn.identity.netId, "Player" + GamemodeBase.Instance.PlayerCount + 1, master);
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

        foreach (var item in networkBufferList)
        {
            object type = item.objectType;
            string method = item.objectMethod;
            object[] args = item.args;
            object[] tempArgs = new object[item.args.Length];
            if (args.Length > 1)
            {
                if (args[0].ToString() == "NetworkConnection")
                {
                    // Boxing to replace string "NetworkConnection" with the actual connection.
                    tempArgs[0] = conn;
                }
            }
            // Create new array that only holds the args to pass to the method parameters.
            for (int i = 1; i < tempArgs.Length; i++)
            {
                tempArgs[i] = args[i];
            }
            type.GetType()
                .GetMethod(method, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(type, tempArgs);
        }
    }

    // This is only called on the server.
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        conn.identity.gameObject.GetComponentInChildren<PlayerEquipment>().Svr_DropAllItems();
        playerIds.Remove(conn);
        base.OnServerDisconnect(conn);
    }

    private void OnGUI()
    {
        GUI.TextField(new Rect(20, 100, 150, 20), "Network Test ON");
    }
}
