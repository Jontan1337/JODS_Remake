using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frag : Grenade
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
                Explode();
                boom = true;
            }
        }
    }
}
