using UnityEngine;
using Mirror;

public class EquipmentControl : NetworkBehaviour
{
    private Equipment equipment;


    public override void OnStartServer()
    {
        GetComponent<PlayerSetup>().onSpawnEquipment += GetEquipment;
    }
    public override void OnStopServer()
    {
        GetComponent<PlayerSetup>().onSpawnEquipment -= GetEquipment;
        equipment.onServerEquippedItemChange -= Svr_ChangeControlBind;
    }

    private void GetEquipment(Equipment equipment)
    {
        this.equipment = equipment;
        this.equipment.onServerEquippedItemChange += Svr_ChangeControlBind;
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
