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

    public virtual void ActiveAbility()
    {

    }
}



