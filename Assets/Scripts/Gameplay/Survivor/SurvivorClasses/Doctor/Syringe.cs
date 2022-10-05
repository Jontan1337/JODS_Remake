using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Syringe : Projectile
{
    private Survivor survivor;
    [SerializeField] StatusEffectSO statusEffect = null;

    private SurvivorPlayerData survivorPlayerData;

    public override void OnInstantiate()
    {
        base.OnInstantiate();
        objectPoolTag = Tags.Syringe;
        transform.Rotate(new Vector3(90, 0, 0));

        survivorPlayerData = owner.GetComponent<SurvivorPlayerData>();
        survivor = owner.GetComponent<Survivor>();
    }

    [Server]
    public override void OnHit(Collision hit)
    {
        var reviveManager = hit.collider.transform.root.gameObject.GetComponent<ReviveManager>();

        if (hasHit) return;

        base.OnHit(hit);

        if (hit.collider.TryGetComponent(out IDamagableTeam idmg))
        {
            if (idmg?.Team == Teams.Player)
            {
                survivorPlayerData.Points += (int)PointsTable.Heal;
                //if (survivor.optionOneFirstChoice && hit.collider.GetComponent<SurvivorStatManager>().IsDown)
                //{
                //    reviveManager.StartCoroutine(reviveManager.BeingRevived());
                //}
                if (statusEffectsToApply.Count > 0)
                {
                    foreach (StatusEffectToApply statusEffectToApply in statusEffectsToApply)
                    {
                        hit.collider.transform.root.gameObject.GetComponent<StatusEffectManager>()?
                            .Svr_ApplyStatusEffect(statusEffectToApply.statusEffect.ApplyEffect(hit.collider.transform.root.gameObject));
                    }
                    Infection infect = (Infection)hit.collider.transform.root.gameObject.GetComponent<StatusEffectManager>().Svr_GetStatusEffect(statusEffect);
                    if (infect != null)
                    {
                        infect.infectionLevel -= 1;
                    }
                }
            }
            else
            {
                UnitBase ub = hit.collider.transform.root.gameObject.GetComponent<UnitBase>();

                if (!ub.TryGetComponent(out ZombieStronk stronk))
                {
                    idmg?.Svr_Damage(int.MaxValue, owner);

                    for (int i = 1; i < 4; i++)
                    {
                        ub.Dismember_BodyPart(i, (int)DamageTypes.Blunt);
                    }

                    survivorPlayerData.Points += (int)PointsTable.Kill;
                }
            }
        }



    }
}

