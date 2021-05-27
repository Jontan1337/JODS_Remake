using UnityEngine;

public class ZombieSpitter : UnitBase, IZombie, IControllable
{
    public override void Attack()
    {
        if (!Infect())
        {
            return;
        }

        if (CanRangedAttack)
        {
            TryRangedAttack();
        }
        else if (CanMeleeAttack) 
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
        if (melee.statusEffectToApply == null || ranged.statusEffectToApply == null)
        {
            Debug.LogError(name + " has no infection debuff! Assign the 'Infection' as the 'Status Effect To Apply' on the UnitSO");
            return false;
        }
        if (melee.amount == 0 || ranged.amount == 0)
        {
            Debug.LogError(name + " has no infection amount!");
            return false;
        }
        return true;
    }
    #endregion
}
