using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieCommon : UnitBase
{
    public override void Attack()
    {
        if (CanMeleeAttack)
        {
            MeleeAttack();
            StartCoroutine(MeleeCooldownCoroutine());
        }
    }
}
