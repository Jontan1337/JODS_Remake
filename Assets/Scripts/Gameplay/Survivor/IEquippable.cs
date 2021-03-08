using UnityEngine;
using Mirror;

public interface IEquippable
{
    string Name { get; }
    GameObject Item { get; }
    EquipmentType EquipmentType { get; }

    [Server]
    void Svr_GiveAuthority(NetworkConnection conn);
    [Server]
    void Svr_RemoveAuthority();
}
