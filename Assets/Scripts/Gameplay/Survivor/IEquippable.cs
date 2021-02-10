using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquippable
{
    string Name { get; }
    GameObject Item { get; }
    EquipmentType EquipmentType { get; }
}
