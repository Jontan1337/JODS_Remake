using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class LobbyCharacters : NetworkBehaviour
{
    [Header("Player Info")]
    [SyncVar] public int lobbyCharID;
    [Header("References")]
    public LobbyPlayer player;
    public GameObject smoke;
    public new GameObject light;
    public GameObject nameTag;
    [Space]
    [SerializeField] private SkinnedMeshRenderer characterRenderer = null;

    private List<SurvivorSO> survivorSOList = new List<SurvivorSO>();

    private void Start()
    {
        survivorSOList = PlayableCharactersManager.instance.survivorSOList;
    }

    // Register when the lobby character is assigned to a parent player.
    public override void OnStartAuthority()
    {
        if (!isLocalPlayer) return;
        light.GetComponent<Light>().enabled = true;
    }
    [Server]
    public void Svr_GetCharacter()
    {
        if (player.hasSelectedACharacter)
        {
            Rpc_ChangeCharacter(player.survivorSO.name);
        }
    }

    [ClientRpc]
    public void Rpc_ChangeCharacter(string survivorName)
    {
        print("Rpc_ChangeCharacter :" + survivorName);
        foreach(SurvivorSO survivor in survivorSOList)
        {
            if (survivor.name == survivorName)
            {
                characterRenderer.material = survivor.survivorMaterial;
                characterRenderer.sharedMesh = survivor.survivorMesh;
                break;
            }
        }
    }

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
}