using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpitter : UnitBase
{
    public override void Attack()
    {
        if (CanRangedAttack)
        {
            RangedAttack();
            StartCoroutine(RangedCooldownCoroutine());
        }
        else if (CanMeleeAttack) 
        {
            MeleeAttack();
            StartCoroutine(MeleeCooldownCoroutine());
        }
    }
}
