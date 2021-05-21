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
            "\nLocal: Called on server only." +
            "\nClientRPC: Called on all clients." +
            "\nTargetRPC: Called on the player that owns this object.")] public ActiveType activeType;
    public List<DynamicItem> children;
}

public class PlayerSetup : NetworkBehaviour
{
    public List<DynamicItem> dynamicItems;

    public Action<GameObject> onSpawnItem;

    [Space]
    [Space]
    [SyncVar] public string playerName;

    [Header("Prefabs for player setup")]
    [SerializeField] private GameObject playerHands;
    [SerializeField] private GameObject equipment;
    [SerializeField] private GameObject slotsUIParent;
    [SerializeField] private TextMesh playerNameText;
    [SerializeField, Tooltip("A list of the equipment types, the player should start with.")]
    public List<EquipmentType> equipmentSlotsTypes = new List<EquipmentType>();

    [Header("References")]

    [Space]

    [SerializeField] private List<GameObject> disableIfPlayer = new List<GameObject>();
    [SerializeField] private List<GameObject> disableIfNotPlayer = new List<GameObject>();
    [SerializeField] private List<GameObject> enableIfPlayer = new List<GameObject>();
    [SerializeField] private List<GameObject> enableIfNotPlayer = new List<GameObject>();
    [SerializeField] private GameObject[] prefabDisableIfPlayer;
    [SerializeField] private GameObject[] prefabDisableIfNotPlayer;
    [SerializeField] private GameObject[] prefabEnableIfPlayer;
    [SerializeField] private GameObject[] prefabEnableIfNotPlayer;

    [SerializeField] private List<GameObject> dynamicallySpawnedItems;

    [SerializeField] private bool Survivor;
    [SerializeField] private bool isMe;
    [SerializeField] private int points;

    public override void OnStartServer()
    {
        if (isServer)
        {
            NetworkTest.RelayOnServerAddPlayer += Svr_UpdateVars;
        }
    }
    public override void OnStopServer()
    {
        if (isServer)
        {
            NetworkTest.RelayOnServerAddPlayer -= Svr_UpdateVars;
        }
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
        }
        yield return new WaitForSeconds(0.1f);
        if (hasAuthority)
        {
            foreach (GameObject g in disableIfPlayer) { g.SetActive(false); }
            foreach (GameObject g in enableIfPlayer) { g.SetActive(true); }
            foreach (GameObject g in prefabDisableIfPlayer) { g.SetActive(false); }
            foreach (GameObject g in prefabEnableIfPlayer) { g.SetActive(true); }

            foreach (GameObject dynamicItem in dynamicallySpawnedItems)
            {
                dynamicItem.TryGetComponent(out IInitializable<PlayerSetup> initializable);
                initializable?.Init(this);
            }

            foreach (GameObject dynamicItem in dynamicallySpawnedItems)
            {
                onSpawnItem?.Invoke(dynamicItem);
            }

            TryGetComponent(out Spectator spectator);
            // Only set player name if player is not a spectator.
            if (!spectator)
            {
                //CmdChangeName(PlayerPrefs.GetString("PlayerName"));
            }

            if (Survivor)
            {
                gameObject.layer = 15; //Ignore Raycast Layer
                playerNameText.text = playerName;
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
                //LocalSetup(prefabItem, parentTransform);
                break;
            case ActiveType.OnlyOthers:
                disableIfPlayer.Add(GOItem);
                enableIfNotPlayer.Add(GOItem);
                //Rpc_ClientSetup(prefabItem, parentTransform);
                break;
            case ActiveType.All:
                enableIfPlayer.Add(GOItem);
                enableIfNotPlayer.Add(GOItem);
                //Rpc_TargetSetup(connectionToClient, prefabItem, parentTransform);
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
        GOItem.transform.SetParent(parent);
        GOItem.transform.localPosition = item.position;
        dynamicallySpawnedItems.Add(GOItem);
        return GOItem;
    }

    //private void LocalSetup(GameObject prefabItem, Transform parentTransform)
    //{
    //    GameObject GOItem = Instantiate(prefabItem);
    //    NetworkServer.Spawn(GOItem, connectionToClient);
    //    GOItem.transform.SetParent(transform);
    //    onSpawnItem?.Invoke(GOItem);
    //}
    //[ClientRpc]
    //private void Rpc_ClientSetup(GameObject prefabItem, Transform parentTransform)
    //{
    //    GameObject GOItem = Instantiate(prefabItem);
    //    NetworkServer.Spawn(GOItem, connectionToClient);
    //    GOItem.transform.SetParent(parentTransform);
    //    onSpawnItem?.Invoke(GOItem);
    //}
    //[TargetRpc]
    //private void Rpc_TargetSetup(NetworkConnection target, GameObject prefabItem, Transform parentTransform)
    //{
    //    GameObject GOItem = Instantiate(prefabItem);
    //    NetworkServer.Spawn(GOItem, connectionToClient);
    //    GOItem.transform.SetParent(parentTransform);
    //    onSpawnItem?.Invoke(GOItem);
    //}

    //[Server]
    //private void Svr_SpawnHands()
    //{
    //    GameObject GOPlayerHands = Instantiate(playerHands);
    //    NetworkServer.Spawn(GOPlayerHands, connectionToClient);
    //    GOPlayerHands.transform.SetParent(playerHandsParent);
    //    GOPlayerHands.transform.localPosition = new Vector3(0.25f, 0f, 0.6f);
    //    playerEquipment.playerHands = GOPlayerHands.transform;
    //}


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
