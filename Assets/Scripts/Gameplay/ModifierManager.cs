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


    public float MovementSpeed
    {
        get { return movementSpeed; }
        set { movementSpeed = Mathf.Clamp(value, 0, 10); }
    }

    public float Cooldown
    {
        get { return cooldown; }
        set { cooldown = Mathf.Clamp(value, 0, 10); }
    }

    public float Healing { get; set; }

    public float MeleeDamage
    {
        get { return meleeDamage; }
        set { meleeDamage = Mathf.Clamp(value, 0, 10); }
    }

    public float RangedDamage
    {
        get { return rangedDamage; }
        set { rangedDamage = Mathf.Clamp(value, 0, 10); }
    }
}

public enum Stats
{
    Movementspeed,
    Cooldown,
    Healing,
    MeleeDamage,
    RangedDamage
}
