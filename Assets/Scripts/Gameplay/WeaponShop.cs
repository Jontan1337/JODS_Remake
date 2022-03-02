﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class ShopItem
{
    public string shopItemName;
    public GameObject shopItemPrefab;
    public int shopItemPrice;
}
public class WeaponShop : NetworkBehaviour, IInteractable
{
    [Header("Shop Selection")]
    [SerializeField] private List<ShopItem> currentWeaponSelection = new List<ShopItem>();
    [SerializeField] private List<ShopItem> currentEquipmentSelection = new List<ShopItem>();
    [Space]
    [SerializeField] private int minimumAmountOfMeleeWeapons = 1;
    [SerializeField] private int minimumAmountOfHealingItems = 2;

    private int currentSelectionMeleeWeapons = 0;
    private int currentSelectionHealingItems = 0;

    [Header("Weapon database")]
    [SerializeField] private List<ShopItem> rangedWeapons = new List<ShopItem>();
    [Space]
    [SerializeField] private List<ShopItem> meleeWeapons = new List<ShopItem>();
    [Space]
    [SerializeField] private List<ShopItem> healingEquipment = new List<ShopItem>();
    [Space]
    [SerializeField] private List<ShopItem> equipment = new List<ShopItem>();

    private List<ShopItem> allShopItems = new List<ShopItem>();

    [Header("References")]
    [SerializeField] private UIShopButton[] weaponSlots = null;
    [SerializeField] private UIShopButton[] equipmentSlots = null;
    private UIShopButton[] allSlots = null;
    [Space]
    [SerializeField] private GameObject shopCanvas = null;
    [Space]
    [SerializeField] private GameObject weaponStats = null;
    [SerializeField] private Text weaponNameText = null;
    [SerializeField] private Text weaponDamageValueText = null;
    [SerializeField] private Text weaponDamageTypeValueText= null;
    [SerializeField] private Text weaponDamageTypeText= null;
    [Space]
    [SerializeField] private Transform topPartOfCrate = null;
    [SerializeField] private float topOpenValue = 330f;
    [Space]
    [SerializeField] private Text playerPointsText = null;
    [SerializeField, SyncVar] private bool isInteractable = true;

    [Header("Network Related")]
    private List<GameObject> playersInShop = new List<GameObject>();

    private GameObject playerGameObject = null;

    private string pointsTextDefault = "Your points: ";

    [Header("Debug")]
    [SerializeField] private bool test = false;

    public bool IsInteractable { get => isInteractable; set => isInteractable = value; }

    public string ObjectName => transform.name;

    private void Awake()
    {
        weaponStats.SetActive(false);
        shopCanvas.SetActive(false);

        //Combine both arrays of UIShopButtons into one
        allSlots = new UIShopButton[weaponSlots.Length + equipmentSlots.Length];
        weaponSlots.CopyTo(allSlots, 0);
        equipmentSlots.CopyTo(allSlots, weaponSlots.Length);
        //This Array is later used when buying items, to disable it for other players.

        //Combine all lists containing weapons into one list
        allShopItems.AddRange(rangedWeapons);
        allShopItems.AddRange(meleeWeapons);
        allShopItems.AddRange(healingEquipment);
        allShopItems.AddRange(equipment);
        //This is later used to tell the players which items have been generated by the server
    }

    private void Start()
    {
        if (!isServer) return;
        if (test)
        {
            StartCoroutine(GenerateNewSelectionDelay());
        }
    }

    //private void OnDestroy()
    //{
    //    if (playerGameObject != null)
    //    {
    //        CloseShop();
    //    }
    //}
    private void OnDisable()
    {
        if (playerGameObject != null)
        {
            PlayerManager.Instance.DisableMenu();
        }
    }

    public void InitializeShop() { }

    //THIS COROUTINE IS ONLY NECESSARY FOR TESTING PURPOSES. DELETE EVENTUALLY
    private IEnumerator GenerateNewSelectionDelay()
    {
        yield return new WaitForSeconds(0.2f);
        Svr_GenerateNewSelection();
    }

    [ClientRpc]
    private void Rpc_ShopVisuals(bool open)
    {
        if (shopTopVisualCo != null)
        {
            StopCoroutine(shopTopVisualCo);
        }
        shopTopVisualCo = StartCoroutine(ShopTopVisual(open));
    }

    private Coroutine shopTopVisualCo;
    private IEnumerator ShopTopVisual(bool open)
    {
        if (open)
        {
            while (topPartOfCrate.localEulerAngles.x > topOpenValue)
            {
                yield return null;
                topPartOfCrate.Rotate(-Time.deltaTime * 50, 0f, 0f);
            }
        }
        else
        {
            while (topPartOfCrate.localEulerAngles.x < 359f)
            {
                yield return null;
                topPartOfCrate.Rotate(Time.deltaTime * 50, 0f, 0f);
            }
        }
    }

    public void CloseShop()
    {
        Cmd_CloseShop(playerGameObject);
    }
    [Command(ignoreAuthority = true)]
    private void Cmd_CloseShop(GameObject interacter)
    {
        Svr_HandleUser(interacter);
        //PlayerManager.Instance.Rpc_DisableMenu(interacter.GetComponent<NetworkIdentity>().connectionToClient);
        Rpc_CloseShop(interacter.GetComponent<NetworkIdentity>().connectionToClient);
    }
    [TargetRpc]
    private void Rpc_CloseShop(NetworkConnection target)
    {
        print("PlayerManager DisableMenu");
        PlayerManager.Instance.DisableMenu();
    }

    [TargetRpc]
    private void Rpc_ShowPoints(NetworkConnection target, int points)
    {
        playerPointsText.text = pointsTextDefault + points;
    }

    [Server]
    public void Svr_Interact(GameObject interacter)
    {
        Svr_HandleUser(interacter);

        Rpc_Interact(interacter.GetComponent<NetworkIdentity>().connectionToClient, interacter);
    }

    [Server]
    private void Svr_HandleUser(GameObject interacter)
    {
        if (playersInShop.Contains(interacter))
        {
            playersInShop.Remove(interacter);
            if (playersInShop.Count == 0)
            {
                Rpc_ShopVisuals(false);
            }
        }
        else
        {
            if (playersInShop.Count == 0)
            {
                Rpc_ShopVisuals(true);
                if (!GamemodeBase.Instance) return;
                NetworkIdentity identity = interacter.GetComponent<NetworkIdentity>();
                Rpc_ShowPoints(identity.connectionToClient,
                GamemodeBase.Instance.Svr_GetPoints(identity.netId));
            }
            playersInShop.Add(interacter);
        }
    }

    [TargetRpc]
    public void Rpc_Interact(NetworkConnection target, GameObject interacter)
    {
        //Store the players gameobject
        //This is later used when a button is pressed to check which player pressed it.
        playerGameObject = interacter;
        //This could probably be done differently. Optimize later.

        InteractWithShop();
    }

    public void InteractWithShop()
    {
        PlayerManager.Instance.activeMenuCanvas = shopCanvas.transform;
        PlayerManager.Instance.EnableMenu();
        PlayerManager.Instance.onMenuClosed += OnMenuClosed;
    }

    public void OnMenuClosed()
    {
        Cmd_CloseShop(playerGameObject);
        PlayerManager.Instance.onMenuClosed -= OnMenuClosed;
    }

    private void SetShopSlotValues(List<ShopItem> shopItems, bool weaponSlotItems)
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            var item = shopItems[i];

            UIShopButton newSlot = weaponSlotItems ? weaponSlots[i] : equipmentSlots[i];
            int allSlotsIndex = GetSlotIndex(newSlot);
            int allShopItemsIndex = GetShopItemIndex(shopItems, i);

            //Assign the item to the slot
            newSlot.Item = item;

            Rpc_SetShopSlotValues(allShopItemsIndex, allSlotsIndex, weaponSlotItems);
        }
    }

    public int GetShopItemIndex(List<ShopItem> shopItems, int i)
    {
        //Get the index of the item, from the allShopItems array
        int allShopItemsIndex = 0;
        for (int x = 0; x < allShopItems.Count; x++)
        {
            if (allShopItems[x] == shopItems[i])
            {
                allShopItemsIndex = x;
                break;
            }
        }

        return allShopItemsIndex;
    }

    public int GetSlotIndex(UIShopButton newSlot)
    {
        //Get the index of the slot, from the allSlots array
        int allSlotsIndex = 0;
        for (int x = 0; x < allSlots.Length; x++)
        {
            if (allSlots[x] == newSlot)
            {
                allSlotsIndex = x;
                break;
            }
        }

        return allSlotsIndex;
    }

    [ClientRpc]
    private void Rpc_SetShopSlotValues(int allShopItemsIndex, int allSlotsIndex, bool weaponSlotItems)
    {
        var item = allShopItems[allShopItemsIndex];

        UIShopButton newSlot = allSlots[allSlotsIndex];
        newSlot.Item = item;
    }

    public void BuyItem(int index)
    {
        Cmd_BuyItem(index, playerGameObject);
    }

    [Command(ignoreAuthority = true)]
    public void Cmd_BuyItem(int index, GameObject player)
    {
        UIShopButton button = allSlots[index];
        ShopItem item = button.Item;

        if (GamemodeBase.Instance)
        {
            GamemodeBase gamemode = GamemodeBase.Instance;

            NetworkIdentity identity = player.GetComponent<NetworkIdentity>();
            uint playerId = identity.netId;

            //Check if the player has enough points to buy the item
            if (gamemode.Svr_GetPoints(playerId) < item.shopItemPrice)
            {
                //If not, nothing happens.
                return;
            }
            else
            {
                //If they have enough, detract the price from their points,
                gamemode.Svr_ModifyStat(playerId, -item.shopItemPrice, PlayerDataStat.Points);
                Rpc_ShowPoints(identity.connectionToClient, gamemode.Svr_GetPoints(playerId));
            }

        }

        //Buy the item for the player
        var spawnedItem = Instantiate(item.shopItemPrefab);
        NetworkServer.Spawn(spawnedItem);

        spawnedItem.GetComponent<EquipmentItem>().Svr_Interact(player);

        //And disable the item button for others, so they can't purchase it.
        //This is done by getting the index of the button in the allSlots array.
        //By using the index and not the gameobject itself, we avoid having to apply a networkidentity to the buttons
        for (int i = 0; i < allSlots.Length; i++)
        {
            //Find the correct one
            if (allSlots[i] == button)
            {
                //Tell everyone to disable the button with the index found
                Rpc_EnableItemButton(i, false);
                break;
            }
        }
    }

    [ClientRpc]
    private void Rpc_EnableItemButton(int buttonIndex, bool enable)
    {
        allSlots[buttonIndex].GetComponent<UIShopButton>().EnableShopButton(enable);
    }

    public void ShowWeaponInfo(ShopItem item)
    {
        if (item == null)
        {
            weaponStats.SetActive(false);
            return;
        }
        else
        {
            weaponNameText.text = item.shopItemName;

            bool isMelee = item.shopItemPrefab.TryGetComponent(out MeleeWeapon meleeComponent);
            bool isRanged = item.shopItemPrefab.TryGetComponent(out RangedWeapon rangedComponent);

            weaponDamageTypeText.text = isMelee ? "Damage Type" : "Ammo Type";
            weaponDamageTypeValueText.text = isMelee ? meleeComponent.DamageType.ToString() : rangedComponent.AmmunitionType.ToString();

            weaponDamageValueText.text = isMelee ? meleeComponent.Damage.ToString() : rangedComponent.Damage.ToString();

            weaponStats.SetActive(true);
        }
    }


    [Server]
    public void Svr_GenerateNewSelection()
    {
        //Reset
        currentWeaponSelection.Clear();
        currentEquipmentSelection.Clear();

        currentSelectionHealingItems = 0;
        currentSelectionMeleeWeapons = 0;

        //Enable all of the buttons
        for (int i = 0; i < allSlots.Length; i++)
        {
            Rpc_EnableItemButton(i, true);
        }

        //Add new items
        while (currentWeaponSelection.Count < weaponSlots.Length)
        {
            //First add the required amount of melee weapons to the selection (default 1)
            if (currentSelectionMeleeWeapons < minimumAmountOfMeleeWeapons)
            {
                currentWeaponSelection.Add(meleeWeapons[Random.Range(0, meleeWeapons.Count)]);
                currentSelectionMeleeWeapons++;
                continue;
            }

            //Then generate random weapons

            // 25% chance to be a melee weapon
            // 75% chance to be a ranged weapon
            bool newMeleeWeapon = Random.Range(1, 5) == 4;

            ShopItem newItem =newMeleeWeapon ?
                meleeWeapons[Random.Range(0, meleeWeapons.Count)] :
                rangedWeapons[Random.Range(0, rangedWeapons.Count)];


            currentWeaponSelection.Add(newItem);
        }
        if (currentWeaponSelection.Count > 0)
        {
            SetShopSlotValues(currentWeaponSelection, true);
        }
        else
        {
            Debug.LogError("Something went wrong. Shop generated without any weapons to be purchased. " +
         "This should not be possible");
        }

        while (currentEquipmentSelection.Count < equipmentSlots.Length)
        {
            //First add the required amount of healing items to the selection (default 2)
            if (currentSelectionHealingItems < minimumAmountOfHealingItems)
            {
                currentEquipmentSelection.Add(healingEquipment[Random.Range(0, healingEquipment.Count)]);
                currentSelectionHealingItems++;
                continue;
            }

            //Then generate random equipment

            // 25% chance to be a healing item
            // 75% chance to be any other item
            bool newHealingItem = Random.Range(1, 5) == 4;

            currentEquipmentSelection.Add(newHealingItem ?
                healingEquipment[Random.Range(0, healingEquipment.Count)] :
                equipment[Random.Range(0, equipment.Count)]);
        }
        if (currentEquipmentSelection.Count > 0)
        {
            SetShopSlotValues(currentEquipmentSelection, false);
        }
        else
        {
            Debug.LogError("Something went wrong. Shop generated without any equipment to be purchased. " +
         "This should not be possible");
        }
    }



    private void OnValidate()
    {
        foreach (var item in rangedWeapons)
        {
            item.shopItemName = item.shopItemPrefab.name;
        }        
        foreach (var item in meleeWeapons)
        {
            item.shopItemName = item.shopItemPrefab.name;
        }        
        foreach (var item in healingEquipment)
        {
            item.shopItemName = item.shopItemPrefab.name;
        }
        foreach (var item in equipment)
        {
            item.shopItemName = item.shopItemPrefab.name;
        }
    }
}
