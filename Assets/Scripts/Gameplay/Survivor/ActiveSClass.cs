﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSClass : NetworkBehaviour, IDamagable
{
    private SurvivorClass sClass;
    private SurvivorController sController;
    private Object classScript;

    [SerializeField] private SurvivorSO survivorSO;
    [SerializeField] private SkinnedMeshRenderer survivorRenderer;
    [Space]
    [Header("Stats")]
    [SerializeField] private int health = 100;
    [SerializeField] private int armor = 0;
    [SerializeField] private float abilityCooldown = 0;
    [SerializeField] private float abilityCooldownCount = 0;
    [SerializeField] private float movementSpeed = 0;

    [Space]
    [Header("Weapon Stats")]
    [SerializeField] private float reloadSpeed = 0;
    [SerializeField] private float accuracy = 0;
    [SerializeField] private float ammoCapacity = 0;

    private bool abilityIsReady = true;

    //public GameObject starterWeapon;

    private void Awake()
    {
        sController = GetComponent<SurvivorController>();
        //JODSInput.Controls.Survivor.ActiveAbility.performed += ctx => sClass.ActiveAbility();
        JODSInput.Controls.Survivor.ActiveAbility.performed += ctx => Ability();
        SetSurvivorClass(survivorSO);
        SelectedClass();
        if (survivorSO.abilityObject)
        {
            sClass.abilityObject = survivorSO.abilityObject;
        }
        armor                = survivorSO.armor;
        health               = survivorSO.health;
        accuracy             = survivorSO.accuracy;
        reloadSpeed          = survivorSO.reloadSpeed;
        ammoCapacity         = survivorSO.ammoCapacity;
        movementSpeed        = survivorSO.movementSpeed;
        abilityCooldown      = survivorSO.abilityCooldown;
        abilityCooldownCount = abilityCooldown;
        sController.speed   *= movementSpeed;

        //starterWeapon     = soldier.starterWeapon;
    }

    void Ability()
    {
        if (abilityIsReady)
        {
            sClass.ActiveAbility();
            if (sClass.abilityActivatedSuccesfully)
            {
                StartCoroutine(AbilityCooldown());
                sClass.abilityActivatedSuccesfully = false;
            }
        }
    }

    IEnumerator AbilityCooldown()
    {
        abilityIsReady = false;
        while (abilityCooldownCount > 0)
        {
            abilityCooldownCount -= 1;
            yield return new WaitForSeconds(1);
        }
        abilityCooldownCount = abilityCooldown;
        abilityIsReady = true;
    }

    public void SetSurvivorClass(SurvivorSO survivorSO)
    {
        this.survivorSO = survivorSO;
    }

    void SelectedClass()
    {
        System.Type selectedClass = System.Type.GetType(survivorSO.classScript.name + ",Assembly-CSharp");
        sClass = (SurvivorClass)gameObject.AddComponent(selectedClass);

        survivorRenderer.material = survivorSO.survivorMaterial;
        survivorRenderer.sharedMesh = survivorSO.survivorMesh;
    }
    public Teams Team => Teams.Player;
    [Server]
    public void Svr_Damage(int damage, Transform target = null)
    {
        if (armor > 0) armor -= damage;
        else health -= damage;
    }

    public int GetHealth() => health;

    public bool IsDead() => false;
}
