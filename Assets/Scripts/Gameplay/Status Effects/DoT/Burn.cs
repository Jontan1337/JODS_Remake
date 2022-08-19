using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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
        onFire = true;
        onFireParticles = GameObject.Instantiate(onFireParticlesPrefab);
        NetworkServer.Spawn(onFireParticles);
        onFireParticles.transform.SetParent(obj.transform, false);
    }

    public override void Svr_OnEffectApplied()
    {
        if (burnParticlesPrefab == null) return;
        burnParticles = GameObject.Instantiate(burnParticlesPrefab);
        NetworkServer.Spawn(burnParticles);
        burnParticles.transform.SetParent(obj.transform, false);
    }
    public override void Svr_ApplyEffect(int? amount)
    {
        if (amount > 0) duration = (float)amount;
        if (amount > 5 && !onFire)
        {
            SetOnFire();
        }
        idmg.Svr_Damage(onFire ? burn.onFireDamagePerTick : burn.damagePerTick);
    }

    public override void Svr_End() 
    {
        GameObject.Destroy(burnParticles);
        if (onFireParticles) GameObject.Destroy(onFireParticles);

    }

    public override void Svr_Tick()
    {
        if (duration > 5 && !onFire)
        {       
            SetOnFire();
        }

        base.Svr_Tick();
        //Do Damage
        idmg.Svr_Damage(onFire ? burn.onFireDamagePerTick : burn.damagePerTick);
    }
    public override float GetImageAlpha() => onFire ? 200 : 90;
}
