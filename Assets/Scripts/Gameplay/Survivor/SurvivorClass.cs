using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SurvivorClass : MonoBehaviour
{

    protected int health;
    protected int armor;
    protected float movementSpeed;
    protected float reloadSpeed;
    protected float accuracy;
    protected float ammoCapacity;
    protected float abilityCooldown;
    protected GameObject starterWeapon;

    //public int health;
    //public int armor;
    //public float movementSpeed;
    //public float reloadSpeed;
    //public float accuracy;
    //public float ammoCapacity;
    //public float abilityCooldown;
    //public GameObject starterWeapon;

    public virtual void ActiveAbility()
    {

    }
}



