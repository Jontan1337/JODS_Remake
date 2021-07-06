using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbyCountdown : NetworkBehaviour
{
    [Header("Countdown")]
    [SerializeField] private bool countdownIsActive = false;
    [SerializeField, Range(0, 10)] private int countdownDuration = 5;
    [SerializeField] private Text countdownText = null;
    [SerializeField] private AudioClip countdownSound = null;

    public void StartCountdown()
    {
        //Start a countdown to start the game
        countdownCo = StartCoroutine(Countdown());
    }

    public void StopCountdown()
    {
        if (countdownIsActive)
        {
            StopCoroutine(countdownCo);
            countdownIsActive = false;
            Rpc_EnableCountdownText(false);
        }
    }

    private Coroutine countdownCo = null;

    private IEnumerator Countdown()
    {
        int countdown = countdownDuration;
        countdownIsActive = true;

        Rpc_EnableCountdownText(true);
        Rpc_ChangeCountdownText(countdown.ToString());

        while (countdown > 0)
        {
            yield return new WaitForSeconds(1);

            countdown -= 1;

            Rpc_ChangeCountdownText(countdown.ToString());

            //TODO : Add sound for each second passing.
        }

        //After the countdown duration has passed

        //Disable the countdown text 
        Rpc_EnableCountdownText(false);

        //Tell the server that the countdown is complete
        Svr_CountdownCompleted(); //This will eventually start the game
        //Tell all clients that the countdown is complete
        Rpc_CountdownCompleted(); //It just disables the ready button
    }

    [Server]
    private void Svr_CountdownCompleted()
    {
        Lobby.Instance.ServerCountdownCompleted();
    }
    [ClientRpc]
    private void Rpc_CountdownCompleted()
    {
        Lobby.Instance.ClientCountdownCompleted();
    }

    [ClientRpc]
    private void Rpc_EnableCountdownText(bool active)
    {
        countdownText.gameObject.SetActive(active);
    }

    [ClientRpc]

    private void Rpc_ChangeCountdownText(string newText)
    {
        countdownText.text = newText;
    }
}
