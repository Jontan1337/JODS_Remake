using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieTentacle : UnitBase, IZombie, IControllable
{
    [SerializeField]
    private InfectionSO infection;
    public InfectionSO Infection { get => infection; set => infection = value; }
    [SerializeField]
    private int infectionAmount = 15;
    public int InfectionAmount { get => infectionAmount; set => infectionAmount = value; }

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

    public override void MeleeAttack()
    {
        base.MeleeAttack();

        if (!WithinMeleeRange() || !CanSee(currentTarget)) return;

        Infect(currentTarget);
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
            if (!CaughtSurvivor) yield break;

            //Do some damage yo
            Damage(special.specialDamage);
            print("oof");
        }
    }

    #region Interface Functions
    public void TakeControl()
    {
        throw new System.NotImplementedException();
    }

    public void Infect(Transform target)
    {
        target.GetComponent<StatusEffectManager>().ApplyStatusEffect(infection.ApplyEffect(target.gameObject), infectionAmount);
    }
    #endregion
}
