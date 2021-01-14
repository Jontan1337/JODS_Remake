using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieTentacle : UnitBase
{
    public override void Attack()
    {
        if (CanMeleeAttack)
        {
            MeleeAttack();
        }
    }
}
