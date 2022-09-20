using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Sirenix.OdinInspector;

public class ModifierManagerUnit : ModifierManager
{
    [Header("Unit Modifiers")]
    [Space]
    [Space]

    [Title("Melee Modifiers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Range(0, 10)] private float meleeDamage = 1;
    [SerializeField, Range(0, 10)] private float meleeRange = 1;
    [SerializeField, Range(0, 10)] private float meleeCooldown = 1;

    [Title("Ranged Modifiers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Range(0, 10)] private float rangedDamage = 1;
    [SerializeField, Range(0, 10)] private float rangedRange = 1;
    [SerializeField, Range(0, 10)] private float rangedCooldown = 1;

    [Title("Special Modifiers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Range(0, 10)] private float specialDamage = 1;
    [SerializeField, Range(0, 10)] private float specialCooldown = 1;
    [SerializeField, Range(0, 10)] private float specialRange = 1;

    [Title("Movement Modifiers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Range(0, 10)] private float chaseTime = 1;

    [Title("Other Modifiers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Range(0, 10)] private float sightDistance = 1;
    [SerializeField, Range(0, 10)] private float alertSize = 1;
}
