using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSClass : MonoBehaviour
{

    public GameObject _class;

    int health;
    int armor;
    float movementSpeed;
    float reloadSpeed;
    float accuracy;
    float ammoCapacity;
    float abilityCooldown;
    GameObject starterWeapon;

    private void Start()
    {
       
    }

    float abilityCooldownCount;


    IEnumerator ActivateAbility()
    {
        yield return new WaitForSeconds(abilityCooldownCount);
    }
}
