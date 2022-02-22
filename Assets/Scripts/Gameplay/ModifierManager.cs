using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierManager : NetworkBehaviour
{
    [SyncVar, SerializeField] private float movementSpeed = 1;
    [SyncVar, SerializeField] private float cooldown = 1;
    [SyncVar, SerializeField] private float healing = 1;
    [SyncVar, SerializeField] private float meleeDamage = 1;
    [SyncVar, SerializeField] private float rangedDamage = 1;


    public float MovementSpeed { get; set; }
    public float Cooldown { get; set; }
    public float Healing { get; set; }
    public float MeleeDamage { get; set; }
    public float RangedDamage { get; set; }
}

public enum Stats
{
    Movementspeed,
    Cooldown,
    Healing,
    MeleeDamage,
    RangedDamage
}
