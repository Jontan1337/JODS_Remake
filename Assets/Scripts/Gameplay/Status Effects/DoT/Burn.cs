using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : StatusEffect
{
    private IDamagable idmg;
    private BurnSO burn;
    private bool onFire = false;
    private GameObject burnParticlesPrefab;
    private GameObject burnParticles;
    private GameObject onFireParticlesPrefab;
    private GameObject onFireParticles;
    public Burn(StatusEffectSO effect, GameObject obj) : base(effect, obj)
    {
        idmg = obj.GetComponent<IDamagable>();
        burn = (BurnSO)effect;
        burnParticlesPrefab = burn.burnParticles;
        onFireParticlesPrefab = burn.onFireParticles;
    }
    private void SetOnFire()
    {
        if (onFireParticlesPrefab == null) return;
        onFireParticles = GameObject.Instantiate(onFireParticlesPrefab, obj.transform);
    }

    public override void OnEffectApplied()
    {
        if (burnParticlesPrefab == null) return;
        burnParticles = GameObject.Instantiate(burnParticlesPrefab, obj.transform);
    }
    public override void ApplyEffect(int? amount)
    {
        idmg.Svr_Damage(onFire ? burn.onFireDamagePerTick : burn.damagePerTick);
    }

    public override void End() 
    {
        GameObject.Destroy(burnParticles);
        if (onFireParticles) GameObject.Destroy(onFireParticles);
    }

    public override void Tick()
    {
        if (duration > 5 && !onFire)
        {
            onFire = true;
            SetOnFire();
        }

        base.Tick();
        //Do Damage
        idmg.Svr_Damage(onFire ? burn.onFireDamagePerTick : burn.damagePerTick);
    }
}
