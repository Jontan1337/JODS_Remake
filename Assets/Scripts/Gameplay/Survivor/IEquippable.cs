﻿using UnityEngine;
using Mirror;

public interface IEquippable
{
    string Name { get; }
    GameObject Item { get; }
    EquipmentType EquipmentType { get; }
}