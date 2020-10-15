using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbySync : NetworkBehaviour
{
    private List<GameObject> PlayerLabels = new List<GameObject>();
    public LobbySeat[] lobbySeats = null;
    public GameObject[] playersInLobby = null;
    public LobbyAudio sound = null;

    public Transform matchList;
    public GameObject matchListItem;

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

    //public void PlayerChangeCharacter(GameObject player, int characterIndex)
    //{
    //    if (!hasAuthority) return;
    //    LobbyPlayer currentPlayer = player.GetComponent<LobbyPlayer>();

    //    goPlayersInLobby[currentPlayer.playerID].GetComponent<LobbyCharacters>().Cmd_ChangeCharacter(characterIndex);
    //}


    #region Playerlabels list (NOT USED)
    [ClientRpc]
    private void Rpc_SyncLabels(GameObject listItem, string playerName)
    {
        listItem.GetComponentInChildren<Text>().text = playerName;
        listItem.transform.SetParent(matchList);
    }
    [Server]
    private void Svr_SyncLabels()
    {
        for (int i = 0; i < PlayerLabels.Count; i++)
        {
            Rpc_SyncLabels(PlayerLabels[i].gameObject, Lobby.Instance.roomPlayers[i].playerName);
        }
    }
    private IEnumerator CoSyncNames()
    {
        yield return new WaitForSeconds(0.3f);

        Svr_SyncLabels();
    }

    [Server]
    public void Svr_AddPlayerLabel(int playerID)
    {
        LobbyPlayer newLobbyPlayer = Lobby.Instance.roomPlayers[playerID];
        newLobbyPlayer.playerID = playerID;
        GameObject newListMatchItem = Instantiate(matchListItem, matchList);
        newListMatchItem.GetComponentInChildren<Text>().text = newLobbyPlayer.playerName;
        PlayerLabels.Add(newListMatchItem);
        NetworkServer.Spawn(newListMatchItem);
        newLobbyPlayer._nameLabel = newListMatchItem;
        newLobbyPlayer._nameLabelMasterColor = newListMatchItem.GetComponentInChildren<Image>();

        StartCoroutine(CoSyncNames());
    }
    #endregion


    [Server]
    public void Svr_AddPlayer(int playerID)
    {
        #region First Time Setup
        if (playersInLobby.Length == 0)
        {
            playersInLobby = new GameObject[Lobby.Instance.maxConnections];
        }
        #endregion

        print($"playerID: {playerID}");
        print($"roomSlots Count: {Lobby.Instance.roomPlayers.Count}");
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
                    StartCoroutine(RotateNameTag(tempLobbyPlayer, lobbyPlayerCharacter.gameObject));
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




    private IEnumerator RotateNameTag(GameObject player, GameObject character)
    {
        // Wait for a small amount of time because the network is delayed or something?
        yield return new WaitForSeconds(0.2f);

        Rpc_RotateNameTag(player, character);
    }
    private void SetLobbyCamera(GameObject player)
    {
        Rpc_SetLobbyCamera(player);
    }

    [ClientRpc]
    private void Rpc_SetLobbyCamera(GameObject player)
    {
        LobbyPlayer tempLP = player.GetComponent<LobbyPlayer>();
        tempLP.lobbyCamera = Lobby.Instance.lobbyCamera;
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
                Lobby.Instance.lobbyCamera.transform.position
            );
    }


    [Server]
    public void Svr_PlayerSmoke(GameObject chara)
    {
        chara.GetComponentInParent<LobbyCharacters>().Svr_GetChoice();
    }

    [Server]
    public void Svr_RemovePlayerLabel(int playerID)
    {
        NetworkServer.Destroy(PlayerLabels[playerID].gameObject);
        PlayerLabels.RemoveAt(playerID);
        sound.PlaySound("Leave");
    }
    [Server]
    // When a player disconnects, remove that character and reset that player seat.
    public void Svr_RemovePlayer(int playerID)
    {
        playersInLobby[playerID] = null;
        lobbySeats[playerID].player = null;
        lobbySeats[playerID].isTaken = false;
        NetworkServer.Destroy(playersInLobby[playerID]);
        sound.PlaySound("Leave");
    }
}