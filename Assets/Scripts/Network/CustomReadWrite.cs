using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomReadWrite
{
    #region Equipment
    public static void WriteEquipment(this NetworkWriter writer, PlayerEquipment value)
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
    public static PlayerEquipment ReadEquipment(this NetworkReader reader)
    {
        ILogger logger = LogFactory.GetLogger<NetworkReader>();
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        logger.Log($"Reading {identity}");
        if (identity == null)
        {
            return null;
        }
        return identity.GetComponent<PlayerEquipment>();
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
    #endregion

    #region Survivor Classes
    public static void WriteTaekwondoClass(this NetworkWriter writer, TaekwondoClass value)
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
    public static TaekwondoClass ReadTaekwondoClass(this NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        if (identity == null)
        {
            return null;
        }
        return identity.GetComponent<TaekwondoClass>();
    }

    public static void WriteSoldierClass(this NetworkWriter writer, SoldierClass value)
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
    public static SoldierClass ReadSoldierClass(this NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        if (identity == null)
        {
            return null;
        }
        return identity.GetComponent<SoldierClass>();
    }

    public static void WriteEngineerClass(this NetworkWriter writer, EngineerClass value)
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
    public static EngineerClass ReadEngineerClass(this NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        if (identity == null)
        {
            return null;
        }
        return identity.GetComponent<EngineerClass>();
    }
    /*
    public static void WriteSurvivorClass(this NetworkWriter writer, SurvivorClass value)
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
    public static SurvivorClass ReadSurvivorClass(this NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        if (identity == null)
        {
            return null;
        }
        return identity.GetComponent<SurvivorClass>();
    }
    */
    #endregion

    /*
    public static void WriteActiveSClass(this NetworkWriter writer, ActiveSClass value)
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
    public static ActiveSClass ReadActiveSClass(this NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        if (identity == null)
        {
            return null;
        }
        return identity.GetComponent<ActiveSClass>();
    }
    */
}
