public class ZombieSpitter : UnitBase
{
    public override void Attack()
    {
        if (CanRangedAttack)
        {
            RangedAttack();
        }
        else if (CanMeleeAttack) 
        {
            MeleeAttack();
        }
    }
}
