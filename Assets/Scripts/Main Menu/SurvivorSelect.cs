﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SurvivorSelect : NetworkBehaviour
{
    public SurvivorSO survivor;
    [Space]
    [SyncVar (hook = nameof(ShowVisuals)), SerializeField] private bool selected;
    [SyncVar] public int playerIndex;
    [Header("Visual")]
    [SerializeField] private GameObject selectedVisual = null;

    public void Select(bool value, bool removeSO = true)
    {
        selected = value;
        ShowVisuals(false , value);

        if (value == false)
        {
            if (removeSO) RemovePlayerSurvivorSO(playerIndex);

            playerIndex = 0;
        }
    }

    [ClientRpc]
    public void Rpc_SelectSurvivor(int index, bool hasSelected)
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

}
