using UnityEngine;
using Mirror;

public class EquipmentControl : NetworkBehaviour
{
    private Equipment equipment;

    public override void OnStartServer()
    {
        transform.root.GetComponent<PlayerSetup>().onSpawnItem += GetEquipment;
    }
    public override void OnStopServer()
    {
        transform.root.GetComponent<PlayerSetup>().onSpawnItem -= GetEquipment;
        equipment.onServerEquippedItemChange -= Svr_ChangeControlBind;
    }

    public void GetEquipment(GameObject item)
    {
        if (item.TryGetComponent(out ItemName itemName))
        {
            switch (itemName.itemName)
            {
                case ItemNames.Equipment:
                    this.equipment = item.GetComponent<Equipment>();
                    this.equipment.onServerEquippedItemChange += Svr_ChangeControlBind;
                    break;
                default:
                    break;
            }
        }
    }

    [Server]
    private void Svr_ChangeControlBind(GameObject oldItem, GameObject newItem)
    {
        print("ChangeControlBind");

        if (oldItem)
            Rpc_UnBind(connectionToClient, oldItem);

        if (newItem)
            Rpc_Bind(connectionToClient, newItem);
    }
    [TargetRpc]
    private void Rpc_Bind(NetworkConnection target, GameObject item)
    {
        print("Bind");
        item.GetComponent<IBindable>().Bind();
    }
    [TargetRpc]
    private void Rpc_UnBind(NetworkConnection target, GameObject item)
    {
        print("UnBind");
        item.GetComponent<IBindable>().UnBind();
    }
}
