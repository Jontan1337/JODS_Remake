using System.Collections;
using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject masterPrefab;
    [SerializeField] private GameObject survivorPrefab;

    private bool spawned;
    private bool isMaster = false;

    private List<SurvivorSO> survivorSOList = new List<SurvivorSO>();
    private List<MasterSO> masterSOList = new List<MasterSO>();

    private void Start()
    {
        survivorSOList = PlayableCharactersManager.instance.survivorSOList;
        masterSOList = PlayableCharactersManager.instance.masterSOList;
    }

    public override void OnStartAuthority()
    {
        // Check if this is currently the server and if the object is is not the same as the host.
        if (!hasAuthority) return;

        if (!spawned)
        {
            spawned = true;
            isMaster = PlayerPrefs.GetInt("IsMaster") == 1;

            string _class = isMaster ? PlayerPrefs.GetString("Master") : PlayerPrefs.GetString("Survivor");

            Cmd_ReplacePlayer(gameObject, isMaster, _class);
        }
    }

    [Command]
    void Cmd_ReplacePlayer(GameObject playerOwner, bool _isMaster, string _class)
    {
        GameObject go = Instantiate(_isMaster ? masterPrefab : survivorPrefab, transform.position, transform.rotation);

        if (_isMaster)
        {
            foreach (MasterSO master in masterSOList)
            {
                if (master.name == _class)
                {
                    go.GetComponent<Master>().SetMasterClass(master);
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
                    go.GetComponent<ActiveSClass>().SetSurvivorClass(survivor);
                    break;
                }
            }
        }

        //TODO :
        //Get a reference to the list of survivors and masters, so that we can fetch the SO for the master/survivor.
        //Then assign the SO to the new GO

        print($"New Player: {go.name} For {playerOwner.name}");

        NetworkServer.ReplacePlayerForConnection(connectionToClient, go);

        NetworkServer.Destroy(gameObject);
    }
}
