using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SurvivorSelect : NetworkBehaviour
{
    public SurvivorSO survivor;
    [Space]
    [SyncVar (hook = nameof(ShowVisuals)), SerializeField] private bool selected;
    [SyncVar] public int playerIndex;
    [SyncVar (hook = nameof(ChangeNameText)), SerializeField] private string playerName;
    [Header("Visual")]
    [SerializeField] private GameObject selectedVisual = null;
    [SerializeField] private TextMesh playerNameText = null;
    public bool Selected => selected;

    public void Select(bool value, bool removeSO = true)
    {
        selected = value;
        ShowVisuals(false , value);

        if (value == false)
        {
            if (removeSO) RemovePlayerSurvivorSO(playerIndex);

            playerIndex = 0;

            return;
        }

        ChangeNameText("", playerName);
    }

    [ClientRpc]
    public void Rpc_SelectSurvivor(int index, bool hasSelected, string playerName)
    {
        if (selected && index != playerIndex) return;

        if (hasSelected)
        {
            if (index != playerIndex) return;
        }

        if (index != playerIndex)
        {
            playerIndex = index;

            SetPlayerSurvivorSO(playerIndex);

            this.playerName = playerName;
        }

        Select(!selected);
    }

    public bool IsMySelection(int index)
    {
        if (!selected) return false;

        if (index == playerIndex) return true;

        return false;
    }


    private void SetPlayerSurvivorSO(int index)
    {
        NetworkIdentity player = NetworkIdentity.spawned[(uint)index];

        player.GetComponent<LobbyPlayer>()?.SetSurvivorSO(survivor);
        player.GetComponent<SurvivorSelector>().ChangeCharacter(survivor.name);
    }

    private void RemovePlayerSurvivorSO(int index)
    {
        NetworkIdentity player = NetworkIdentity.spawned[(uint)index];

        player?.GetComponent<LobbyPlayer>()?.SetSurvivorSO(null);
        player?.GetComponent<SurvivorSelector>().ChangeCharacter(null);
    }

    private void ShowVisuals(bool oldVal, bool newVal)
    {
        selectedVisual.SetActive(newVal);
    }

    private void ChangeNameText(string oldVal, string newVal)
    {
        playerNameText.text = newVal;
    }
            
    [Server]    //This method is called by the server (lobby manager)
    public void Svr_OverrideSelection()
    {
        //This will simply override any changes made to this, resetting it to the default state of Not Selected.
        //Meaning that no player has chosen this survivor, making it available to be chosen.
        //Only the server will see this though, so no need to call RemovePlayerSurvivorSO, 
        //as only the server will use this script after this method has been called.

        selected = false;
    }
}
