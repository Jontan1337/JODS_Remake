using Mirror;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomReadWrite
{
    private static readonly ILogger logger = LogFactory.GetLogger<NetworkWriter>();
    #region Equipment
    public static void WriteEquipment(this NetworkWriter writer, PlayerEquipment value)
    {
        if (value == null)
        {
            writer.WriteUInt32(0);
            return;
        }
        NetworkIdentity identity = value.GetComponent<NetworkIdentity>();
        if (identity != null)
        {
            writer.WriteUInt32(identity.netId);
        }
        else
        {
            logger.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
            writer.WriteUInt32(0);
        }
    }
    public static PlayerEquipment ReadEquipment(this NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        if (identity == null)
        {
            return null;
        }
        return identity.GetComponent<PlayerEquipment>();
    }
    public static void WriteEquipmentSlot(this NetworkWriter writer, EquipmentSlot value)
    {
        if (value == null)
        {
            writer.WriteUInt32(0);
            return;
        }
        NetworkIdentity identity = value.GetComponent<NetworkIdentity>();
        if (identity != null)
        {
            writer.WriteUInt32(identity.netId);
        }
        else
        {
            logger.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
            writer.WriteUInt32(0);
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
    public static void WriteEquipmentItem(this NetworkWriter writer, EquipmentItem value)
    {
        if (value == null)
        {
            writer.WriteUInt32(0);
            return;
        }
        NetworkIdentity identity = value.GetComponent<NetworkIdentity>();
        if (identity != null)
        {
            writer.WriteUInt32(identity.netId);
        }
        else
        {
            logger.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
            writer.WriteUInt32(0);
        }
    }
    public static EquipmentItem ReadEquipmentItem(this NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        if (identity == null)
        {
            return null;
        }
        return identity.GetComponent<RaycastWeapon>();
    }
    public static void WriteRaycastWeapon(this NetworkWriter writer, RaycastWeapon value)
    {
        if (value == null)
        {
            writer.WriteUInt32(0);
            return;
        }
        NetworkIdentity identity = value.GetComponent<NetworkIdentity>();
        if (identity != null)
        {
            writer.WriteUInt32(identity.netId);
        }
        else
        {
            logger.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
            writer.WriteUInt32(0);
        }
    }
    public static RaycastWeapon ReadRaycastWeapon(this NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        if (identity == null)
        {
            return null;
        }
        return identity.GetComponent<RaycastWeapon>();
    }
    public static void WriteProjectileWeapon(this NetworkWriter writer, ProjectileWeapon value)
    {
        if (value == null)
        {
            writer.WriteUInt32(0);
            return;
        }
        NetworkIdentity identity = value.GetComponent<NetworkIdentity>();
        if (identity != null)
        {
            writer.WriteUInt32(identity.netId);
        }
        else
        {
            logger.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
            writer.WriteUInt32(0);
        }
    }
    public static ProjectileWeapon ReadProjectileWeapon(this NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        if (identity == null)
        {
            return null;
        }
        return identity.GetComponent<ProjectileWeapon>();
    }
    #endregion

    #region Survivor Classes
    public static void WriteTaekwondoClass(this NetworkWriter writer, TaekwondoClass value)
    {
        if (value == null)
        {
            writer.WriteUInt32(0);
            return;
        }
        NetworkIdentity identity = value.GetComponent<NetworkIdentity>();
        if (identity != null)
        {
            writer.WriteUInt32(identity.netId);
        }
        else
        {
            logger.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
            writer.WriteUInt32(0);
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
        if (value == null)
        {
            writer.WriteUInt32(0);
            return;
        }
        NetworkIdentity identity = value.GetComponent<NetworkIdentity>();
        if (identity != null)
        {
            writer.WriteUInt32(identity.netId);
        }
        else
        {
            logger.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
            writer.WriteUInt32(0);
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
        if (value == null)
        {
            writer.WriteUInt32(0);
            return;
        }
        NetworkIdentity identity = value.GetComponent<NetworkIdentity>();
        if (identity != null)
        {
            writer.WriteUInt32(identity.netId);
        }
        else
        {
            logger.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
            writer.WriteUInt32(0);
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

    public static void WriteHandIKEffectors(this NetworkWriter writer, HandIKEffectors value)
    {
        if (value == null)
        {
            writer.WriteUInt32(0);
            return;
        }
        NetworkIdentity identity = value.GetComponent<NetworkIdentity>();
        if (identity != null)
        {
            writer.WriteUInt32(identity.netId);
        }
        else
        {
            logger.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
            writer.WriteUInt32(0);
        }
    }
    public static HandIKEffectors ReadHandIKEffectors(this NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        if (identity == null)
        {
            return null;
        }
        return identity.GetComponent<HandIKEffectors>();
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


    public static void WriteFullBodyBipedIK(this NetworkWriter writer, FullBodyBipedIK value)
    {
        if (value == null)
        {
            writer.WriteUInt32(0);
            return;
        }
        NetworkIdentity identity = value.GetComponent<NetworkIdentity>();
        if (identity != null)
        {
            writer.WriteUInt32(identity.netId);
        }
        else
        {
            logger.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
            writer.WriteUInt32(0);
        }
    }
    public static FullBodyBipedIK ReadFullBodyBipedIK(this NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        if (identity == null)
        {
            return null;
        }
        return identity.GetComponent<FullBodyBipedIK>();
    }

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
