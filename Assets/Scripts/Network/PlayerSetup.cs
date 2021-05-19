using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.Events;

public enum CallType
{
    Local,
    ClientRPC,
    TargetRPC
}

[System.Serializable]
public struct DynamicItem {
    public GameObject prefab;
    public Vector3 position;
    [Tooltip("Choose how the item should spawn. " +
            "\nLocal: Called on server only." +
            "\nClientRPC: Called on all clients." +
            "\nTargetRPC: Called on the player that owns this object.")] public CallType callType;
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

    [Header("References from player setup")]
    [SerializeField] private Equipment playerEquipment;

    [Header("References")]
    [SerializeField] private Transform playerHandsParent;

    [Space]
    [SerializeField] private GameObject[] disableIfPlayer;
    [SerializeField] private GameObject[] disableIfNotPlayer;
    [SerializeField] private GameObject[] enableIfPlayer;
    [SerializeField] private GameObject[] enableIfNotPlayer;

    [SerializeField] private bool Survivor;
    [SerializeField] private bool isMe;
    [SerializeField] private int points;

    private void Start()
    {
        if (hasAuthority)
        {
            Cmd_SpawnEssentials();

            foreach (GameObject g in disableIfPlayer) { g.SetActive(false); }
            foreach (GameObject g in enableIfPlayer) { g.SetActive(true); }

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
        Svr_CallType(child.callType, child.prefab, parent);
        List<DynamicItem> dynamicChildren = child.children;
        foreach (var childsChild in dynamicChildren)
        {
            Svr_RecursiveChildren(childsChild, child.prefab.transform);
        }
    }

    [Server]
    private void Svr_SpawnItems()
    {
        foreach (DynamicItem item in dynamicItems)
        {
            
            Svr_CallType(item.callType, item.prefab, transform);
            List<DynamicItem> dynamicChildren = item.children;
            foreach (DynamicItem child in dynamicChildren)
            {
                Svr_RecursiveChildren(child, item.prefab.transform);
            }
        }







        //GameObject GOEquipment = Instantiate(equipment);
        //GOEquipment.GetComponent<Equipment>().equipmentSlotsTypes = equipmentSlotsTypes;
        //NetworkServer.Spawn(GOEquipment, connectionToClient);
        //GOEquipment.transform.SetParent(transform);
        //GOEquipment.transform.localPosition = new Vector3();
        //playerEquipment = GOEquipment.GetComponent<Equipment>();
        //onSpawnEquipment?.Invoke(playerEquipment);
    }

    [Server]
    private void Svr_CallType(CallType type, GameObject prefabItem, Transform parentTransform)
    {
        switch (type)
        {
            case CallType.Local:
                LocalSetup(prefabItem, parentTransform);
                break;
            case CallType.ClientRPC:
                Rpc_ClientSetup(prefabItem, parentTransform);
                break;
            case CallType.TargetRPC:
                Rpc_TargetSetup(connectionToClient, prefabItem, parentTransform);
                break;
            default:
                break;
        }
    }

    private void LocalSetup(GameObject prefabItem, Transform parentTransform)
    {
        GameObject GOItem = Instantiate(prefabItem);
        NetworkServer.Spawn(GOItem, connectionToClient);
        GOItem.transform.SetParent(transform);
        onSpawnItem?.Invoke(GOItem);
    }
    [ClientRpc]
    private void Rpc_ClientSetup(GameObject prefabItem, Transform parentTransform)
    {
        GameObject GOItem = Instantiate(prefabItem);
        NetworkServer.Spawn(GOItem, connectionToClient);
        GOItem.transform.SetParent(parentTransform);
        onSpawnItem?.Invoke(GOItem);
    }
    [TargetRpc]
    private void Rpc_TargetSetup(NetworkConnection target, GameObject prefabItem, Transform parentTransform)
    {
        GameObject GOItem = Instantiate(prefabItem);
        NetworkServer.Spawn(GOItem, connectionToClient);
        GOItem.transform.SetParent(parentTransform);
        onSpawnItem?.Invoke(GOItem);
    }

    [Server]
    private void Svr_SpawnHands()
    {
        GameObject GOPlayerHands = Instantiate(playerHands);
        NetworkServer.Spawn(GOPlayerHands, connectionToClient);
        GOPlayerHands.transform.SetParent(playerHandsParent);
        GOPlayerHands.transform.localPosition = new Vector3(0.25f, 0f, 0.6f);
        playerEquipment.playerHands = GOPlayerHands.transform;
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
