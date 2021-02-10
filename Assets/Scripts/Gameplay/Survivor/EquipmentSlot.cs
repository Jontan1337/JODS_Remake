using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class EquipmentSlot : NetworkBehaviour
{
    public string slotName;

    [Header("Item info")]
    [SerializeField, SyncVar(hook = nameof(OnEquipmentChanged))]
    private GameObject equipmentItem;
    private void OnEquipmentChanged(GameObject oldVal, GameObject newVal)
    {
        Rpc_UpdateUI(connectionToClient, newVal);
    }
    [SerializeField]
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
    private TMP_Text textMesh;
    [SerializeField]
    private Image slotImage;

    private Equipment playerEquipment;

    public GameObject UISlot
    {
        get => uiSlot;
        set
        {
            uiSlot = value;
            slotImage = UISlot.GetComponent<Image>();
            textMesh = UISlot.GetComponentInChildren<TMP_Text>();
        }
    }

    #region Properties
    public GameObject EquipmentItem
    {
        get => equipmentItem;
        private set
        {
            equipmentItem = value;
        }
    }
    public EquipmentType EquipmentType { get => equipmentType; set => equipmentType = value; }
    public KeyCode KeyCode { get => keyCode; private set => keyCode = value; }
    //public int KeyNumber { get => keyNumber; private set => keyNumber = value; }
    //public Color SelectedColor { get => selectedColor; private set => selectedColor = value; }
    //public Color DeselectedColor { get => deselectedColor; private set => deselectedColor = value; }
    //public TMP_Text TextMesh { get => textMesh; private set => textMesh = value; }
    //public Image SlotImage { get => slotImage; private set => slotImage = value; }
    #endregion

    private void Awake()
    {
        keyNumber = transform.GetSiblingIndex() + 1;
        keyCode = KeyCode.Alpha0 + keyNumber;
    }

    public override void OnStartServer()
    {
        Debug.Log("Subscribed to RelayOnServerAddPlayer", this);
        NetworkTest.RelayOnServerAddPlayer += Svr_UpdateVars;
        if (isServer)
        {
        }
    }
    public override void OnStartAuthority()
    {
        playerEquipment = GetComponentInParent<Equipment>();
    }

    #region Serialization
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if (!initialState)
        {
            writer.WriteGameObject(equipmentItem);
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
        }
    }

    #endregion

    #region Late Joiner Synchronization
    [Server]
    private void Svr_UpdateVars(NetworkConnection conn)
    {
        Rpc_UpdateItemObject(conn, EquipmentItem);
    }

    [TargetRpc]
    private void Rpc_UpdateItemObject(NetworkConnection target, GameObject value)
    {
        Debug.Log($"Received {value}");
        EquipmentItem = value;
    }
    #endregion

    [Server]
    public bool Svr_Equip(GameObject equipment, EquipmentType equipmentType)
    {
        if (equipmentType == EquipmentType)
        {
            EquipmentItem = equipment;
            return true;
        }
        return false;
    }

    [Server]
    public bool Svr_Drop(Transform dropTransform)
    {
        if (EquipmentItem != null)
        {
            Debug.Log("Drop", this);
            EquipmentItem = null;
            return true;
        }
        return false;
    }

    [TargetRpc]
    public void Rpc_Select(NetworkConnection conn)
    {
        if (slotImage)
            slotImage.color = selectedColor;
    }
    [TargetRpc]
    public void Rpc_Deselect(NetworkConnection conn)
    {
        if (slotImage)
            slotImage.color = deselectedColor;
    }
    [TargetRpc]
    private void Rpc_UpdateUI(NetworkConnection target, GameObject value)
    {
        if (value != null)
        {
            textMesh.SetText(value.name);
        }
        else
        {
            textMesh.SetText("Empty");
        }
    }
}