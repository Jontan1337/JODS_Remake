using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror.Discovery;
using Mirror.Cloud;

public class MatchListing : NetworkDiscovery
{
    [Header("References")]
    public Button CreateMatch;
    public Button RefreshList;

    public ApiConnector connector;
    public Transform ListOfMatches = null;
    public GameObject SingleServerListItem = null;
    [SerializeField] private List<GameObject> discoveredServers = new List<GameObject>();


    private void Awake()
    {
        RefreshList.onClick.AddListener(delegate() { StartNetDiscovery(); } );
        OnServerFound.AddListener(delegate(ServerResponse response) { OnDiscoveredServer(response); });
        //UnityEditor.Events.UnityEventTools.AddPersistentListener(OnServerFound, OnDiscoveredServer);
    }

    public void StartNetDiscovery()
    {
        for (int i = 0; i < discoveredServers.Count; i++)
        {
            Destroy(discoveredServers[i].gameObject);
        }
        discoveredServers.Clear();
        StartDiscovery();
        //connector.ListServer.ClientApi.GetServerList();
    }

    public void OnDiscoveredServer(ServerResponse response)
    {
        bool discovered = false;

        for (int i = 0; i < discoveredServers.Count; i++)
        {
            ServerListItem tempSvrListItem = discoveredServers[i].GetComponent<ServerListItem>();
            // If the server is already on the discovered server list, then break out
            // of loop and check next index in serverCollection.
            if (response.EndPoint.Address.ToString() == tempSvrListItem.Address)
            {
                // Set true since we move on to the next server in serverCollection.
                discovered = true;
                break;
            }
        }
        if (!discovered)
        {
            SetupSingleListItem(response);
            discovered = false;
        }
    }

    private void SetupSingleListItem(ServerResponse response)
    {
        GameObject match = Instantiate(SingleServerListItem);
        match.transform.SetParent(ListOfMatches, false);
        match.transform.localScale = Vector3.one;
        ServerListItem newServerListItem = match.GetComponent<ServerListItem>();
        string tempName = $"{response.EndPoint.Address}";
        newServerListItem.Setup(tempName, 0, response.EndPoint.Address.ToString());
        //match.transform.Find("match name").GetComponent<Text>().text = $"{result.address} | Players : {result.playerCount} / {result.maxPlayerCount}" /*m.currentSize*/;
        newServerListItem.JoinButton.onClick.AddListener(delegate () { Lobby.Instance.MMJoinMatch(response.uri); });
        discoveredServers.Add(match);
    }
}
