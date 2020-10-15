using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyAudio : NetworkBehaviour
{
    [SerializeField] private AudioSource source = null;
    [SerializeField] private AudioClip PlayerJoin = null;
    [SerializeField] private AudioClip PlayerLeave = null;
    [SerializeField] private AudioClip PlayerChange = null;

    public void PlaySound(string soundType)
    {
        switch (soundType)
        {
            case "Join":
            case "Leave":
                Rpc_Play(soundType);
                break;

            case "Change":
                Play(soundType);
                break;
        }
    }


    private void Play(string soundType)
    {
        switch (soundType)
        {
            case "Change":
                source.PlayOneShot(PlayerChange);
                break;
        }
    }

    [ClientRpc]
    private void Rpc_Play(string soundType)
    {
        switch (soundType)
        {
            case "Join":
                source.PlayOneShot(PlayerJoin);
                break;
            case "Leave":
                source.PlayOneShot(PlayerLeave);
                break;
        }
    }
}
