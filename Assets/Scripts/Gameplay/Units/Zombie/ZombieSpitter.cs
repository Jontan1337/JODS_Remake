public class ZombieSpitter : UnitBase
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
}
