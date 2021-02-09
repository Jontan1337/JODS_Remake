using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Survivor", menuName = "Classes/Survivor/New Survivor Class", order = 1)]
public class SurvivorSO : ScriptableObject
{
    [Header("Stats")]
    public float movementSpeed;
    public float abilityCooldown;

    [Header("Weapon Stats")]
    public float reloadSpeed;
    public float accuracy;
    public float ammoCapacity;

    [Header("Class specific")]
    public Object classScript;
    public GameObject starterWeapon;
    public GameObject abilityObject;
    public Mesh survivorMesh;
    public Material survivorMaterial;
}
