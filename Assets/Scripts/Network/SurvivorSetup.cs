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
    public Action<GameObject> onSpawnItem;
    public Action onDestroyPlayer;

    [Space]
    [SyncVar] public string playerName;
    [SerializeField] private TextMesh playerNameText = null; // Change to something else??

    [Header("First person setup")]
    [SerializeField] private Transform headTransform = null;
    [SerializeField] private Transform bodyTransform = null;

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
    private void Rpc_SetSyncLists(NetworkConnection target,
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
        }
        yield return new WaitForSeconds(0.1f);
        if (hasAuthority)
        {
            headTransform.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            bodyTransform.localPosition += new Vector3(0f, 0f, -0.2f);

            foreach (GameObject g in disableIfPlayer) { g.SetActive(false); }
            foreach (GameObject g in enableIfPlayer) { g.SetActive(true); }
            foreach (GameObject g in prefabDisableIfPlayer) { g.SetActive(false); }
            foreach (GameObject g in prefabEnableIfPlayer) { g.SetActive(true); }

            InitSpawnedItems();

            foreach (GameObject dynamicItem in dynamicallySpawnedItems)
            {
                onSpawnItem?.Invoke(dynamicItem);
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
        //Svr_SpawnHands();
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
            Svr_RecursiveChildren(childsChild, parent);
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

    //[Command]
    //public void CmdChangeName(string newName)
    //{
    //    playerName = newName;
    //    name = playerName;
    //    RpcChangeName(newName);
    //}
    //[ClientRpc]
    //public void RpcChangeName(string newName)
    //{
    //    playerName = newName;
    //    if (playerNameText)
    //    {
    //        playerNameText.text = newName;
    //    }
    //}

    //public void Die()
    //{
    //    CmdDestroyPlayer();
    //}

    //// Replace the player as a spectator when they die
    //[Command]
    //void CmdDestroyPlayer()
    //{
    //    Debug.Log(gameObject.name + " has died");
    //    GameObject spectator = (GameObject)Resources.Load("Spawnables/Spectator");
    //    Vector3 pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2, gameObject.transform.position.z);
    //    GameObject me = Instantiate(spectator, pos, gameObject.transform.rotation);
    //    NetworkServer.Spawn(me);

    //    if (NetworkServer.ReplacePlayerForConnection(connectionToClient, me))
    //    {
    //        NetworkServer.Destroy(gameObject);
    //    }
    //}

    //public void DestroyUnit(GameObject unit, float time)
    //{
    //    Debug.Log("Destroying unit : " + unit);
    //    if (hasAuthority)
    //    {
    //        CmdDestroyUnit(unit, time);
    //    }
    //}

    //[Command]
    //void CmdDestroyUnit(GameObject unit, float time)
    //{
    //    StartCoroutine(DestroyFX(time,unit));
    //}
    //IEnumerator DestroyFX(float tiem, GameObject go)
    //{
    //    yield return new WaitForSeconds(tiem);

    //    NetworkServer.Destroy(go);
    //}
}
