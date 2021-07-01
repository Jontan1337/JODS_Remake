using System.Collections;
using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject masterPrefab = null;
    [SerializeField] private GameObject survivorPrefab = null;

    [SerializeField] private List<SurvivorSO> survivorSOList = new List<SurvivorSO>();
    [SerializeField] private List<UnitMasterSO> masterSOList = new List<UnitMasterSO>();

    public override void OnStartServer()
    {
        survivorSOList = PlayableCharactersManager.instance.GetAllSurvivors();
        masterSOList = PlayableCharactersManager.instance.GetAllMasters();

        Lobby.OnServerReadied += SpawnPlayer;
    }

    [ServerCallback]
    public void OnDestroy() => Lobby.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn, string _class, bool _isMaster)
    {
        GameObject oldPlayerInstance = conn.identity.gameObject;

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
            NetworkServer.Destroy(oldPlayerInstance);
        }

        //StartCoroutine(IESpawnPlayer(conn, _class, _isMaster));
    }
}
