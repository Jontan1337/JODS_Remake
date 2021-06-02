using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.InputSystem;

public class PlayerEquipment : NetworkBehaviour, IInitializable<PlayerSetup>
{
    [Tooltip("A list of the equipment types, the player should have.")]
    public List<EquipmentType> equipmentSlotsTypes = new List<EquipmentType>();
    [Space]
    public Transform playerHands;

    [SerializeField, SyncVar]
    private GameObject equippedItem;
    [SerializeField, SyncVar]
    private EquipmentSlot selectedEquipmentSlot;
    [SerializeField]
    private List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();

    [Header("Equipment slot setup settings")]
    [SerializeField, Tooltip("The parent transform, where the equipment slots should be instantiated.")]
    private Transform equipmentSlotsParent;
    [SerializeField, Tooltip("The parent transform, where the UI equipment slots should be instantiated.")]
    private Transform equipmentSlotsUIParent;
    [SerializeField, Tooltip("The prefab of the equipment slots.")]
    private GameObject equipmentSlotPrefab;
    [SerializeField, Tooltip("The prefab of the UI equipment slots.")]
    private GameObject equipmentSlotUIPrefab;

    private int equipmentSlotsCount = 0;
    private Coroutine COMoveToHands;

    private const string slotsUIParentName = "CanvasInGame/Hotbar";

    #region ServerOnly Fields

    public Action<GameObject, GameObject> onServerEquippedItemChange;
    private Action<GameObject> onServerItemPickedUp;

    #endregion

    #region ClientOnly Fields

    private Action<GameObject> onClientItemPickedUp;

    #endregion

    public GameObject EquippedItem
    {
        get => equippedItem;
        private set
        {
            if (!isServer) return;

            GameObject oldEquippedItem = null;
            if (equippedItem)
            {
                oldEquippedItem = equippedItem;
            }
            equippedItem = value;
            if (equippedItem)
            {
                Svr_ShowItem(equippedItem);
            }
            Svr_InvokeEquippedItemChange(oldEquippedItem, equippedItem);
        }
    }

    public EquipmentSlot SelectedEquipmentSlot
    {
        get => selectedEquipmentSlot;
        private set
        {
            if (!isServer) return;

            // Stop listening if the old selected slot's item changes.
            if (selectedEquipmentSlot)
            {
                selectedEquipmentSlot.onServerItemChange -= (GameObject oldItem, GameObject newItem) => Svr_SelectedSlotItemChange(newItem);
                if (EquippedItem)
                {
                    Svr_HideItem(EquippedItem);
                }
            }
            // Listen if the new selected slot's item changes.
            value.onServerItemChange += (GameObject oldItem, GameObject newItem) => Svr_SelectedSlotItemChange(newItem);

            selectedEquipmentSlot = value;
            EquippedItem = selectedEquipmentSlot.EquipmentItem;
        }
    }
    public List<EquipmentSlot> EquipmentSlots
    {
        get => equipmentSlots;
        private set
        {
            equipmentSlots = value;
        }
    }
    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        if (!isServer)
        {
            onClientItemPickedUp += OnItemPickedUp;
        }
        if (isServer)
        {
            onServerItemPickedUp += Svr_OnItemPickedUp;
        }
    }

    public void Init(PlayerSetup initializer)
    {
        if (IsInitialized) return;

        if (hasAuthority)
        {
            Cmd_EquipmentSlotsSetup();

            // This is run in Start instead of OnStartServer because
            // OnStartServer gets called before Start when equipment object
            // is not set as child to player yet.
            initializer.onSpawnItem += GetReferences;
        }
        if (isServer)
        {
        }

        IsInitialized = true;
    }

    private void OnTransformParentChanged()
    {
        //if (hasAuthority)
        {
            // When parent changed, check if it's correctly set to player Survivor
            // and then find UI hotbar under Survivors canvas.
            Debug.Log($"My parent {transform.parent}", this);

            if (transform.parent.name.Contains("Survivor"))
            {
                equipmentSlotsUIParent = transform.parent.Find(slotsUIParentName);
                equipmentSlotsTypes = transform.root.GetComponent<PlayerSetup>().equipmentSlotsTypes;
            }
        }
    }

    [TargetRpc]
    private void Rpc_UpdateEquipmentSlots(NetworkConnection conn, EquipmentSlot newEquipment, int value)
    {
        if (value == 1)
        {
            equipmentSlots.Add(newEquipment);
        }
        else if (value == -1)
        {
            equipmentSlots.Remove(newEquipment);
        }
    }

    #region NetworkBehaviour Callbacks
    public override void OnStartServer()
    {
        NetworkTest.RelayOnServerAddPlayer += Svr_UpdateVars;
    }

    public override void OnStartAuthority()
    {
        JODSInput.Controls.Survivor.Drop.performed += OnDropItem;
        JODSInput.Controls.Survivor.Hotbarselecting.performed += number => Cmd_SelectSlot(Mathf.RoundToInt(number.ReadValue<float>()) - 1);
        
    }
    public override void OnStartClient()
    {
        equipmentSlotsParent = transform;
    }

    public override void OnStopServer()
    {
        NetworkTest.RelayOnServerAddPlayer -= Svr_UpdateVars;
    }
    public override void OnStopAuthority()
    {
        transform.root.GetComponent<PlayerSetup>().onSpawnItem -= GetReferences;
        JODSInput.Controls.Survivor.Drop.performed -= OnDropItem;
        JODSInput.Controls.Survivor.Hotbarselecting.performed -= number => Cmd_SelectSlot(Mathf.RoundToInt(number.ReadValue<float>()) - 1);
    }
    #endregion

    #region Serialization
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if (!initialState)
        {
            writer.WriteGameObject(equippedItem);
            writer.WriteEquipmentSlot(selectedEquipmentSlot);
            return true;
        }
        return false;

    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (!initialState)
        {
            equippedItem = reader.ReadGameObject();
            selectedEquipmentSlot = reader.ReadEquipmentSlot();
        }
    }
    #endregion

    #region Late Joiner Synchronization
    [Server]
    private void Svr_UpdateVars(NetworkConnection conn)
    {
        if (SelectedEquipmentSlot)
            Rpc_UpdateSelectedSlot(conn, SelectedEquipmentSlot);

        if (EquippedItem)
            Rpc_UpdateEquippedItem(conn, EquippedItem);

        if (playerHands)
            Rpc_UpdatePlayerHands(conn, playerHands);
    }

    [TargetRpc]
    private void Rpc_UpdateSelectedSlot(NetworkConnection target, EquipmentSlot value)
    {
        SelectedEquipmentSlot = value;
    }
    [TargetRpc]
    private void Rpc_UpdateEquippedItem(NetworkConnection target, GameObject value)
    {
        EquippedItem = value;
    }
    [TargetRpc]
    private void Rpc_UpdatePlayerHands(NetworkConnection target, Transform value)
    {
        playerHands = value;
    }
    #endregion

    [Server]
    public void Svr_Equip(GameObject equipment, EquipmentType equipmentType)
    {
        if (SelectedEquipmentSlot.EquipmentType != equipmentType || SelectedEquipmentSlot.EquipmentItem != null)
        {
            Svr_SelectSlotOfType(equipmentType);
        }

        // If selected equipment bar is empty, equip item in that bar,
        // else look for an available bar.
        if (SelectedEquipmentSlot.EquipmentType == equipmentType)
        {
            Svr_DropItem(SelectedEquipmentSlot.EquipmentItem);
            SelectedEquipmentSlot.Svr_EquipItem(equipment, equipmentType);
        }

        // EquipmentType none is meant for equipment
        // that doesn't fit any of the slots.
        if (equipmentType == EquipmentType.None)
        {
            if (equipment.TryGetComponent(out IEquippable equippable))
            {
                if (EquippedItem)
                {
                    Svr_HideItem(EquippedItem);
                }
                EquippedItem = equipment;
            }
        }
        Svr_PlaceItemInHands();
        Svr_InvokeItemPickedUp(EquippedItem);
        Rpc_InvokeItemPickedUp(EquippedItem);
    }

    [Server]
    private void Svr_SelectSlotOfType(EquipmentType equipmentType)
    {
        Svr_SelectSlot(equipmentSlots.IndexOf(Svr_GetAvailableSlot(equipmentType)));
        if (selectedEquipmentSlot.EquipmentType != equipmentType)
        {
            Svr_SelectSlot(equipmentSlots.IndexOf(Svr_GetFirstSlotOfType(equipmentType)));
        }
    }

    // Finds and returns the first bar that has no equipment.
    // If none are available, it returns the currently selected bar.
    [Server]
    private EquipmentSlot Svr_GetAvailableSlot(EquipmentType equipmentType)
    {
        for (int i = 0; i < equipmentSlotsCount; i++)
        {
            EquipmentSlot currentSlot = EquipmentSlots[i];
            if (currentSlot.EquipmentItem == null && currentSlot.EquipmentType == equipmentType)
                return currentSlot;
        }
        return SelectedEquipmentSlot;
    }
    [Server]
    private EquipmentSlot Svr_GetFirstSlotOfType(EquipmentType equipmentType)
    {
        for (int i = 0; i < equipmentSlotsCount; i++)
        {
            EquipmentSlot currentSlot = EquipmentSlots[i];
            if (currentSlot.EquipmentType == equipmentType)
                return currentSlot;
        }
        return SelectedEquipmentSlot;
    }

    [Server]
    private void Svr_InvokeEquippedItemChange(GameObject oldItem, GameObject newItem)
    {
        onServerEquippedItemChange?.Invoke(oldItem, newItem);
    }
    [Server]
    private void Svr_InvokeItemPickedUp(GameObject newItem)
    {
        onServerItemPickedUp?.Invoke(newItem);
    }
    [ClientRpc]
    private void Rpc_InvokeItemPickedUp(GameObject newItem)
    {
        onClientItemPickedUp?.Invoke(newItem);
    }

    [Server]
    private void Svr_OnItemPickedUp(GameObject newItem)
    {
        COMoveToHands = StartCoroutine(MoveToHands(newItem));
    }
    private void OnItemPickedUp(GameObject newItem)
    {
        COMoveToHands = StartCoroutine(MoveToHands(newItem));
    }
    [Server]
    private void Svr_OnItemDropped(GameObject newItem)
    {
        StopCoroutine(COMoveToHands);
    }
    private void OnItemDropped(GameObject newItem)
    {
        StopCoroutine(COMoveToHands);
    }

    [Server]
    private void Svr_SelectedSlotItemChange(GameObject newItem)
    {
        EquippedItem = newItem;
    }

    [Command]
    public void Cmd_SelectSlot(int slotIndex)
    {
        Svr_SelectSlot(slotIndex);
    }
    [Server]
    public void Svr_SelectSlot(int slotIndex)
    {
        if (slotIndex+1 <= equipmentSlotsCount && slotIndex+1 >= 0)
        {
            if (equipmentSlots[slotIndex] != null)
            {
                SelectedEquipmentSlot?.Rpc_Deselect(connectionToClient);
                SelectedEquipmentSlot = equipmentSlots[slotIndex];
                SelectedEquipmentSlot.Rpc_Select(connectionToClient);
            }
        }
    }

    [Server]
    private void Svr_PlaceItemInHands()
    {
        if (EquippedItem.TryGetComponent(out PhysicsToggler pt))
        {
            pt.Svr_DisableItemPhysics();
        }
        EquippedItem.transform.parent = playerHands;
    }

    private IEnumerator MoveToHands(GameObject newItem)
    {
        while (!Equals(newItem.transform.position, playerHands.position)
                && !Equals(newItem.transform.rotation, playerHands.rotation))
        {
            newItem.transform.position = Vector3.Lerp(newItem.transform.position, playerHands.position, Time.deltaTime * 20);
            newItem.transform.rotation = Quaternion.Lerp(newItem.transform.rotation, playerHands.rotation, Time.deltaTime * 20);

            yield return null;

            if (Vector3.Distance(newItem.transform.position, playerHands.position) < 0.08f
                && Vector3.Distance(newItem.transform.eulerAngles, playerHands.eulerAngles) < 0.08f)
            {
                newItem.transform.position = playerHands.position;
                newItem.transform.rotation = playerHands.rotation;
                yield break;
            }
        }
    }

    [Command]
    private void Cmd_ShowItem(GameObject item)
    {
        Svr_ShowItem(item);
    }
    [Command]
    private void Cmd_HideItem(GameObject item)
    {
        Svr_HideItem(item);
    }
    [Server]
    private void Svr_ShowItem(GameObject item)
    {
        item.GetComponent<Renderer>().enabled = true;
        Rpc_ShowItem(item);
    }
    [Server]
    private void Svr_HideItem(GameObject item)
    {
        item.GetComponent<Renderer>().enabled = false;
        Rpc_HideItem(item);
    }
    [ClientRpc]
    private void Rpc_ShowItem(GameObject item)
    {
        item.GetComponent<Renderer>().enabled = true;
    }
    [ClientRpc]
    private void Rpc_HideItem(GameObject item)
    {
        item.GetComponent<Renderer>().enabled = false;
    }

    private void OnDropItem(InputAction.CallbackContext context)
    {
        Cmd_DropItem(EquippedItem);
        OnItemDropped(null);
    }
    [Command]
    private void Cmd_DropItem(GameObject item)
    {
        Svr_DropItem(item);
    }
    [Server]
    private void Svr_DropItem(GameObject item)
    {
        if (item)
        {
            Svr_ShowItem(item);
            if (item.TryGetComponent(out PhysicsToggler pt))
            {
                pt.Svr_EnableItemPhysics();
            }
            item.GetComponent<IInteractable>().IsInteractable = true;
            item.transform.parent = null;
            Svr_OnItemDropped(null);
            if (item == SelectedEquipmentSlot.EquipmentItem)
            {
                SelectedEquipmentSlot.Svr_RemoveItem();
            }
        }
    }
    [Server]
    public void Svr_DropAllItems()
    {
        print("Svr_DropAllItems");
        foreach (EquipmentSlot item in equipmentSlots)
        {
            if (item.EquipmentItem == null) continue;

            Svr_ShowItem(item.EquipmentItem);
            if (item.EquipmentItem.TryGetComponent(out PhysicsToggler pt))
            {
                pt.Svr_EnableItemPhysics();
            }
            item.EquipmentItem.GetComponent<IInteractable>().IsInteractable = true;
            item.EquipmentItem.transform.parent = null;
            Svr_OnItemDropped(null);
            item.Svr_RemoveItem();
        }
    }

    [Command]
    private void Cmd_EquipmentSlotsSetup()
    {
        foreach (EquipmentType type in equipmentSlotsTypes)
        {
            // Setup the networked prefab that holds the item.
            GameObject hotbarSlot = Instantiate(equipmentSlotPrefab);
            NetworkServer.Spawn(hotbarSlot, gameObject);
            hotbarSlot.transform.parent = equipmentSlotsParent;
            EquipmentSlot tempSlot = hotbarSlot.GetComponent<EquipmentSlot>();
            tempSlot.EquipmentType = type;

            // Setup the local UI prefab that shows the item slot.
            Rpc_CreateUISlots(connectionToClient, tempSlot);
            equipmentSlots.Add(tempSlot);
            if (!hasAuthority && isServer)
                Rpc_UpdateEquipmentSlots(connectionToClient, tempSlot, 1);
        }
        equipmentSlotsCount = EquipmentSlots.Count;
        Svr_SelectSlot(0);
    }

    // Setup local player UI hotbar.
    [TargetRpc]
    private void Rpc_CreateUISlots(NetworkConnection conn, EquipmentSlot tempSlot)
    {
        tempSlot.gameObject.transform.parent = equipmentSlotsParent;
        GameObject hotbarSlotUI = Instantiate(equipmentSlotUIPrefab, equipmentSlotsUIParent);
        tempSlot.UISlot = hotbarSlotUI;
    }

    private void GetReferences(GameObject item)
    {
        if (item.TryGetComponent(out ItemName itemName))
        {
            switch (itemName.itemName)
            {
                case ItemNames.PlayerHands:
                    playerHands = item.transform;
                    Cmd_SetPlayerHands(playerHands);
                    break;
                default:
                    break;
            }
        }
    }

    [Command]
    private void Cmd_SetPlayerHands(Transform playerHands)
    {
        this.playerHands = playerHands;
    }
}