using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System;

public class EquipmentSlot : NetworkBehaviour
{
    public string slotName;
    public Action<GameObject, GameObject> onItemChange;

    [Header("Item info")]
    [SerializeField, SyncVar(hook = nameof(OnEquipmentChanged))]
    private GameObject equipmentItem;
    [SerializeField, SyncVar]
    private EquipmentType equipmentType;

    [Header("Item index")]
    [SerializeField]
    private KeyCode keyCode;
    [SerializeField]
    private int keyNumber;

    [Header("Hotbar Slot Visuals")]
    [SerializeField]
    private GameObject uiSlot;
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color deselectedColor;
    [SerializeField]
    private TMP_Text textItemName;
    [SerializeField]
    private TMP_Text textItemType;
    [SerializeField]
    private Image slotImage;

    private Equipment playerEquipment;
    [SyncVar, SerializeField] private bool hasPhysics;

    public GameObject UISlot
    {
        get => uiSlot;
        set
        {
            uiSlot = value;
            slotImage = UISlot.GetComponent<Image>();
            textItemName = UISlot.GetComponentsInChildren<TMP_Text>()[0];
            TextItemType = UISlot.GetComponentsInChildren<TMP_Text>()[1];
        }
    }

    #region Properties
    public GameObject EquipmentItem
    {
        get => equipmentItem;
        private set
        {
            GameObject oldItem = equipmentItem;

            if (equipmentItem)
                equipmentItem.GetComponent<IEquippable>()?.Svr_RemoveAuthority();

            equipmentItem = value;
            onItemChange?.Invoke(oldItem, equipmentItem);
        }
    }
    public EquipmentType EquipmentType { get => equipmentType; set => equipmentType = value; }
    public KeyCode KeyCode { get => keyCode; private set => keyCode = value; }
    public TMP_Text TextItemType {
        get => textItemType;
        private set
        {
            textItemType = value;
            textItemType.text = EquipmentType.ToString();
        }
    }
    #endregion

    #region Hooks
    private void OnEquipmentChanged(GameObject oldVal, GameObject newVal)
    {
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
        NetworkTest.RelayOnServerAddPlayer += Svr_UpdateVars;
    }
    public override void OnStartAuthority()
    {
        playerEquipment = GetComponentInParent<Equipment>();
    }

    // ---- ON STOP ----
    public override void OnStopServer()
    {
        NetworkTest.RelayOnServerAddPlayer -= Svr_UpdateVars;
    }
    #endregion

    #region Serialization
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if (!initialState)
        {
            writer.WriteGameObject(equipmentItem);
            writer.Write(equipmentType);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (!initialState)
        {
            equipmentItem = reader.ReadGameObject();
            equipmentType = reader.Read<EquipmentType>();
        }
    }
    #endregion

    #region Late Joiner Synchronization
    [Server]
    private void Svr_UpdateVars(NetworkConnection conn)
    {
        Rpc_UpdateItemObject(conn, EquipmentItem);
        Rpc_UpdateItemType(conn, EquipmentType);
        Rpc_UpdateItemPhysics(conn,
            EquipmentItem.GetComponent<Rigidbody>().isKinematic,
            EquipmentItem.GetComponent<Collider>().enabled
        );
    }

    [TargetRpc]
    private void Rpc_UpdateItemObject(NetworkConnection target, GameObject value)
    {
        Debug.Log($"Received {value}");
        EquipmentItem = value;
    }
    [TargetRpc]
    private void Rpc_UpdateItemType(NetworkConnection target, EquipmentType value)
    {
        Debug.Log($"Received {value}");
        EquipmentType = value;
    }
    #endregion

    [Server]
    public bool Svr_Equip(GameObject equipment, EquipmentType equipmentType)
    {
        if (equipmentType == EquipmentType)
        {
            EquipmentItem = equipment;
            if (EquipmentItem)
            {
                Svr_DisableItemPhysics();
            }
            return true;
        }
        return false;
    }

    [Server]
    public bool Svr_Drop(Transform dropTransform)
    {
        if (EquipmentItem != null)
        {
            Svr_ShowItem();
            Svr_EnableItemPhysics();
            EquipmentItem.GetComponent<IInteractable>().IsInteractable = true;
            EquipmentItem.transform.parent = null;
            EquipmentItem = null;
            return true;
        }
        return false;
    }

    [Server]
    private void Svr_ShowItem()
    {
        EquipmentItem.SetActive(true);
    }
    [Server]
    private void Svr_HideItem()
    {
        EquipmentItem.SetActive(false);
    }

    #region Toggle physics
    [Server]
    private void Svr_EnableItemPhysics()
    {
        hasPhysics = true;
        EquipmentItem.GetComponent<Rigidbody>().isKinematic = false;
        EquipmentItem.GetComponent<Collider>().enabled = true;
        Rpc_EnableItemPhysics();
    }
    [Server]
    private void Svr_DisableItemPhysics()
    {
        hasPhysics = false;
        EquipmentItem.GetComponent<Rigidbody>().isKinematic = true;
        EquipmentItem.GetComponent<Collider>().enabled = false;
        Rpc_DisableItemPhysics();
    }
    [ClientRpc]
    private void Rpc_EnableItemPhysics()
    {
        EquipmentItem.GetComponent<Rigidbody>().isKinematic = false;
        EquipmentItem.GetComponent<Collider>().enabled = true;
    }
    [ClientRpc]
    private void Rpc_DisableItemPhysics()
    {
        EquipmentItem.GetComponent<Rigidbody>().isKinematic = true;
        EquipmentItem.GetComponent<Collider>().enabled = false;
    }
    [TargetRpc]
    private void Rpc_UpdateItemPhysics(NetworkConnection target, bool isKinematic, bool hasCollider)
    {
        EquipmentItem.GetComponent<Rigidbody>().isKinematic = false;
        EquipmentItem.GetComponent<Collider>().enabled = true;
    }
    #endregion

    [TargetRpc]
    public void Rpc_Select(NetworkConnection conn)
    {
        if (EquipmentItem)
            Svr_ShowItem();

        if (slotImage)
            slotImage.color = selectedColor;
    }
    [TargetRpc]
    public void Rpc_Deselect(NetworkConnection conn)
    {
        if (EquipmentItem)
            Svr_HideItem();

        if (slotImage)
            slotImage.color = deselectedColor;
    }
    [TargetRpc]
    private void Rpc_UpdateUI(NetworkConnection target, GameObject value)
    {
        if (value != null)
        {
            textItemName.SetText(value.name);
        }
        else
        {
            textItemName.SetText("Empty");
        }
    }
}