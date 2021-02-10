using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomReadWrite
{
    public static void WriteEquipment(this NetworkWriter writer, Equipment value)
    {
        ILogger logger = LogFactory.GetLogger<NetworkWriter>();
        if (value == null)
        {
            return;
        }
        NetworkIdentity networkIdentity = value.GetComponent<NetworkIdentity>();
        logger.Log($"Writing {networkIdentity}");
        if (networkIdentity != null)
        {
            writer.WriteNetworkIdentity(networkIdentity);
        }
        else
        {
            logger.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
            writer.WriteNetworkIdentity(null);
        }
    }
    public static Equipment ReadEquipment(this NetworkReader reader)
    {
        ILogger logger = LogFactory.GetLogger<NetworkReader>();
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        logger.Log($"Reading {identity}");
        if (identity == null)
        {
            return null;
        }
        return identity.GetComponent<Equipment>();
    }

    public static void WriteEquipmentSlot(this NetworkWriter writer, EquipmentSlot value)
    {
        ILogger logger = LogFactory.GetLogger<NetworkWriter>();
        if (value == null)
        {
            return;
        }
        NetworkIdentity networkIdentity = value.GetComponent<NetworkIdentity>();
        if (networkIdentity != null)
        {
            writer.WriteNetworkIdentity(networkIdentity);
        }
        else
        {
            logger.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
            writer.WriteNetworkIdentity(null);
        }
    }
    public static EquipmentSlot ReadEquipmentSlot(this NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        if (identity == null)
        {
            return null;
        }
        return identity.GetComponent<EquipmentSlot>();
    }

    //public static void WriteIEquippable(this NetworkWriter writer, IEquippable value)
    //{
    //    ILogger logger = LogFactory.GetLogger<NetworkWriter>();
    //    if (value == null)
    //    {
    //        return;
    //    }
    //    NetworkIdentity networkIdentity = value.Item.GetComponent<NetworkIdentity>();
    //    if (networkIdentity != null)
    //    {
    //        writer.WriteNetworkIdentity(networkIdentity);
    //    }
    //    else
    //    {
    //        logger.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
    //        writer.WriteNetworkIdentity(null);
    //    }
    //}
    //public static IEquippable ReadIEquippable(this NetworkReader reader)
    //{
    //    NetworkIdentity identity = reader.ReadNetworkIdentity();
    //    if (identity == null)
    //    {
    //        return null;
    //    }
    //    return identity.GetComponent<IEquippable>();
    //}
}
