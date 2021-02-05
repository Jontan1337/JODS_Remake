using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSClass : NetworkBehaviour, IDamagable
{
    private SurvivorClass sClass;

    [SerializeField] private SurvivorSO survivorSO;
    [SerializeField] private SkinnedMeshRenderer survivorRenderer;
    [Space]
    [Header("Stats")]
    [SerializeField] private int health = 0;
    [SerializeField] private int armor = 0;
    [SerializeField] private float abilityCooldown = 0;
    [SerializeField] private float movementSpeed = 0;

    [Space]
    [Header("Weapon Stats")]
    [SerializeField] private float reloadSpeed = 0;
    [SerializeField] private float accuracy = 0;
    [SerializeField] private float ammoCapacity = 0;
    //public GameObject starterWeapon;

    private void Awake()
    {
        JODSInput.Controls.Survivor.ActiveAbility.performed += ctx => sClass.ActiveAbility();
        SelectedClass();
        if (survivorSO.abilityObject)
        {
            sClass.abilityObject = survivorSO.abilityObject;
        }
        health = survivorSO.health;
        armor = survivorSO.armor;
        movementSpeed = survivorSO.movementSpeed;
        reloadSpeed = survivorSO.reloadSpeed;
        accuracy = survivorSO.accuracy;
        ammoCapacity = survivorSO.ammoCapacity;
        abilityCooldown = survivorSO.abilityCooldown;
        //starterWeapon = soldier.starterWeapon;
    }

    float abilityCooldownCount;

    public Teams Team => throw new System.NotImplementedException();

    IEnumerator ActivateAbility()
    {
        yield return new WaitForSeconds(abilityCooldownCount);
    }

    void SelectedClass()
    {
        System.Type selectedClass = System.Type.GetType(survivorSO.classScript.name + ",Assembly-CSharp");
        sClass = (SurvivorClass)gameObject.AddComponent(selectedClass);


        survivorRenderer.material = survivorSO.survivorMaterial;
        survivorRenderer.sharedMesh = survivorSO.survivorMesh;
    }
    [Server]
    public void Svr_Damage(int damage)
    {
        health -= damage;
    }
}
