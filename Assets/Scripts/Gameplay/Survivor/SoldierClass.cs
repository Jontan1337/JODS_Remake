using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierClass : SurvivorClass
{
    public GameObject weapon;

    private void Start()
    {
        health = 100;
        armor = 80;
        movementSpeed = 0.8f;
        reloadSpeed = 1.5f;
        ammoCapacity = 1.5f;
        accuracy = 1.5f;
        starterWeapon = weapon;
    }

    public override void ActiveAbility()
    {
        //Shoot rocket
    }
}
