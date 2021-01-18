using Mirror;
using System.Collections;
using UnityEngine;

public class Spit : UnitProjectile
{
    [Space]
    [SerializeField] private GameObject spitBall = null;
    [SerializeField] private ParticleSystem spitParticles;
    [SerializeField] private ParticleSystem trailParticles;
    public override void Destroy() 
    {
        //Overriding this function, because the spit particles still need to be visible after hitting something.

        //Disable the spitball visual
        spitBall.SetActive(false);

        //Stop all the particle systems from creating more particles
        spitParticles.Stop();
        trailParticles.Stop();

        GetComponent<SphereCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        StartCoroutine(DestroyEnumerator()); //Actually destroy the spit later
    }

    private IEnumerator DestroyEnumerator()
    {
        //Destroy the object when all the spit trails are gone
        yield return new WaitForSeconds(trailParticles.main.startLifetime.constant);
        NetworkServer.Destroy(gameObject);
    }
}
