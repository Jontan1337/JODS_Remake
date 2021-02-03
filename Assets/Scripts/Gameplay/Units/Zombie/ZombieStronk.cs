using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieStronk : UnitBase, IZombie, IControllable
{
    [SerializeField]
    private InfectionSO infection;
    public InfectionSO Infection { get => infection; set => infection = value; }
    [SerializeField]
    private int infectionAmount = 15;
    public int InfectionAmount { get => infectionAmount; set => infectionAmount = value; }

    [Header("Stronk")]
    [SerializeField] private Mesh[] weaponMeshes = null;
    [SerializeField] private GameObject weapon = null;
    public ParticleSystem slamFX;

    public override void Start()
    {
        base.Start();
        weapon.GetComponent<MeshFilter>().mesh = weaponMeshes[Random.Range(0, weaponMeshes.Length)];
    }

    public override void Attack()
    {
        if (CanMeleeAttack)
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
