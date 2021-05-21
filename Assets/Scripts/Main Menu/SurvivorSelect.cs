﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SurvivorSelect : NetworkBehaviour
{
    public SurvivorSO survivor;
    [Space]
    [SyncVar, SerializeField] private bool selected;
    [SyncVar, SerializeField] private int playerIndex;
    [Header("Visual")]
    [SerializeField] private GameObject selectedVisual;

    private void Start()
    {
        Select(false);
    }
    private void Select(bool value)
    {
        selected = value;
        selectedVisual.SetActive(value);
    }
    [Server]
    public void Svr_Select(int index)
    {
        if (selected)
        {
            if (index != playerIndex) return;
        }


        bool value = !selected;
        selected = value;
        selectedVisual.SetActive(value);
    }
}