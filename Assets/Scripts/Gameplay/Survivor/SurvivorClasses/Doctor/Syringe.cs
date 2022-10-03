﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Syringe : Projectile
{
    private Survivor survivor;
    [SerializeField] StatusEffectSO statusEffect = null;
    public override void Start()
    {
        base.Start();
        objectPoolTag = Tags.Syringe;
        transform.Rotate(new Vector3(90, 0, 0));
        survivor = owner.GetComponent<Survivor>();
        //StartCoroutine(LifeTime());
    }
    [Server]
    public override void OnHit(Collision hit)
    {
        var reviveManager = hit.collider.transform.root.gameObject.GetComponent<ReviveManager>();
        base.OnHit(hit);
        if (hit.collider.TryGetComponent(out IDamagableTeam idmg))
        {
            if (idmg?.Team == Teams.Player)
            {
                if (survivor.optionOneFirstChoice && GetComponent<SurvivorStatManager>().IsDown)
                {
                    reviveManager.StartCoroutine(reviveManager.BeingRevived());
                }
                else if (statusEffectsToApply.Count > 0)
                {
                    foreach (StatusEffectToApply statusEffectToApply in statusEffectsToApply)
                    {
                        hit.collider.transform.root.gameObject.GetComponent<StatusEffectManager>()?
                            .Svr_ApplyStatusEffect(statusEffectToApply.statusEffect.ApplyEffect(hit.collider.transform.root.gameObject));
                    }


                    Infection infect = (Infection)hit.collider.transform.root.gameObject.GetComponent<StatusEffectManager>()?.Svr_GetStatusEffect(statusEffect);

                    infect.infectionLevel -= 1;
                }

            }
            else
            {
                UnitBase ub = hit.collider.transform.root.gameObject.GetComponent<UnitBase>();
                BaseStatManager baseStatManager = hit.collider.transform.root.gameObject.GetComponent<BaseStatManager>();

                if (!ub.TryGetComponent(out ZombieStronk stronk))
                {
                    for (int i = 1; i < 4; i++)
                    {
                        ub.Dismember_BodyPart(i, (int)DamageTypes.Blunt);
                    }
                    idmg?.Svr_Damage(baseStatManager.Health, owner);
                }


            }
        }



    }

    //IEnumerator LifeTime()
    //{
    //    yield return new WaitForSeconds(5f);
    //    Destroy();
    //}
}

