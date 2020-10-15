using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class UseItem : NetworkBehaviour
{
	[SyncVar] public bool hasItem = false;

	public new Camera camera;
	public GameObject grenadeSpawnPosition;
	public GameObject currentItem;
	private Item item;
	[SerializeField] private PlaceItem placeItem = null;
	[SerializeField] private Text itemText = null;

	void Update()
	{
		if (!hasAuthority) return;

		if (hasItem)
		{
			if (Input.GetKey(KeyCode.G))
			{
				switch (currentItem.GetComponent<Item>().type)
				{
					case Item.Type.placable:
						placeItem.item = currentItem;
						placeItem.Place(true);
						break;
					case Item.Type.heal:
						CmdHeal();
						break;
					case Item.Type.grenade:
						CmdThrowGrenade();
						break;
					default:
						break;
				}
				itemText.GetComponent<Text>().text = "";
			}
			if (Input.GetKeyUp(KeyCode.G) && currentItem.GetComponent<Item>().type == Item.Type.placable)
			{
				PlaceableItem();
			}
		}
		else
		{
			itemText.GetComponent<Text>().text = "";
		}
	}
	public void PlaceableItem()
	{
		if (placeItem.Place(false))
		{
			Cmd_SetNoItem();
		}
	}

	[Command]
	private void Cmd_SetNoItem()
	{
		hasItem = false;
	}

	[Command]
	public void CmdThrowGrenade()
	{
		GameObject activeGrenade = Instantiate(currentItem, new Vector3(grenadeSpawnPosition.transform.position.x, grenadeSpawnPosition.transform.position.y, grenadeSpawnPosition.transform.position.z), transform.rotation);
		activeGrenade.GetComponent<Grenade>().thrown = true;
		activeGrenade.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 1000);
		activeGrenade.GetComponent<Rigidbody>().AddTorque(0, 0, -10);
		NetworkServer.Spawn(activeGrenade);
		hasItem = false;
	}

	[Command]
	public void CmdHeal()
	{
		Stats stats = GetComponent<Stats>();
		stats.HealthPoints = stats.HealthPoints >= 75 ? 100 : stats.HealthPoints + 25;
		hasItem = false;
	}


	//-----------------------------ITEM PICK UP--------------------------------

	public void NewItem(GameObject newItem)
	{
		CmdNewItem(newItem);
		CmdDestroyWeapon(newItem);
		itemText.GetComponent<Text>().text = newItem.GetComponent<Item>().itemName;
	}
	[Command]
	void CmdNewItem(GameObject newItem)
	{
		if (newItem.GetComponent<Item>().type == Item.Type.heal)
		{
			currentItem = Resources.Load<GameObject>("Spawnables/Pickup/Pickup_Bandage");
			hasItem = true;
		}
		else if (newItem)
		{
			currentItem = newItem.GetComponent<WeaponType>().weaponPrefab;
			hasItem = true;
		}
		RpcNewItem(connectionToClient, newItem);
	}
	[TargetRpc]
	void RpcNewItem(NetworkConnection target, GameObject newItem)
	{
		if (newItem)
		{
			currentItem = newItem.GetComponent<WeaponType>().weaponPrefab;
		}
	}
	[Command]
	void CmdDestroyWeapon(GameObject weapon)
	{
		NetworkServer.Destroy(weapon);
	}
}
