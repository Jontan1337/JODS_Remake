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
    private GameObject itemInHands;
    [SerializeField]
    private EquipmentItem equipmentItem;
    [SerializeField, SyncVar]
    private EquipmentSlot selectedEquipmentSlot;
    // Maybe change to SyncList?
    [SerializeField]
    private List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();

    [Header("Player Preferences")]
    [SerializeField]
    private ItemPickupBehaviour itemPickupBehaviour = ItemPickupBehaviour.EquipAny;

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
    private PlayerSetup playerSetup;

    private const string slotsUIParentName = "CanvasInGame/Hotbar";

    #region ServerOnly Fields

    public Action<GameObject, GameObject> onServerEquippedItemChange;
    private Action<GameObject> onServerItemPickedUp;

    #endregion

    #region ClientOnly Fields

    private Action<GameObject> onClientItemPickedUp; // Not used remove perhaps?

    #endregion

    public GameObject ItemInHands
    {
        get => itemInHands;
        private set
        {
            if (!isServer) return;

            GameObject oldEquippedItem = null;
            if (itemInHands)
            {
                oldEquippedItem = itemInHands;
            }
            itemInHands = value;
            if (itemInHands)
            {
                EquipmentItem = itemInHands.GetComponent<EquipmentItem>();
            }
            else
            {
                EquipmentItem = null;
            }
            // Tell EquipmentControl to unbind old item and bind new item.
            Svr_InvokeEquippedItemChange(oldEquippedItem, itemInHands);
        }
    }
    public EquipmentItem EquipmentItem
    {
        get => equipmentItem;
        set
        {
            if (!isServer) return;

            EquipmentItem oldEquipmentItem = equipmentItem;
            if (oldEquipmentItem)
            {
                oldEquipmentItem.onServerDropItem -= Svr_OnItemDropped;
                oldEquipmentItem.Svr_Unequip();
            }
            equipmentItem = value;
            if (equipmentItem)
            {
                equipmentItem.Svr_Equip();
                equipmentItem.onServerDropItem += Svr_OnItemDropped;
            }
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
                selectedEquipmentSlot.onServerItemChange -= Svr_SelectedSlotItemChange;
            }
            // Listen if the new selected slot's item changes.
            value.onServerItemChange += Svr_SelectedSlotItemChange;

            selectedEquipmentSlot = value;
            ItemInHands = selectedEquipmentSlot.EquipmentItem; // NO: Causes weird command when no authority. No understand hlep. but work no problem.
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

    public void Init(PlayerSetup initializer)
    {
        if (IsInitialized) return;

        playerSetup = initializer;

        equipmentSlotsTypes = playerSetup.equipmentSlotsTypes;
        if (isServer)
        {
            onServerItemPickedUp += Svr_OnItemPickedUp;
        }
        if (hasAuthority)
        {
            Cmd_EquipmentSlotsSetup();
            // This is run in Start instead of OnStartServer because
            // OnStartServer gets called before Start when equipment object
            // is not set as child to player yet.
            initializer.onSpawnItem += GetReferences;
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
        JODSInput.Controls.Survivor.Hotbarselecting.performed -= number => Cmd_SelectSlot(Mathf.RoundToInt(number.ReadValue<float>()) - 1);
    }
    #endregion

    #region Serialization
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if (!initialState)
        {
            writer.WriteGameObject(itemInHands);
            writer.WriteEquipmentSlot(selectedEquipmentSlot);
            return true;
        }
        return false;

    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (!initialState)
        {
            itemInHands = reader.ReadGameObject();
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

        if (ItemInHands)
            Rpc_UpdateEquippedItem(conn, ItemInHands);

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
        ItemInHands = value;
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
        // If selected equipment hotbar slot is empty, equip item in that hotbar slot,
        // else look for an available hotbar slot.
        if (SelectedEquipmentSlot.EquipmentType != equipmentType || SelectedEquipmentSlot.EquipmentItem != null)
        {
            EquipmentSlot newSelectedSlot = Svr_GetSlot(equipmentType);
            if (newSelectedSlot != null)
            {
                // If newSelectedSlot is not null, then select it.
                // If newSelectedSlot is null, then use the one that's already selected.
                switch (itemPickupBehaviour)
                {
                    case ItemPickupBehaviour.EquipAny:
                        Svr_SelectSlot(equipmentSlots.IndexOf(newSelectedSlot));
                        break;
                    case ItemPickupBehaviour.EquipWeapon:
                        if (equipmentType == EquipmentType.Weapon)
                        {
                            Svr_SelectSlot(equipmentSlots.IndexOf(newSelectedSlot));
                        }
                        break;
                    case ItemPickupBehaviour.EquipNone:
                        Svr_ReplaceSlotItem(newSelectedSlot, equipment);
                        MoveToHands(equipment);
                        return;
                    default:
                        break;
                }
            }
        }


        if (SelectedEquipmentSlot.EquipmentType == equipmentType)
        {
            if (SelectedEquipmentSlot.EquipmentItem)
            {
                Svr_RemoveItem(SelectedEquipmentSlot.EquipmentItem);
            }
            Svr_EquipItem(equipment);
        }

        // EquipmentType none is meant for equipment
        // that doesn't fit any of the slots.
        if (equipmentType == EquipmentType.None)
        {
            if (equipment.TryGetComponent(out IEquippable equippable))
            {
                if (ItemInHands)
                {
                    equipmentItem.Svr_Unequip();
                }
                ItemInHands = equipment;
            }
        }
        Svr_InvokeItemPickedUp(ItemInHands);
        //Rpc_InvokeItemPickedUp(EquippedItem);
    }

    [Server]
    private EquipmentSlot Svr_GetSlot(EquipmentType equipmentType)
    {
        // Try to get empty slot of same type.
        EquipmentSlot tempAvailableSlot = Svr_GetAvailableSlot(equipmentType);
        if (tempAvailableSlot != null)
        {
            return tempAvailableSlot;
            //Svr_SelectSlot(equipmentSlots.IndexOf(tempAvailableSlot));
        }
        // If no empty slot of same type was found.
        // Try to get first slot of same type.
        if (selectedEquipmentSlot.EquipmentType != equipmentType)
        {
            EquipmentSlot tempSlotOfType = Svr_GetFirstSlotOfType(equipmentType);
            if (tempSlotOfType != null)
            {
                return tempSlotOfType;
                //Svr_SelectSlot(equipmentSlots.IndexOf(tempSlotOfType));
            }
        }
        return null;
    }

    // Finds and returns the first bar that has no equipment.
    // If none are available, it returns the currently selected bar.
    // Returns null if no matching and empty slot is available.
    [Server]
    private EquipmentSlot Svr_GetAvailableSlot(EquipmentType equipmentType)
    {
        for (int i = 0; i < equipmentSlotsCount; i++)
        {
            EquipmentSlot currentSlot = EquipmentSlots[i];
            if (currentSlot.EquipmentItem == null && currentSlot.EquipmentType == equipmentType)
                return currentSlot;
        }
        return null;
    }
    // Returns the first slot of the same type even if that slot already has an item.
    // Returns null if no matching slot type was found.
    [Server]
    private EquipmentSlot Svr_GetFirstSlotOfType(EquipmentType equipmentType)
    {
        for (int i = 0; i < equipmentSlotsCount; i++)
        {
            EquipmentSlot currentSlot = EquipmentSlots[i];
            if (currentSlot.EquipmentType == equipmentType)
                return currentSlot;
        }
        return null;
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
    [Server]
    private void Svr_OnItemPickedUp(GameObject newItem)
    {
        print("Svr_OnItemPickedUp");
        OnItemPickedUp(newItem);
        Rpc_OnItemPickedUp(newItem);
    }
    [ClientRpc]
    private void Rpc_OnItemPickedUp(GameObject newItem)
    {
        if (isServer) return;
        OnItemPickedUp(newItem);
    }
    private void OnItemPickedUp(GameObject newItem)
    {
        COMoveToHands = StartCoroutine(SmoothMoveToHands(newItem));
    }

    [Server]
    private void Svr_OnItemDropped(GameObject item)
    {
        OnItemDropped(item);
        Rpc_OnItemDropped(item);
        Svr_ClearHotbarSlot();
    }
    [ClientRpc]
    private void Rpc_OnItemDropped(GameObject item)
    {
        if (isServer) return;
        OnItemDropped(item);
    }
    private void OnItemDropped(GameObject item)
    {
        if (COMoveToHands != null)
        {
            StopCoroutine(COMoveToHands);
            COMoveToHands = null;
        }
    }

    [Server]
    private void Svr_ReplaceSlotItem(EquipmentSlot equipmentSlot, GameObject newItem)
    {
        // Check if selected equipmentslot already has an item.
        if (equipmentSlot.EquipmentItem)
        {
            equipmentSlot.EquipmentItem.TryGetComponent(out EquipmentItem equipmentSlotsEquipmentItem);
            // Drop the previous item.
            equipmentSlotsEquipmentItem.Svr_Drop();
            // Remove the item in the equipmentslot.
            equipmentSlot.Svr_RemoveItem();
        }
        // Equip the new item in the equipment slot.
        equipmentSlot.Svr_EquipItem(newItem);
        newItem.TryGetComponent(out EquipmentItem newEquipmentItem);
        // Assign the new item to this player and parent to playerhands.
        newEquipmentItem?.Svr_Pickup(playerHands, connectionToClient);
        // Hide the new item since we're not using it, only equipping it.
        newEquipmentItem?.Svr_HideItem();
    }

    [Server]
    private void Svr_SelectedSlotItemChange(GameObject oldItem, GameObject newItem)
    {
        ItemInHands = newItem;
    }

    [Command]
    public void Cmd_SelectSlot(int slotIndex)
    {
        Svr_SelectSlot(slotIndex);
    }
    [Server]
    public void Svr_SelectSlot(int slotIndex)
    {
        if (slotIndex + 1 <= equipmentSlotsCount && slotIndex + 1 >= 0)
        {
            if (equipmentSlots[slotIndex] != null)
            {
                SelectedEquipmentSlot?.Rpc_Deselect(connectionToClient);
                SelectedEquipmentSlot = equipmentSlots[slotIndex];
                SelectedEquipmentSlot.Rpc_Select(connectionToClient);
            }
        }
    }

    private IEnumerator SmoothMoveToHands(GameObject newItem)
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
    private void MoveToHands(GameObject newItem)
    {
        newItem.transform.position = playerHands.position;
        newItem.transform.rotation = playerHands.rotation;
    }

    [Server]
    private void Svr_EquipItem(GameObject item)
    {
        if (item.TryGetComponent(out EquipmentItem equipmentItem))
        {
            equipmentItem.Svr_Pickup(playerHands, connectionToClient);
        }
        SelectedEquipmentSlot.Svr_EquipItem(item);
    }
    [Command]
    private void Cmd_RemoveItem(GameObject item)
    {
        Svr_RemoveItem(item);
    }
    [Server]
    private void Svr_RemoveItem(GameObject item)
    {
        if (item)
        {
            EquipmentItem.Svr_Drop();
            EquipmentItem = null;
            if (item == SelectedEquipmentSlot.EquipmentItem)
            {
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

            item.EquipmentItem.GetComponent<EquipmentItem>().Svr_Drop();
            item.EquipmentItem.transform.parent = null;
            Svr_OnItemDropped(item.EquipmentItem);
            item.Svr_RemoveItem();
        }
    }

    [Server]
    private void Svr_ClearHotbarSlot()
    {
        SelectedEquipmentSlot.Svr_RemoveItem();
    }

    [Command]
    private void Cmd_EquipmentSlotsSetup()
    {
        Svr_EquipmentSlotsSetup();
    }

    [Server]
    private void Svr_EquipmentSlotsSetup()
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