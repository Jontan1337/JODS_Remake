using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Survivor", menuName = "Classes/Survivor/New Survivor Class", order = 1)]
public class SurvivorSO : ScriptableObject
{
    public string survivorName = "Default Survivor";

    [Header("Stats")]
    public int health;
    public int armor;
    public float movementSpeed;
    public float abilityCooldown;

    [Header("Weapon Stats")]
    public float reloadSpeed;
    public float accuracy;
    public float ammoCapacity;

    [Header("Class specific")]
    public GameObject classScript;
    public GameObject starterWeapon;
    public GameObject abilityObject;
    public Mesh survivorMesh;
    public Material survivorMaterial;

    [Header("Survivor Class Description")]
    [TextArea(10, 20)]
    public string classDescription = "This survivor survives";

    [Header("Survivor Class Special Description")]
    [TextArea(10, 20)]
    public string classSpecialDescription = "This survivor can do special stuff";
}
