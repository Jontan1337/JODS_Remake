using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class ModifierManager : NetworkBehaviour
{
    [Header("Base Modifiers")]
    [SyncVar, SerializeField, Range(0, 10)] private float movementSpeed = 1;
    [Space]
    [SyncVar, SerializeField, Range(0, 10)] private float healing = 1;
    [Space]
    [SyncVar, SerializeField, Range(0, 10)] private float damage = 1;

    public float MovementSpeed
    {
        get { return movementSpeed; }
        set { movementSpeed = Mathf.Clamp(value, 0, 10); }
    }

    public float Healing 
    {
        get { return healing; }
        set { healing = Mathf.Clamp(value, 0, 10); }
    }

    public float Damage
    {
        get { return damage; }
        set { damage = Mathf.Clamp(value, 0, 10); }
    }
}
