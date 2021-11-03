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
    [SerializeField] private SkinnedMeshRenderer bodyRenderer = null;
    [SerializeField] private SkinnedMeshRenderer headRenderer = null;

    [Header("Default Character")]
    [SerializeField] private Material defaultMaterial = null;
    [SerializeField] private Mesh defaultMesh = null;

    private ParticleSystem charSmoke;

    private List<SurvivorSO> survivorSOList = new List<SurvivorSO>();

    private void Start()
    {
        survivorSOList = PlayableCharactersManager.instance.GetAllSurvivors();
        charSmoke = smoke.GetComponent<ParticleSystem>();
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
        if (survivorName == null)
        {
            ChangeToDefaultCharacter();
            return;
        }

        foreach(SurvivorSO survivor in survivorSOList)
        {
            if (survivor.name == survivorName)
            {
                bodyRenderer.material = survivor.characterMaterial;
                headRenderer.material = survivor.characterMaterial;
                bodyRenderer.sharedMesh = survivor.bodyMesh;
                headRenderer.sharedMesh = survivor.headMesh;
                break;
            }
        }
    }

    private void ChangeToDefaultCharacter()
    {
        bodyRenderer.material = defaultMaterial;
        headRenderer.material = defaultMaterial;
        bodyRenderer.sharedMesh = defaultMesh;
        headRenderer.sharedMesh = defaultMesh;
    }

    [Server]
    public void Svr_GetChoice(bool want = false)
    {
        Rpc_ToggleChoice(want ? want : player.wantsToBeMaster);
    }
    [ClientRpc]
    public void Rpc_ToggleChoice(bool playSmoke)
    {
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
        nameTag.GetComponent<TextMesh>().text = player.PlayerName;
        Rpc_GetNameTag(player.gameObject);
    }
    [ClientRpc]
    private void Rpc_GetNameTag(GameObject sharedDataName)
    {
        nameTag.GetComponent<TextMesh>().text = sharedDataName.GetComponent<LobbyPlayer>().PlayerName;
    }
}