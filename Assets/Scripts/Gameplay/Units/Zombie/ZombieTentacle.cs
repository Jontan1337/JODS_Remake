using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieTentacle : UnitBase
{
    [Header("Tentacle")]
    [SerializeField] private bool CaughtSurvivor = false;
    //private player playerInGrasp; //Ya know, make this when players are made
    public override void Attack()
    {
        if (CanSpecialAttack)
        {
            TrySpecialAttack();
        }
        else if (CanMeleeAttack)
        {
            TryMeleeAttack();
        }
    }
    public override void SpecialAttack()
    {
        Debug.Log("Gotcha bitch!");

        //Try to capture a player, making them unable to move or use weapons.

        //If successful ------
        CaughtSurvivor = true;
        animator.SetBool("Grapple", true);

        //Call DamageOverTime
        StartCoroutine(DamageOverTime());
    }
    private IEnumerator DamageOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            //Check if unit still has a player to damage (maybe the player died while in its grasp)
            //If so, leave the special attack, and continue searching for other survivors.


            //Do some damage yo
            Damage(special.specialDamage);
            print("oof");
        }
    }
}
