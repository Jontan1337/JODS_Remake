using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerUnit : NetworkBehaviour
{
    public GameObject cam;
    [SyncVar] public string playerName;
    public bool doIHaveAuthority;
    public bool Survivor;
    public TextMesh playerNameText;

    public GameObject[] disableIfPlayer;
    public GameObject[] disableIfNotPlayer;
    public GameObject[] enableIfPlayer;
    public GameObject[] enableIfNotPlayer;

    public bool isMe;

    //Game Modes
    //Survival
    public int points;

    public override void OnStartAuthority()
    {
        doIHaveAuthority = hasAuthority;
    }

    private void Start()
    {
        if (hasAuthority)
        {
            NetworkServer.SpawnObjects();
            foreach (GameObject g in disableIfPlayer) { g.SetActive(false); }
            foreach (GameObject g in enableIfPlayer) { g.SetActive(true); }
            cam.gameObject.SetActive(true);

            TryGetComponent(out Spectator spectator);
            // Only set player name if player is not a spectator.
            if (!spectator)
            {
                CmdChangeName(PlayerPrefs.GetString("PlayerName"));
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
            cam.gameObject.SetActive(false);
        }
    }

    [Command]
    public void CmdChangeName(string newName)
    {
        playerName = newName;
        name = playerName;
        RpcChangeName(newName);
    }
    [ClientRpc]
    public void RpcChangeName(string newName)
    {
        playerName = newName;
        if (playerNameText)
        {
            playerNameText.text = newName;
        }
    }

    public void Die()
    {
        CmdDestroyPlayer();
    }

    // Replace the player as a spectator when they die
    [Command]
    void CmdDestroyPlayer()
    {
        Debug.Log(gameObject.name + " has died");
        GameObject spectator = (GameObject)Resources.Load("Spawnables/Spectator");
        Vector3 pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2, gameObject.transform.position.z);
        GameObject me = Instantiate(spectator, pos, gameObject.transform.rotation);
        NetworkServer.Spawn(me);

        if (NetworkServer.ReplacePlayerForConnection(connectionToClient, me))
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    public void DestroyUnit(GameObject unit, float time)
    {
        Debug.Log("Destroying unit : " + unit);
        if (hasAuthority)
        {
            CmdDestroyUnit(unit, time);
        }
    }

    [Command]
    void CmdDestroyUnit(GameObject unit, float time)
    {
        StartCoroutine(DestroyFX(time,unit));
    }
    IEnumerator DestroyFX(float tiem, GameObject go)
    {
        yield return new WaitForSeconds(tiem);

        NetworkServer.Destroy(go);
    }
}
