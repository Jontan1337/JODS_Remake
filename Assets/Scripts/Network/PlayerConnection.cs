using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerConnection : NetworkBehaviour
{
    [SyncVar] public bool Master;
    public bool me;
    [SyncVar] public bool isHost;
    [SyncVar] public new bool isClient;
    [SyncVar] public bool loaded;
    public GameObject[] survivorPrefab;
    public GameObject masterPrefab;
    GameObject myUnit;
    public LobbyPlayer player;
    public GameObject chooseCharacter;
    [SyncVar] public int survivorChoice;

    /*
    void Start()
    {
        chooseCharacter.SetActive(false);
        player = GameObject.Find("MyLobbyPlayer").GetComponent<LobbyPlayer>();
        if (player.playerName != "")
        {
            gameObject.name = player.playerName + "(connection)";
            player.gameObject.name = player.playerName + "(player)";
        }
        if (isLocalPlayer == false)
        {
            return;
        }
        if (isServer)
        {
            isHost = true;
        }
        else
        {
            isClient = true;
        }
        player.masterButton.gameObject.SetActive(false);
        player.survivorButton.gameObject.SetActive(false);
        player.playerNameInput.gameObject.SetActive(false);
        if (player.survivor) { chooseCharacter.SetActive(true); }
        me = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer == false)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            SceneManager.LoadScene(0);
        }
        if (!loaded)
        {
            player.masterButton.gameObject.SetActive(false);
            player.survivorButton.gameObject.SetActive(false);
            player.playerNameInput.gameObject.SetActive(false);
            player.youAre.gameObject.SetActive(false);
            if (player.master) { CmdSpawnMyUnit(0); }
        }
    }

    //////////////////////////////// SERVER COMMANDS
    //Client calls a command on the server

    //Spawn players moveable object
    [Command]
    void CmdSpawnMyUnit(int choice)
    {
        if (connectionToClient.isReady)
        {
            if (!loaded)
            {
                if (player.master)
                {
                    loaded = true;
                    Debug.Log(name + ": Spawning My Unit, role is : Master");
                    Master = true;
                    GameObject me = Instantiate(masterPrefab);
                    myUnit = me;
                    me.GetComponent<PlayerUnit>().playerName = player.playerName;
                }
                else
                {
                    loaded = true;
                    Debug.Log(name + ": Spawning My Unit, role is : Survivor");
                    Master = false;
                    GameObject me = Instantiate(survivorPrefab[choice]);
                    myUnit = me;
                    me.GetComponent<PlayerUnit>().playerName = player.playerName;
                }
                NetworkServer.SpawnWithClientAuthority(myUnit, connectionToClient);
                myUnit.GetComponent<PlayerUnit>().ChangeConn(GetComponent<NetworkIdentity>().netId);
                RpcDisableChoose();
            }
        }
    }

    [ClientRpc]
    void RpcDisableChoose()
    {
        chooseCharacter.SetActive(false);
    }

    [Command]
    void CmdDestroyPlayer(GameObject player)
    {
        if (player)
        {
            GameObject spectator = (GameObject)Resources.Load("Spectator");
            Vector3 pos = new Vector3(player.transform.position.x, player.transform.position.y + 2, player.transform.position.z);
            GameObject me = Instantiate(spectator, pos, player.transform.rotation);
            NetworkServer.SpawnWithClientAuthority(me, connectionToClient);
            NetworkServer.Destroy(player);
        }
    }

    //////////////////////////////// RPC
    //Server calls to clients

    [ClientRpc]
    public void RpcDestroyPlayer(GameObject player)
    {
        CmdDestroyPlayer(player);
    }

    public void Choice(int choice)
    {
        CmdChooseSurvivor(choice);
    }
    [Command]
    void CmdChooseSurvivor(int choice)
    {
        survivorChoice = choice;
        CmdSpawnMyUnit(choice);
    }
    */
}
