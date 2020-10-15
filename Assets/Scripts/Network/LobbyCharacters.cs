using UnityEngine;
using Mirror;

public class LobbyCharacters : NetworkBehaviour
{
    [SyncVar] public int lobbyCharID;
    public LobbyPlayer player;
    public GameObject smoke;
    public new GameObject light;
    public GameObject nameTag;
    public GameObject[] characters;
    [SyncVar] public int chosenCharacterIndex = 0;

    // Register when the lobby character is assigned to a parent player.
    public override void OnStartAuthority()
    {
        if (!isLocalPlayer) return;
        light.GetComponent<Light>().enabled = true;
    }

    [Server]
    public void Svr_ChangeCharacter(int index)
    {
        chosenCharacterIndex = index;
        Rpc_Deactivate();
        Rpc_Activate(chosenCharacterIndex);
    }

    [Server]
    public void Svr_GetCharacter()
    {
        Rpc_Deactivate();
        Rpc_Activate(chosenCharacterIndex);
    }

    //public void GetChoice()
    //{
    //    Cmd_GetChoice();
    //}
    //[Command]
    //private void Cmd_GetChoice()
    //{
    //    print(player.wantsToBeMaster);
    //    if (player.wantsToBeMaster)
    //    {
    //        Rpc_GetChoice();
    //    }
    //}
    //[ClientRpc]
    //private void Rpc_GetChoice()
    //{
    //    smoke.GetComponent<ParticleSystem>().Play();
    //}
    [Server]
    public void Svr_GetChoice()
    {
        Rpc_ToggleChoice(player.wantsToBeMaster);
    }
    [ClientRpc]
    public void Rpc_ToggleChoice(bool playSmoke)
    {
        ParticleSystem charSmoke = smoke.GetComponent<ParticleSystem>();
        if (playSmoke)
        {
            charSmoke.Play();
        }
        else
        {
            charSmoke.Stop();
        }
    }

    [Server]
    public void Svr_GetNameTag()
    {
        nameTag.GetComponent<TextMesh>().text = player.playerName;
        Rpc_GetNameTag(player.gameObject);
    }
    [ClientRpc]
    private void Rpc_GetNameTag(GameObject sharedDataName)
    {
        nameTag.GetComponent<TextMesh>().text = sharedDataName.GetComponent<LobbyPlayer>().playerName;
    }

    [ClientRpc]
    public void Rpc_Activate(int index)
    {
        characters[index].SetActive(true);
    }
    [ClientRpc]
    public void Rpc_Deactivate()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(false);
        }
    }
}