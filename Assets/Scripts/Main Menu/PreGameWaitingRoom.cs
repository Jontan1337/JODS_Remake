using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PreGameWaitingRoom : NetworkBehaviour
{
    [SerializeField] private GameObject PreGamePlayerPrefab = null;
    [SerializeField] private Transform PreGamePlayerContainer = null;
    [Space]
    [SerializeField] private GameObject notReadyText = null;
    [SerializeField] private GameObject readyText = null;
    [Space]

    public int playersToWaitFor = 0;
    [SerializeField] private int playersLoaded = 0;

    public static PreGameWaitingRoom Instance;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnStartServer()
    {
        //When a player connects to the new scene, then invoke the ReadyPlayer method
        Lobby.OnServerReadied += SpawnPreGamePlayer; //This method puts the player's info into a list, which will be used when spawning the player.
    }

    public void SpawnPreGamePlayer(string playerName)
    {
        //Just an error catcher. This should theoretically never happen though.
        if (playersLoaded == playersToWaitFor)
        {
            Debug.LogWarning("Tried to spawn more players than expected. This should not happen.");
            return;
        }

        //Spawn a preGamePlayer with the name of the new player.
        GameObject playerInstance = Instantiate(PreGamePlayerPrefab, PreGamePlayerContainer);
        NetworkServer.Spawn(playerInstance); //Spawn it on the server.

        //Set the name of the new object.
        playerInstance.GetComponent<PreGamePlayer>().Svr_SetName(playerName);

        //Increment the int.
        playersLoaded++;

        HasEveryoneLoadedCheck();
    }

    private void HasEveryoneLoadedCheck()
    {
        if (playersLoaded == playersToWaitFor)
        {
            Svr_Ready(true);
        }
    }

    [Server]
    public void Svr_Ready(bool ready)
    {
        Rpc_Ready(ready);

        //If all players are ready
        if (ready)
        {
            //Tell the lobby manager that all players have finished loading the scene.
            //This will initiate a pre-game setup necessary for the game to work.
            Lobby.Instance.AllPlayersHaveLoaded();
        }
    }

    [ClientRpc]
    private void Rpc_Ready(bool ready)
    {
        notReadyText.SetActive(!ready);
        readyText.SetActive(ready);
    }
}
