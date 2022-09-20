using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class ModifierManager : NetworkBehaviour
{
    [SyncVar, SerializeField, Range(0, 10)] private float movementSpeed = 1;
    [SyncVar, SerializeField, Range(0, 10)] private float healing = 1;
    [SyncVar, SerializeField, Range(0, 10)] private float meleeDamage = 1;

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

    public float MeleeDamage
    {
        get { return meleeDamage; }
        set { meleeDamage = Mathf.Clamp(value, 0, 10); }
    }
}
