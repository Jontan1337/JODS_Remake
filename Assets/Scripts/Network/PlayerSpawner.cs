using System.Collections;
using UnityEngine;
using Mirror;
using System.Collections.Generic;

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

    public override void OnStartServer()
    {
        //Get both lits of playable characters, which are later used to spawn each player with their chosen class
        survivorSOList = PlayableCharactersManager.instance.GetAllSurvivors();
        masterSOList = PlayableCharactersManager.instance.GetAllMasters();

        //When a player connects to the new scene, then invoke the ReadyPlayer method
        Lobby.OnServerReadied += Svr_ReadyPlayer; //This method puts the player's info into a list, which will be used when spawning the player.
    }

    [ServerCallback]
    public void OnDestroy() => Lobby.OnServerReadied -= Svr_ReadyPlayer;

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

        //Spawn a new object for the player and set a reference to the new player instance.
        GameObject newPlayerInstance = Instantiate(_isMaster ? masterPrefab : survivorPrefab, transform.position, transform.rotation);

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
                        newPlayerInstance.GetComponent<ActiveSClass>().Rpc_SetSurvivorClass(_class);
                        break;
                    }
                }
            }

            //When the new player instance is all set and ready, then destroy the old one, as it is no longer needed.
            NetworkServer.Destroy(oldPlayerInstance);
        }
    }
}
