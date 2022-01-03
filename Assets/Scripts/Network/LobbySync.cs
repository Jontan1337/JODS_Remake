using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbySync : NetworkBehaviour
{
    [Header("Session Info")]
    private List<GameObject> PlayerLabels = new List<GameObject>();
    public LobbySeat[] lobbySeats = null;
    public GameObject[] playersInLobby = null;
    public LobbyAudio sound = null;



    #region Singleton
    private static LobbySync lobbySync;
    public static LobbySync Instance { get => lobbySync; }

    private void Awake()
    {
        if (lobbySync == null)
        {
            lobbySync = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    [Server]
    public void Svr_AddPlayer(int playerID)
    {
        #region First Time Setup
        if (playersInLobby.Length == 0)
        {
            playersInLobby = new GameObject[Lobby.Instance.maxConnections];
        }
        #endregion

        //print($"playerID: {playerID}");
        //print($"roomSlots Count: {Lobby.Instance.roomPlayers.Count}");
        // If player doesn't have a character already
        // then setup a new character for that player.
        if (playersInLobby[playerID] == null)
        {
            // Get the LobbyPlayer component on the current index
            // from the lobby slots list.
            LobbyPlayer currentPlayer = Lobby.Instance.roomPlayers[playerID].GetComponent<LobbyPlayer>();

            LobbyCharacters lobbyPlayerCharacter = currentPlayer.GetComponent<LobbyCharacters>();

            // Set the current player ID to that players connection ID.
            currentPlayer.playerID = playerID;

            lobbyPlayerCharacter.lobbyCharID = currentPlayer.playerID;

            // Add the new player from the lobbySlots array to the playersInLobby array.
            playersInLobby[playerID] = currentPlayer.gameObject;

            // Look for an available seat.
            foreach (LobbySeat seat in lobbySeats)
            {
                if (!seat.isTaken)
                {
                    // Place the player character at the seat
                    lobbyPlayerCharacter.transform.position = seat.transform.position;
                    lobbyPlayerCharacter.transform.rotation = seat.transform.rotation;
                    Rpc_SyncTransform(lobbyPlayerCharacter.gameObject, seat.gameObject);
                    seat.isTaken = true;
                    seat.player = currentPlayer;
                    currentPlayer.playerSeat = seat;
                    break;
                }
            }

            SetLobbyCamera(currentPlayer.gameObject);

            sound.PlaySound("Join");
            // Run through and synchronize all the players.
            for (int x = 0; x < playersInLobby.Length; x++)
            {
                if (playersInLobby[x])
                {
                    GameObject tempLobbyPlayer = playersInLobby[x].gameObject;
                    LobbyCharacters tempLobbyCharacter = tempLobbyPlayer.GetComponent<LobbyCharacters>();

                    StartCoroutine(GetPlayerInfo(tempLobbyCharacter));
                    RotateNameTagCo = StartCoroutine(RotateNameTag(tempLobbyPlayer, lobbyPlayerCharacter.gameObject));
                }
            }
        }
    }

    private IEnumerator GetPlayerInfo(LobbyCharacters tempLobbyCharacter)
    {
        // Wait for a small amount of time because the network is delayed or something?
        yield return new WaitForSeconds(0.2f);
        
        tempLobbyCharacter.Svr_GetCharacter();
        tempLobbyCharacter.Svr_GetChoice();
        tempLobbyCharacter.Svr_GetNameTag();
    }

    [ClientRpc]
    public void Rpc_SyncTransform(GameObject playerCharacter, GameObject playerSeat)
    {
        playerCharacter.transform.position = playerSeat.transform.position;
        playerCharacter.transform.rotation = playerSeat.transform.rotation;
    }

    private Coroutine RotateNameTagCo;
    private IEnumerator RotateNameTag(GameObject player, GameObject character)
    {
        // Wait for a small amount of time because the network is delayed or something?
        yield return new WaitForSeconds(0.2f);

        while (true)
        {
            Rpc_RotateNameTag(player, character);
            yield return new WaitForSeconds(0.1f);
        }

    }
    public void StopRotation()
    {
        StopCoroutine(RotateNameTagCo);
    }
    private void SetLobbyCamera(GameObject player)
    {
        Rpc_SetLobbyCamera(player);
    }

    [ClientRpc]
    private void Rpc_SetLobbyCamera(GameObject player)
    {
        LobbyPlayer tempLP = player.GetComponent<LobbyPlayer>();
        tempLP.lobbyCamera = Lobby.Instance.mainCamera;
        tempLP._lobbyCam = tempLP.lobbyCamera.GetComponent<Camera>();
    }

    [ClientRpc]
    private void Rpc_RotateNameTag(GameObject player, GameObject character)
    {
        // Set the nametag rotation to face the camera.
        character.GetComponent<LobbyCharacters>().nameTag.transform.rotation = 
            Quaternion.LookRotation
            (
                character.GetComponent<LobbyCharacters>().nameTag.transform.position - 
                Lobby.Instance.mainCamera.transform.position
            );
    }

    [ClientRpc]
    public void Rpc_EnableControls()
    {
        JODSInput.Controls.Enable();
    }
    [ClientRpc]
    public void Rpc_DisableControls()
    {
        JODSInput.Controls.Disable();
    }
}