using UnityEngine;

public class ZombieSpitter : UnitBase, IZombie, IControllable
{
    public override void Attack()
    {
        if (CanRangedAttack)
        {
            TryRangedAttack();
        }
        else if (CanMeleeAttack) 
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
