using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixelade : Grenade
{
    private Animator ani;
    void Start()
    {
        countdown = delay;
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (thrown)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0)
            {
                ani.SetTrigger("Boom");
                Invoke("Remove", destroyDelay);
            }
        }
    }
}
