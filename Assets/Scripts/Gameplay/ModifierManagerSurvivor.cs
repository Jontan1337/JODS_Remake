using UnityEngine;
using System;

public class ModifierManagerSurvivor : MonoBehaviour
{
    public ModifierManagerSurvivorData data;
}

[System.Serializable]
public class ModifierManagerSurvivorData : ModifierManagerBase
{
    [SerializeField, Range(0, 10)] private float rangedDamage = 1;
    [SerializeField, Range(0, 10)] private float cooldown = 1;
    [SerializeField, Range(0, 10)] private float reloadSpeed = 1;
    [SerializeField, Range(0, 10)] private float accuracy = 1;


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

