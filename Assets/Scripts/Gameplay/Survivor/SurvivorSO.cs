using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Survivor", menuName = "Classes/Survivor/New Survivor Class", order = 1)]
public class SurvivorSO : ScriptableObject
{
    public int health;
    public int armor;
    public float movementSpeed;
    public float reloadSpeed;
    public float accuracy;
    public float ammoCapacity;
    public float abilityCooldown;
    public GameObject starterWeapon;
    public Object classScript;
    public Mesh survivorMesh;
    public Material survivorMaterial;
}
