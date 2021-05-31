using System.Collections;
using UnityEngine;
using Mirror;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject masterPrefab;
    [SerializeField] private GameObject survivorPrefab;

    private bool spawned;
    private bool isMaster = false;

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

        //TODO :
        //Get a reference to the list of survivors and masters, so that we can fetch the SO for the master/survivor.
        //Then assign the SO to the new GO

        print($"New Player: {go.name} For {playerOwner.name}");

        NetworkServer.ReplacePlayerForConnection(connectionToClient, go);

        NetworkServer.Destroy(gameObject);
    }
}
