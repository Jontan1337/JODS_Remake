using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public float timeToLive = 1.5F;
    private float tTime;
    public ParticleSystem[] gas;
    [SerializeField] private bool invokeStopInStart = true;
    [SerializeField] private bool invokeStopInParentChanged = false;

    void Start()
    {
        if (invokeStopInStart)
            Invoke(nameof(Stop), timeToLive);
    }

    private void OnTransformParentChanged()
    {
        if (invokeStopInParentChanged)
            Invoke(nameof(Stop), timeToLive);
    }

    //Stops every particle from emissioning. RE: "emissionning"?! Heck???
    void Stop()
    {
        if (gameObject.tag == "Lure")
        {
            for (int i = 0; i < gas.Length; i++)
            {
                var emission = gas[i].emission;
                emission.enabled = false;
                Invoke("Remove", 10F);
            }
        }
        else
        {
            Remove();
        }
    }

    //Destroys itself
    void Remove()
    {
        Destroy(gameObject);
    }
}
