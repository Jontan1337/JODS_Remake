using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Survivor", menuName = "Classes/Survivor/New Survivor Class", order = 1)]
public class SurvivorSO : ScriptableObject
{
    public string survivorName = "Default Survivor";

    [Header("Stats")]
    public int maxHealth;
    public int startingArmor;
    public float abilityCooldown;

    [Header("Modifiers")]
    public float movementSpeedModifier = 1;
    public float reloadSpeedModifier = 1;
    public float accuracyModifier = 1;

    [Header("Class specific")]
    public GameObject classScript;
    public GameObject starterWeapon;
    public GameObject abilityObject;
    public Mesh bodyMesh;
    public Mesh headMesh;
    public Material characterMaterial;

    [Header("Survivor Class Description")]
    [TextArea(10, 20)]
    public string classDescription = "This survivor survives";

    [Header("Survivor Class Special Description")]
    [TextArea(10, 20)]
    public string classSpecialDescription = "This survivor can do special stuff";
}
