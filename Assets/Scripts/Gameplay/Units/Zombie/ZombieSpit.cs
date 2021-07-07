using Mirror;
using System.Collections;
using UnityEngine;

public class ZombieSpit : Projectile
{
    [Header("Spit Visuals")]
    [SerializeField] private GameObject spitBall = null;
    [SerializeField] private ParticleSystem spitParticles = null;
    [SerializeField] private ParticleSystem trailParticles = null;
    [SerializeField] private ParticleSystem[] hitParticles = null;

	public override void Start()
	{
        base.Start();
        objectPoolTag = "Spit";
	}

	public override void OnHit(Collision objectHit)
    {
        if (!piercing && !hasHit)
        {
            hasHit = true;

            Damage(objectHit.collider.gameObject); //Damage the object hit

            objectHit.collider.GetComponent<StatusEffectManager>()?.ApplyStatusEffect(statusEffectToApply.ApplyEffect(objectHit.collider.gameObject), amount); //apply DOT effect

            SpitEffects(); //Apply visual effects

            StartCoroutine(DestroyEnumerator()); //Actually destroy the spit later
        }
    }

    private void SpitEffects()
    {
        //Disable the spitball visual
        spitBall.SetActive(false);

        //Stop all the particle systems from creating more particles
        spitParticles.Stop();
        trailParticles.Stop();
        //Play the Hit Particles
        foreach (var particle in hitParticles)
        {
            particle.Play();
        }

        //Do this
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private IEnumerator DestroyEnumerator()
    {
        float time = trailParticles.main.startLifetime.constant;

        yield return new WaitForSeconds(time);

        //Destroy the object when all the spit trails are gone (time)
        ReturnObjectToPool(0);
    }
}
