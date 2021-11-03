using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.UI;

// Used for managing an in game session.
public class MatchManager : NetworkBehaviour
{
    public List<SurvivorSetup> playerList = new List<SurvivorSetup>();
    public Action onCountdownFinished;

    [SerializeField] private GameObject initializingUI;
    [SerializeField] private Text countdownText = null;

    private int countdownTimer = 0;


    private void Awake()
    {
        if (!isServer) return;

        onCountdownFinished += Svr_BeginMatch;
    }

    [Server]
    public IEnumerator Svr_StartInitialization()
    {
        if (!isServer) yield break;

        yield return new WaitForSeconds(0.1f);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
            playerList.Add(player.GetComponent<SurvivorSetup>());

        Svr_SetupPlayers();
        StartCoroutine(Svr_Countdown());
    }

    [Server]
    private void Svr_SetupPlayers()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            //playerList[i].Rpc_Initialize(playerList[i].connectionToClient);
        }
    }

    [Server]
    private IEnumerator Svr_Countdown()
    {
        for (int i = 0; i <= 3; i++)
        {
            countdownText.text = $"{countdownTimer}";
            yield return new WaitForSeconds(1f);
            countdownTimer--;
        }

        //onCountdownFinished?.Invoke();
        Svr_BeginMatch();
    }

    [Server]
    private void Svr_BeginMatch()
    {
        //for (int i = 0; i < playerList.Count; i++)
        //{
        //    playerList[i].Rpc_RemoveBlindfold(playerList[i].connectionToClient);
        //}
    }
}
