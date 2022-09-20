using Mirror;
using UnityEngine;

public class ModifierManagerSurvivor : ModifierManager
{
    [SyncVar, SerializeField, Range(0, 10)] private float rangedDamage = 1;
    [SyncVar, SerializeField, Range(0, 10)] private float cooldown = 1;
    [SyncVar, SerializeField, Range(0, 10)] private float reloadSpeed = 0;
    [SyncVar, SerializeField, Range(0, 10)] private float accuracy = 0;


    public float RangedDamage
    {
        get { return rangedDamage; }
        set { rangedDamage = Mathf.Clamp(value, 0, 10); }
    }
    public float Accuracy
    {
        get { return accuracy; }
        set { accuracy = Mathf.Clamp(value, 0, 10); }
    }
    public float ReloadSpeed
    {
        get { return reloadSpeed; }
        set { reloadSpeed = Mathf.Clamp(value, 0, 10); }
    }
    public float Cooldown
    {
        get { return cooldown; }
        set { cooldown = Mathf.Clamp(value, 0, 10); }
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
