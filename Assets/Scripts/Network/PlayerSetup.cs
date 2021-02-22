using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    [SyncVar] public string playerName;

    [SerializeField] private GameObject playerHands;
    [SerializeField] private GameObject equipment;
    [SerializeField] private GameObject slotsUIParent;
    [SerializeField] private bool doIHaveAuthority;
    [SerializeField] private bool Survivor;
    [SerializeField] private TextMesh playerNameText;

    [SerializeField] private GameObject[] disableIfPlayer;
    [SerializeField] private GameObject[] disableIfNotPlayer;
    [SerializeField] private GameObject[] enableIfPlayer;
    [SerializeField] private GameObject[] enableIfNotPlayer;

    [SerializeField] private bool isMe;

    //Game Modes
    //Survival
    [SerializeField] private int points;

    public override void OnStartAuthority()
    {
        doIHaveAuthority = hasAuthority;
    }

    private void Start()
    {
        if (hasAuthority)
        {
            Cmd_SpawnEssentials();

            foreach (GameObject g in disableIfPlayer) { g.SetActive(false); }
            foreach (GameObject g in enableIfPlayer) { g.SetActive(true); }

            TryGetComponent(out Spectator spectator);
            // Only set player name if player is not a spectator.
            if (!spectator)
            {
                //CmdChangeName(PlayerPrefs.GetString("PlayerName"));
            }

            if (Survivor)
            {
                gameObject.layer = 15; //Ignore Raycast Layer
                playerNameText.text = playerName;
            }

            name = name + " (ME)";
        }
        else
        {
            foreach (GameObject g in disableIfNotPlayer) { g.SetActive(false); }
            foreach (GameObject g in enableIfNotPlayer) { g.SetActive(true); }
        }
    }

    [Command]
    private void Cmd_SpawnEssentials()
    {
        Svr_SpawnHands();
        Svr_SpawnEquipment();
    }
    [Server]
    private void Svr_SpawnHands()
    {
        GameObject GOPlayerHands = Instantiate(playerHands);
        NetworkServer.Spawn(GOPlayerHands, connectionToClient);
        GOPlayerHands.transform.SetParent(transform);
        GOPlayerHands.transform.localPosition = new Vector3(0.25f, 1.6f, 0.6f);
    }
    [Server]
    private void Svr_SpawnEquipment()
    {
        GameObject GOEquipment = Instantiate(equipment);
        NetworkServer.Spawn(GOEquipment, connectionToClient);
        GOEquipment.transform.SetParent(transform);
        GOEquipment.transform.localPosition = new Vector3();
    }


    //[Command]
    //public void CmdChangeName(string newName)
    //{
    //    playerName = newName;
    //    name = playerName;
    //    RpcChangeName(newName);
    //}
    //[ClientRpc]
    //public void RpcChangeName(string newName)
    //{
    //    playerName = newName;
    //    if (playerNameText)
    //    {
    //        playerNameText.text = newName;
    //    }
    //}

    //public void Die()
    //{
    //    CmdDestroyPlayer();
    //}

    //// Replace the player as a spectator when they die
    //[Command]
    //void CmdDestroyPlayer()
    //{
    //    Debug.Log(gameObject.name + " has died");
    //    GameObject spectator = (GameObject)Resources.Load("Spawnables/Spectator");
    //    Vector3 pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2, gameObject.transform.position.z);
    //    GameObject me = Instantiate(spectator, pos, gameObject.transform.rotation);
    //    NetworkServer.Spawn(me);

    //    if (NetworkServer.ReplacePlayerForConnection(connectionToClient, me))
    //    {
    //        NetworkServer.Destroy(gameObject);
    //    }
    //}

    //public void DestroyUnit(GameObject unit, float time)
    //{
    //    Debug.Log("Destroying unit : " + unit);
    //    if (hasAuthority)
    //    {
    //        CmdDestroyUnit(unit, time);
    //    }
    //}

    //[Command]
    //void CmdDestroyUnit(GameObject unit, float time)
    //{
    //    StartCoroutine(DestroyFX(time,unit));
    //}
    //IEnumerator DestroyFX(float tiem, GameObject go)
    //{
    //    yield return new WaitForSeconds(tiem);

    //    NetworkServer.Destroy(go);
    //}
}
