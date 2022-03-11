using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System;

public class EquipmentSlot : NetworkBehaviour
{
    public string slotName;
    public Action<GameObject, GameObject> onServerItemChange;

    [Header("Item info")]
    [SerializeField, SyncVar(hook = nameof(OnEquipmentChanged))] private GameObject equipmentItem;
    [SerializeField, SyncVar] private EquipmentType equipmentType = EquipmentType.None;

    [Header("Item index")]
    [SerializeField] private KeyCode keyCode;
    [SerializeField] private int keyNumber;

    [Header("Hotbar Slot Visuals")]
    [SerializeField] private GameObject uiSlot;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color deselectedColor = Color.gray;
    [SerializeField] private HotbarSlot hotbarSlot;
    [SerializeField] private Image slotImage;

    private PlayerEquipment playerEquipment;

    public GameObject UISlot
    {
        get => uiSlot;
        set
        {
            uiSlot = value;
            slotImage = UISlot.GetComponent<Image>();
            hotbarSlot = UISlot.GetComponent<HotbarSlot>();
            hotbarSlot.ToggleItemImage();
            hotbarSlot.SetItemName("─");
            hotbarSlot.SetItemType(EquipmentType.ToString());
        }
    }

    #region Properties
    public GameObject EquipmentItem
    {
        get => equipmentItem;
        private set
        {
            if (!isServer) return;

            GameObject oldItem = equipmentItem;
            equipmentItem = value;
            onServerItemChange?.Invoke(oldItem, equipmentItem);
        }
    }
    public EquipmentType EquipmentType { get => equipmentType; set => equipmentType = value; }
    #endregion

    #region Hooks
    private void OnEquipmentChanged(GameObject oldVal, GameObject newVal)
    {
        // This method call is weird since this hook is called by all clients.. wtf??
        Rpc_UpdateUI(connectionToClient, newVal);
    }
    #endregion

    private void Awake()
    {
        keyNumber = transform.GetSiblingIndex() + 1;
        keyCode = KeyCode.Alpha0 + keyNumber;
    }

    #region NetworkBehaviour Callbacks
    // ---- ON START ----
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
    public override void OnStartAuthority()
    {
        playerEquipment = GetComponentInParent<PlayerEquipment>();
    }

    // ---- ON STOP ----
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
    #endregion

    #region Serialization
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if (!initialState)
        {
            writer.WriteGameObject(EquipmentItem);
            print((int)EquipmentType);
            writer.WriteInt32((int)EquipmentType);
            return true;
        }
        return false;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (!initialState)
        {
            EquipmentItem = reader.ReadGameObject() ?? null;
            EquipmentType = (EquipmentType)reader.ReadInt32();
            print(EquipmentType);
            return;
        }
    }
    #endregion

    #region Late Joiner Synchronization
    [Server]
    private void Svr_UpdateVars(NetworkConnection conn)
    {
        Rpc_UpdateItemObject(conn, EquipmentItem);
        Rpc_UpdateItemType(conn, EquipmentType);
    }

    [TargetRpc]
    private void Rpc_UpdateItemObject(NetworkConnection target, GameObject value)
    {
        EquipmentItem = value;
    }
    [TargetRpc]
    private void Rpc_UpdateItemType(NetworkConnection target, EquipmentType value)
    {
        EquipmentType = value;
    }
    #endregion

    [Server]
    public void Svr_AssignItem(GameObject equipment)
    {
        EquipmentItem = equipment;
    }
    // This method is strict, so it won't equip the item if the type doesn't match this slot.
    // Returns true if item is equipped, false if the item is not equipped.
    [Server]
    public bool Svr_AssignItemStrict(GameObject equipment, EquipmentType equipmentType)
    {
        if (equipmentType == EquipmentType)
        {
            EquipmentItem = equipment;
            return true;
        }
        return false;
    }

    [Server]
    public void Svr_UnassignItem()
    {
        EquipmentItem = null;
    }

    [TargetRpc]
    public void Rpc_Select(NetworkConnection conn)
    {
        if (slotImage)
            slotImage.color = selectedColor;
        UISlot.GetComponent<RectTransform>().sizeDelta += new Vector2(10f, 10f);
    }
    [TargetRpc]
    public void Rpc_Deselect(NetworkConnection conn)
    {
        if (slotImage)
            slotImage.color = deselectedColor;
        UISlot.GetComponent<RectTransform>().sizeDelta -= new Vector2(10f, 10f);
    }
    [TargetRpc]
    private void Rpc_UpdateUI(NetworkConnection target, GameObject value)
    {
        if (value != null)
        {
            hotbarSlot.SetItemName(value.name);
            hotbarSlot.SetItemImage(value.GetComponent<EquipmentItem>().UISilhouette);
            hotbarSlot.ToggleItemImage();
        }
        else
        {
            hotbarSlot.SetItemName("─");
            hotbarSlot.ToggleItemImage();
        }
    }
}