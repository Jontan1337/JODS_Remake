using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PreGamePlayer : NetworkBehaviour
{
    [SerializeField, SyncVar(hook = nameof(SetName))] private string playerName = ""; 
    [SerializeField] private Text playerNameText = null;

    [Server]
    public void Svr_SetName(string newPlayerName)
    {
        playerName = newPlayerName;
        SetName("", newPlayerName);
    }

    public void SetName(string old, string newName)
    {
        playerNameText.text = newName;
    }
}
