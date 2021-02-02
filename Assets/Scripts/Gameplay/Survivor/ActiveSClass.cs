using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSClass : MonoBehaviour
{
    private SurvivorClass sClass;

    [SerializeField] private SurvivorSO survivorSO;
    [SerializeField] private SkinnedMeshRenderer survivorRenderer;

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
        JODSInput.Controls.Survivor.ActiveAbility.performed += ctx => sClass.ActiveAbility();
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
        System.Type selectedClass = System.Type.GetType(survivorSO.classScript.name + ",Assembly-CSharp");
        sClass = (SurvivorClass)gameObject.AddComponent(selectedClass);

        survivorRenderer.material = survivorSO.survivorMaterial;
        survivorRenderer.sharedMesh = survivorSO.survivorMesh;




        //switch (survivorSO.name)        {

        //    case "Soldier":
        //        survivorSO.classScript = gameObject.AddComponent<SoldierClass>();
        //        break;

        //    case "Taekwondo":
        //        survivorSO.classScript = gameObject.AddComponent<TaekwondoClass>();
        //        break;

        //    default:
        //        break;
        //}
    }
}
