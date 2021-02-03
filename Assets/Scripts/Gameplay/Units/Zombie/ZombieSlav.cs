﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSlav : UnitBase, IZombie, IControllable
{
    public override void Attack()
    {
        if (CanMeleeAttack)
        {
            TryMeleeAttack();
        }
    }

    #region Interface Functions
    public void TakeControl()
    {
        throw new System.NotImplementedException();
    }

    public void Infect(Transform target)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
