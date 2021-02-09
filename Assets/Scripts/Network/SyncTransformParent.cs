using UnityEngine;
using Mirror;

public class SyncTransformParent : NetworkBehaviour
{
    [SerializeField, SyncVar]
    private Transform parent;

    private void OnTransformParentChanged()
    {
        if (isServer)
        {
            parent = transform.parent;
        }
    }

    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        writer.WriteTransform(transform.parent);
        base.OnSerialize(writer, initialState);
        return true;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        parent = reader.ReadTransform();
    }
}

//public static class ReadWriteTransform
//{
//    public static void WriteTransform(this NetworkWriter writer, EquipmentSlot equipmentSlot)
//    {
//        NetworkIdentity networkIdentity = equipmentSlot.GetComponent<NetworkIdentity>();
//        writer.WriteNetworkIdentity(networkIdentity);
//    }
//    public static Transform ReadTransform(this NetworkReader reader)
//    {
//        NetworkIdentity networkIdentity = reader.ReadNetworkIdentity();
//        Transform transform = networkIdentity.GetComponent<Transform>();
//        return transform;
//    }

//}