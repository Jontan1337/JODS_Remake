using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Sirenix.OdinInspector;

public class ModifierManagerUnit : MonoBehaviour
{
    public ModifierManagerUnitData data;
}

[System.Serializable]
public class ModifierManagerUnitData : ModifierManagerBase
{
    

    [Header("Unit Modifiers")]
    [Space]
    [Space]

    [Title("Health Modifiers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Range(0, 10)] private float health = 1;
    public float Health
    {
        get { return health; }
        set { health = Mathf.Clamp(value, 0, 10); }
    }

    [Title("Melee Modifiers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Range(0, 10)] private float meleeDamage = 1;
    public float MeleeDamage
    {
        get { return meleeDamage; }
        set { meleeDamage = Mathf.Clamp(value, 0, 10); }
    }
    [SerializeField, Range(0, 10)] private float meleeRange = 1;
    public float MeleeRange
    {
        get { return meleeRange; }
        set { meleeRange = Mathf.Clamp(value, 0, 10); }
    }
    [SerializeField, Range(0, 10)] private float meleeCooldown = 1;
    public float MeleeCooldown
    {
        get { return meleeCooldown; }
        set { meleeCooldown = Mathf.Clamp(value, 0, 10); }
    }

    [Title("Ranged Modifiers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Range(0, 10)] private float rangedDamage = 1;
    public float RangedDamage
    {
        get { return rangedDamage; }
        set { rangedDamage = Mathf.Clamp(value, 0, 10); }
    }
    [SerializeField, Range(0, 10)] private float rangedRange = 1;
    public float RangedRange
    {
        get { return rangedRange; }
        set { rangedRange = Mathf.Clamp(value, 0, 10); }
    }
    [SerializeField, Range(0, 10)] private float rangedCooldown = 1;
    public float RangedCooldown
    {
        get { return rangedCooldown; }
        set { rangedCooldown = Mathf.Clamp(value, 0, 10); }
    }

    [Title("Special Modifiers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Range(0, 10)] private float specialDamage = 1;
    public float SpecialDamage
    {
        get { return specialDamage; }
        set { specialDamage = Mathf.Clamp(value, 0, 10); }
    }
    [SerializeField, Range(0, 10)] private float specialCooldown = 1;
    public float SpecialCooldown
    {
        get { return specialCooldown; }
        set { specialCooldown = Mathf.Clamp(value, 0, 10); }
    }
    [SerializeField, Range(0, 10)] private float specialRange = 1;
    public float SpecialRange
    {
        get { return specialRange; }
        set { specialRange = Mathf.Clamp(value, 0, 10); }
    }

    [Title("Movement Modifiers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Range(0, 10)] private float chaseTime = 1;
    public float ChaseTime
    {
        get { return chaseTime; }
        set { chaseTime = Mathf.Clamp(value, 0, 10); }
    }

    [Title("Other Modifiers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Range(0, 10)] private float sightDistance = 1;
    public float SightDistance
    {
        get { return sightDistance; }
        set { sightDistance = Mathf.Clamp(value, 0, 10); }
    }
    [SerializeField, Range(0, 10)] private float alertSize = 1;
    public float AlertSize
    {
        get { return alertSize; }
        set { alertSize = Mathf.Clamp(value, 0, 10); }
    }
}
