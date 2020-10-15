using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lure : Grenade
{
    void Start()
    {
        countdown = delay;
    }

    void Update()
    {
        if (thrown)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0 && !boom)
            {
                Effect();
                boom = true;
            }
        }
    }

    void Effect()
    {
        explosionEffect = Instantiate(explosionEffect, transform.position, transform.rotation);
        Invoke("Remove", destroyDelay);
    }
}
