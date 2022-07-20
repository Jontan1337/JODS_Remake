using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Syringe : Projectile
{
    [SerializeField] StatusEffectSO statusEffectToRemove = null;
    public override void Start()
    {
        base.Start();
        objectPoolTag = Tags.Syringe;
        transform.Rotate(new Vector3(90, 0, 0));
        StartCoroutine(LifeTime());
    }
    [Server]
    public override void OnHit(Collision hit)
    {
        base.OnHit(hit);
        IDamagable idmg = hit.collider.GetComponent<IDamagable>();
        if (idmg?.Team == Teams.Player)
        {
            if (statusEffectsToApply.Count > 0)
            {
                foreach (StatusEffectToApply statusEffectToApply in statusEffectsToApply)
                {
                    hit.collider.transform.root.gameObject.GetComponent<StatusEffectManager>()?
                        .Svr_ApplyStatusEffect(statusEffectToApply.statusEffect.ApplyEffect(hit.collider.transform.root.gameObject));
                }
            }

        }
        else
        {
            idmg?.Svr_Damage(damage, owner);
        }
        hit.collider.transform.root.gameObject.GetComponent<StatusEffectManager>()?.Svr_RemoveStatusEffect(statusEffectToRemove);
    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(5f);
        Destroy();
    }
}

