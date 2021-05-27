using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSlav : UnitBase, IZombie, IControllable
{
    public override void Attack()
    {
        if (!Infect())
        {
            return;
        }

        if (CanMeleeAttack)
        {
            TryMeleeAttack();
        }
    }

    public override void OnSelect()
    {
        throw new System.NotImplementedException();
    }

    public override void OnDeselect()
    {
        throw new System.NotImplementedException();
    }

    #region Interface Functions
    public void TakeControl()
    {
        throw new System.NotImplementedException();
    }

    public bool Infect()
    {
        if (melee.statusEffectToApply == null)
        {
            Debug.LogError(name + " had no infection debuff assigned and could not infect the target");
            return false;
        }
        if (melee.amount == 0)
        {
            Debug.LogError(name + " had no infection amount and could not infect the target");
            return false;
        }
        return true;
    }
    #endregion
}
