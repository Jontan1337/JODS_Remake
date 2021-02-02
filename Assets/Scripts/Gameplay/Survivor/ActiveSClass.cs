using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSClass : MonoBehaviour
{
    ClassList classList = new ClassList();

    [SerializeField] private SurvivorSO survivorSO;
    [SerializeField] int health;
    [SerializeField] int armor;
    [SerializeField] float movementSpeed;
    [SerializeField] float reloadSpeed;
    [SerializeField] float accuracy;
    [SerializeField] float ammoCapacity;
    [SerializeField] float abilityCooldown;
    //public GameObject starterWeapon;

    private void Start()
    {
        JODSInput.Controls.Survivor.ActiveAbility.performed += ctx => survivorSO.classScript.ActiveAbility();
        SelectedClass();
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


    IEnumerator ActivateAbility()
    {
        yield return new WaitForSeconds(abilityCooldownCount);
    }

    void SelectedClass()
    {
        switch (survivorSO.name)        {

            case "Soldier":
                survivorSO.classScript = gameObject.AddComponent<SoldierClass>();
                break;

            case "Taekwondo":
                survivorSO.classScript = gameObject.AddComponent<TaekwondoClass>();
                break;

            default:
                break;
        }
    }
}
