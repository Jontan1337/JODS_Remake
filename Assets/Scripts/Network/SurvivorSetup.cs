using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.Events;

public enum ActiveType
{
    OnlyLocal,
    OnlyOthers,
    All
}

[System.Serializable]
public struct DynamicItem {
    public string listItemName;
    public GameObject prefab;
    public Vector3 position;
    [Tooltip("Choose how the item should spawn. " +
            "\nOnlyLocal: Only active on the player that owns this object." +
            "\nOnlyOthers: Active on everyone except this player." +
            "\nAll: Active on all clients.")] public ActiveType activeType;
    public List<DynamicItem> children;
}

public class SurvivorSetup : NetworkBehaviour
{
    [Header("Settings")]
    public List<DynamicItem> dynamicItems;
    [SerializeField, Tooltip("A list of the equipment types, the player should start with.")]
    public List<EquipmentType> equipmentSlotsTypes = new List<EquipmentType>();
    public Action<GameObject> onServerSpawnItem;
    public Action<GameObject> onClientSpawnItem;
    public Action onDestroyPlayer;
    public Action onFinishedPlayerSetup;

    [Space]
    [SyncVar] public string playerName;
    [SerializeField] private TextMesh playerNameText = null; // Change to something else??

    [Header("First person setup")]
    public SkinnedMeshRenderer headMesh = null;
    [SerializeField] private SkinnedMeshRenderer bodyMesh = null;
    [SerializeField] private SkinnedMeshRenderer armsMesh = null;
    [SerializeField] private Transform armatureTransform = null;
    [SerializeField] private float armatureForwardOffset = -0.3f;

    [Header("References")]
    [SerializeField] private GameObject[] prefabDisableIfPlayer = null;
    [SerializeField] private GameObject[] prefabDisableIfNotPlayer = null;
    [SerializeField] private GameObject[] prefabEnableIfPlayer = null;
    [SerializeField] private GameObject[] prefabEnableIfNotPlayer = null;

    [Header("Runtime setup")]
    [SerializeField] private List<GameObject> disableIfPlayer = new List<GameObject>();
    [SerializeField] private List<GameObject> disableIfNotPlayer = new List<GameObject>();
    [SerializeField] private List<GameObject> enableIfPlayer = new List<GameObject>();
    [SerializeField] private List<GameObject> enableIfNotPlayer = new List<GameObject>();
    [SerializeField] private List<GameObject> dynamicallySpawnedItems;

    public override void OnStartServer()
    {
        if (NetworkTest.Instance != null)
        {
            NetworkTest.RelayOnServerAddPlayer += Svr_UpdateVars;
        }
        else
        {
            Lobby.RelayOnServerSynchronize += Svr_UpdateVars;
        }
    }
    public override void OnStopServer()
    {
        if (NetworkTest.Instance != null)
        {
            NetworkTest.RelayOnServerAddPlayer -= Svr_UpdateVars;
        }
        else
        {
            Lobby.RelayOnServerSynchronize -= Svr_UpdateVars;
        }
    }
    public override void OnStopClient()
    {
        // Mirror wtf why no callback for when about to destroy object the fuk??
        //onDestroyPlayer?.Invoke();
    }

    private void Start()
    {
        if (isServer)
            StartCoroutine(InitSetup());
    }

    [Server]
    private void Svr_UpdateVars(NetworkConnection target)
    {
        Rpc_SetSyncLists(
            target,
            disableIfPlayer,
            disableIfNotPlayer,
            enableIfPlayer,
            enableIfNotPlayer,
            dynamicallySpawnedItems
        );
        Rpc_InitSetup(target);
    }

    [TargetRpc]
    private void Rpc_SetSyncLists(
        NetworkConnection target,
        List<GameObject> disableIfPlayer,
        List<GameObject> disableIfNotPlayer,
        List<GameObject> enableIfPlayer,
        List<GameObject> enableIfNotPlayer,
        List<GameObject> dynamicallySpawnedItems)
    {
        this.disableIfPlayer = disableIfPlayer;
        this.disableIfNotPlayer = disableIfNotPlayer;
        this.enableIfPlayer = enableIfPlayer;
        this.enableIfNotPlayer = enableIfNotPlayer;
        this.dynamicallySpawnedItems = dynamicallySpawnedItems;
    }

    [TargetRpc]
    private void Rpc_InitSetup(NetworkConnection target)
    {
        StartCoroutine(InitSetup());
    }

    private IEnumerator InitSetup()
    {
        if (isServer)
        {
            Svr_SpawnItems();
            InitSpawnedItems();
            foreach (GameObject dynamicItem in dynamicallySpawnedItems)
            {
                onServerSpawnItem?.Invoke(dynamicItem);
            }
        }
        yield return new WaitForSeconds(0.1f);
        if (hasAuthority)
        {
            armsMesh.gameObject.layer = LayerMask.NameToLayer("FirstPerson");
            bodyMesh.gameObject.layer = LayerMask.NameToLayer("FirstPerson");
            headMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            armatureTransform.localPosition += new Vector3(0f, 0f, armatureForwardOffset);

            foreach (GameObject g in disableIfPlayer) { g.SetActive(false); }
            foreach (GameObject g in enableIfPlayer) { g.SetActive(true); }
            foreach (GameObject g in prefabDisableIfPlayer) { g.SetActive(false); }
            foreach (GameObject g in prefabEnableIfPlayer) { g.SetActive(true); }

            InitSpawnedItems();

            foreach (GameObject dynamicItem in dynamicallySpawnedItems)
            {
                onClientSpawnItem?.Invoke(dynamicItem);
            }

            name = name + " (ME)";
        }
        else
        {
            foreach (GameObject g in disableIfNotPlayer) { g.SetActive(false); }
            foreach (GameObject g in enableIfNotPlayer) { g.SetActive(true); }
            foreach (GameObject g in prefabDisableIfNotPlayer) { g.SetActive(false); }
            foreach (GameObject g in prefabEnableIfNotPlayer) { g.SetActive(true); }
        }
        onFinishedPlayerSetup?.Invoke();
    }

    private void InitSpawnedItems()
    {
        foreach (GameObject dynamicItem in dynamicallySpawnedItems)
        {
            dynamicItem.TryGetComponent(out IInitializable<SurvivorSetup> initializable);
            initializable?.Init(this);
        }
    }

    [Command]
    private void Cmd_SpawnEssentials()
    {
        Svr_SpawnItems();
    }

    [Server]
    private void Svr_RecursiveChildren(DynamicItem child, Transform parent)
    {
        GameObject GOItem = Svr_SpawnDynamicItem(child, parent);
        Svr_CheckType(child.activeType, GOItem);
        List<DynamicItem> dynamicChildren = child.children;
        // Loop through current child's children.
        foreach (var childsChild in dynamicChildren)
        {
            Svr_RecursiveChildren(childsChild, GOItem.transform);
        }
    }

    [Server]
    private void Svr_SpawnItems()
    {
        // Loop through top parents in the list.
        foreach (DynamicItem item in dynamicItems)
        {
            GameObject GOItem = Svr_SpawnDynamicItem(item, transform);
            Svr_CheckType(item.activeType, GOItem);
            List<DynamicItem> dynamicChildren = item.children;
            // Loop through current parent's children.
            foreach (DynamicItem child in dynamicChildren)
            {
                Svr_RecursiveChildren(child, GOItem.transform);
            }
        }
    }

    [Server]
    private void Svr_CheckType(ActiveType type, GameObject GOItem)
    {
        switch (type)
        {
            case ActiveType.OnlyLocal:
                disableIfNotPlayer.Add(GOItem);
                enableIfPlayer.Add(GOItem);
                break;
            case ActiveType.OnlyOthers:
                disableIfPlayer.Add(GOItem);
                enableIfNotPlayer.Add(GOItem);
                break;
            case ActiveType.All:
                enableIfPlayer.Add(GOItem);
                enableIfNotPlayer.Add(GOItem);
                break;
            default:
                break;
        }
    }

    [Server]
    private GameObject Svr_SpawnDynamicItem(DynamicItem item, Transform parent)
    {
        GameObject GOItem = Instantiate(item.prefab);
        NetworkServer.Spawn(GOItem, connectionToClient);
        GOItem.transform.localPosition = item.position;
        GOItem.transform.SetParent(parent, false);
        dynamicallySpawnedItems.Add(GOItem);
        return GOItem;
    }

    [TargetRpc]
    public void Rpc_ToggleHead(NetworkConnection target)
    {
        var shadowsOnly = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        var on = UnityEngine.Rendering.ShadowCastingMode.On;
        print(headMesh.shadowCastingMode == shadowsOnly);
        headMesh.shadowCastingMode = (headMesh.shadowCastingMode == shadowsOnly) ? on : shadowsOnly;
    }
}
