using System.Collections;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

public struct PlayerToSpawn
{
    public NetworkConnection conn;
    public string _class;
    public bool _isMaster;
}
public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject masterPrefab = null;
    [SerializeField] private GameObject survivorPrefab = null;

    [SerializeField] private List<SurvivorSO> survivorSOList = new List<SurvivorSO>();
    [SerializeField] private List<UnitMasterSO> masterSOList = new List<UnitMasterSO>();

    private List<PlayerToSpawn> playersToSpawns = new List<PlayerToSpawn>();

    public MapSettingsSO mapSettings;
    private List<Vector3> spawnPoints;

    public override void OnStartServer()
    {
        //Get both lists of playable characters, which are later used to spawn each player with their chosen class
        survivorSOList = PlayableCharactersManager.Instance.GetAllSurvivors();
        masterSOList = PlayableCharactersManager.Instance.GetAllMasters();
        spawnPoints = new List<Vector3>(mapSettings.spawnPoints);

        //When a player connects to the new scene, then invoke the ReadyPlayer method
        Lobby.OnServerReadied += Svr_ReadyPlayer; //This method puts the player's info into a list, which will be used when spawning the player.

        Lobby.OnPlayersLoaded += Svr_SpawnAllPlayers;
    }

    [ServerCallback]
    public void OnDestroy()
    {
        Lobby.OnServerReadied -= Svr_ReadyPlayer;
        Lobby.OnPlayersLoaded -= Svr_SpawnAllPlayers;
    }

    [Server]
    private void Svr_ReadyPlayer(NetworkConnection conn, string _class, bool _isMaster)
    {
        playersToSpawns.Add(new PlayerToSpawn { conn = conn, _class = _class, _isMaster = _isMaster });
    }

    [Server]
    public void Svr_SpawnAllPlayers()
    {
        //Iterate through all players
        foreach(var player in playersToSpawns)
        {
            //Tell the server to spawn the player
            Svr_SpawnPlayer(player.conn, player._class, player._isMaster);
        }
    }


    [Server]
    public void Svr_SpawnPlayer(NetworkConnection conn, string _class, bool _isMaster)
    {
        //Reference to the old player instance
        GameObject oldPlayerInstance = conn.identity.gameObject;

        //Get a spawnpoint from the list of spawnpoints
        int newSpawnpointIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
        Vector3 spawnpoint = spawnPoints[newSpawnpointIndex];
        spawnPoints.RemoveAt(newSpawnpointIndex);

        //Spawn a new object for the player and set a reference to the new player instance.
        GameObject newPlayerInstance = Instantiate(_isMaster ? masterPrefab : survivorPrefab, spawnpoint, Quaternion.identity);

        if (NetworkServer.ReplacePlayerForConnection(conn, newPlayerInstance))
        {
            if (_isMaster)
            {
                foreach (UnitMasterSO master in masterSOList)
                {
                    if (master.name == _class)
                    {
                        newPlayerInstance.GetComponent<UnitMaster>().Rpc_SetMasterClass(_class);
                        break;
                    }
                }
            }
            else if (!_isMaster)
            {
                foreach (SurvivorSO survivor in survivorSOList)
                {
                    if (survivor.name == _class)
                    {
                        newPlayerInstance.GetComponent<ActiveSurvivorClass>().Rpc_SetSurvivorClass(_class);
                        break;
                    }
                }
            }

            newPlayerInstance.GetComponent<BasePlayerData>().PlayerName = 
                oldPlayerInstance.GetComponent<LobbyPlayer>().PlayerName;

            GamemodeBase.Instance.Svr_AddPlayer(conn.identity.netId,  _isMaster);

            //When the new player instance is all set and ready, then destroy the old one, as it is no longer needed.
            NetworkServer.Destroy(oldPlayerInstance);
        }

        // Call Lobby method which invokes RelayOnServerSynchronize.
        StartCoroutine(DelaySynchronize(conn));
    }

    private IEnumerator DelaySynchronize(NetworkConnection conn)
    {
        yield return new WaitForSeconds(0.3f);
        Lobby.InvokeRelayOnServerSynchronize(conn);
    }
}
