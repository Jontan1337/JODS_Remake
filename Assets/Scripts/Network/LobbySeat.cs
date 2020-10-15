using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbySeat : NetworkBehaviour
{
    [SyncVar] public bool isTaken = false;
    public LobbyPlayer player;
}
