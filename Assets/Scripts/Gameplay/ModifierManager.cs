using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class ModifierManager : NetworkBehaviour
{
    #region Base Modifers
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

    #endregion

    #region Resistance Modifiers

    [Header("Resistance Modifiers")]
    [SyncVar, SerializeField, Range(0, 10)] private float damageResistance = 1;
    [Space]
    [SyncVar, SerializeField, Range(0, 10)] private float slashResistance = 1;
    [SyncVar, SerializeField, Range(0, 10)] private float bluntResistance = 1;
    [SyncVar, SerializeField, Range(0, 10)] private float pierceResistance = 1;
    [Space]
    [SyncVar, SerializeField, Range(0, 10)] private float fireResistance = 1;


    public float DamageResistance
    {
        get { return damageResistance; }
        set { damageResistance = Mathf.Clamp(value, 0, 10); }
    }

    public float SlashResistance
    {
        get { return slashResistance; }
        set { slashResistance = Mathf.Clamp(value, 0, 10); }
    }

    public float BluntResistance
    {
        get { return bluntResistance; }
        set { bluntResistance = Mathf.Clamp(value, 0, 10); }
    }

    public float PierceResistance
    {
        get { return pierceResistance; }
        set { pierceResistance = Mathf.Clamp(value, 0, 10); }
    }

    public float FireResistance
    {
        get { return fireResistance; }
        set { fireResistance = Mathf.Clamp(value, 0, 10); }
    }
    #endregion

}
