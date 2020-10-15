﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Units/New Unit", order = 1)]

public class SOUnit : ScriptableObject
{
    [Header("Necessities")]
    public new string name;
    public GameObject[] unitPrefab;
    public Sprite unitSprite;
    [Space]
    public int energyCost = 10;
    public int refundAmount = 5;
    [Space]
    public int xpGain = 10;
    public int xpToUnlock = 100;
    public int xpToUpgrade = 100;
    [Space]
    public bool starterUnit;

    [Header("Stats")]
    public int health = 100;
    [Space]
    public bool isMelee = true;
    public bool isRanged = false;
    public bool hasSpecial = false;
    [Space]
    public int meleeDamage = 10;
    public float meleeRange = 2.5f;
    public float meleeCooldown = 1f;
    [Space]
    public int rangedDamage = 20;
    public int minRange = 2;
    public int maxRange = 20;
    public float rangedCooldown = 5f;
    [Space]
    public float specialCooldown = 20f;


    [Header("Movement")]
    public float movementSpeed = 1.5f;
    [Space]
    public float chaseTime = 10; //How long the unit will chase, if the unit can't see it's target

    [Header("Other")]
    public int sightDistance = 20;
    public float eyeHeight = 2f;
    public int viewAngle = 50;
    public int alertRadius = 20;
    public bool canAlert = true;

    [Header("Sounds")]
    public AudioClip[] idleSounds;
    public AudioClip[] footstepSounds;

    [Header("Animations")]
    public RuntimeAnimatorController unitAnimator;
    /*
    public AnimationClip[] idleAnimation;
    public AnimationClip[] walkAnimation;
    public AnimationClip[] runAnimation;
    public AnimationClip[] meleeAnimation;
    public AnimationClip[] rangedAnimation;
    public AnimationClip[] specialAnimation;
    public AnimationClip[] dieAnimation; //TEMPORARY UNTILL RAGDOLL IS M'DOONE
    */

    private void OnValidate()
    {
        if (unitPrefab.Length != 0)
        {
            name = unitPrefab[0].name.Split('_')[0].Trim();
        }
    }
}
