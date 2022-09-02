using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UnitMeshGroup
{
    public Mesh[] bodyVariations;
    public Mesh[] headVariations;
    public Mesh[] leftArmVariations;
    public Mesh[] rightArmVariations;
}
[System.Serializable]
public struct UnitUpgradePath
{
    public int amountOfUpgrades;
    public float upgradeAmount;
}

public enum UnitDamageType
{
    Melee, Ranged, Special
}

[System.Serializable]
public struct StatusEffectToApply
{
    public StatusEffectSO statusEffect;
    public int amount;
}

[CreateAssetMenu(fileName = "Unit", menuName = "Unit Master/New Unit", order = 1)]
public class UnitSO : ScriptableObject
{
    [Header("General")]
    public new string name;
    public GameObject unitPrefab;
    [Header("Visual")]
    public UnitMeshGroup unitAppearanceVariations;
    [Space]
    public Material[] unitMaterialVariations;
    [Space]
    [Space]
    public Sprite unitSprite;
    [Header("Energy")]
    public int energyCost = 10;
    public int refundAmount = 5;
    [Header("Experience")]
    public int xpGain = 10;
    public int xpToUnlock = 100;
    public int xpToUpgrade = 100;
    [Header("Other Gameplay")]
    public bool starterUnit;
    [Space]
    public int maxAmountAlive = 50;

    [Header("Stats")]
    public int health = 100;
    [Space]
    public UnitDamageType unitDamageType = UnitDamageType.Melee;
    [Space]
    public bool hasMelee = true;
    public bool hasRanged = false;
    public bool hasSpecial = false;

    [System.Serializable]
    public class Melee
    {
        public int meleeDamageMin = 2;
        public int meleeDamageMax = 5;
        public float meleeRange = 2.5f;
        public float meleeCooldown = 0.5f;
        [Space]
        public List<StatusEffectToApply> statusEffectsToApply;
    }
    [Space]
    public Melee melee;

    [System.Serializable]
    public class Ranged
    {
        public int rangedDamage = 20;
        public int minRange = 2;
        public int maxRange = 20;
        public int rangedCooldown = 5;
        public int preferredRange = 15;
        [Space]
        public Tags projectileTag;
        public int projectileSpeed;
        public Vector3 projectileSpawnLocation;
        public bool standStill = true;
        [Space]
        public bool directRangedAttack = false; //Non-projectile attack. Not implemented yet because no unit uses it
        [Space]
        public List<StatusEffectToApply> statusEffectsToApply;
    }
    public Ranged ranged;

    [System.Serializable]
    public class Special
    {
        public int specialCooldown = 2;
        public int specialDamage = 0;
        public float specialTriggerRange = 5;
        public float specialRange = 5;
        [Space]
        public bool standStill = true;
        public bool lookAtTarget = true;
        public bool availableFromStart = true;
        [Space]
        public List<StatusEffectToApply> statusEffectsToApply;
    }
    public Special special;
    [System.Serializable]
    public class Upgrades
    {
        [Header("Upgrade milestone")]
        public int unitsToPlace = 50;
        public AnimationCurve upgradeCurve;

        [Header("Health")]
        public UnitUpgradePath unitUpgradesHealth;
        public string traitHealth;
        public string traitHealthDescription;
        [Header("Damage")]
        public UnitUpgradePath unitUpgradesDamage;
        public string traitDamage;
        public string traitDamageDescription;
        [Header("Speed")]
        public UnitUpgradePath unitUpgradesSpeed;
        public string traitSpeed;
        public string traitSpeedDescription;
    }
    [Space]
    public Upgrades upgrades;

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
    [Space]
    public Color bloodColor = Color.red;

    [System.Serializable]
    public class Sounds
    {
        public float headHeight = 2f;
        [Space]
        [Range(0, 2)] public float basePitch = 1f;
        [Space]
        public AudioClip[] idleSounds;
        [Range(0, 1)] public float idleVolume = 0.3f;
        [Space]
        public AudioClip[] alertSounds;
        [Range(0, 1)] public float alertVolume = 0.3f;
        [Range(0, 1)] public float alertingSoundChance = 0.9f;
        [Range(0, 1)] public float alertedSoundChance = 0.15f;
        [Space]
        public AudioClip[] meleeSounds;
        [Range(0, 1)] public float meleeVolume = 0.4f;
        [Range(0, 1)] public float meleeSoundChance = 0.15f;
        [Space]
        public AudioClip[] rangedSounds;
        [Range(0, 1)] public float rangedVolume = 0.4f;
        [Range(0, 1)] public float rangedSoundChance = 1f;
        [Space]
        public AudioClip[] specialSounds;
        [Range(0, 1)] public float specialVolume = 0.4f;
        [Range(0, 1)] public float specialSoundChance = 1f;
        [Space]
        public AudioClip[] footstepSounds;
        [Range(0, 1)] public float footstepVolume = 0.1f;
    }
    [Space]
    public Sounds sounds;

    [System.Serializable]
    public class Selectable
    {
        public bool canSelect = true;
    }
    [Space]
    public Selectable select;

    [Header("Details")]
    [TextArea(1, 5)] public string description = "This is a unit";
    [Range(0, 20)] public int powerStat;
    [Range(0, 20)] public int healthStat;


    private void OnValidate()
    {
        if (unitPrefab != null)
        {
            name = unitPrefab.name.Split('_')[0].Trim();
        }
    }
}
