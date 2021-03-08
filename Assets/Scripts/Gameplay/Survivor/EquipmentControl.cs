using UnityEngine;
using Mirror;

public class EquipmentControl : NetworkBehaviour
{
    private Equipment equipment;
    public override void OnStartAuthority()
    {
        GetComponent<PlayerSetup>().onSpawnEquipment += GetEquipment;
    }
    public override void OnStopAuthority()
    {
        GetComponent<PlayerSetup>().onSpawnEquipment -= GetEquipment;
        equipment.onEquippedItemChange -= ChangeControlBind;
    }

    private void GetEquipment(Equipment equipment)
    {
        this.equipment = equipment;
        this.equipment.onEquippedItemChange += ChangeControlBind;
    }

    private void ChangeControlBind(GameObject oldItem, GameObject newItem)
    {
        print("ChangeControlBind");

        if (oldItem)
            UnBind(oldItem);

        if (newItem)
            Bind(newItem);
    }
    private void Bind(GameObject item)
    {
        print("Bind");
        item.GetComponent<IBindable>().Bind();
    }
    private void UnBind(GameObject item)
    {
        print("UnBind");
        item.GetComponent<IBindable>().UnBind();
    }
}
