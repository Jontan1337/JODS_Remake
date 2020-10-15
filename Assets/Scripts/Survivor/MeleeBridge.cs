using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBridge : MonoBehaviour
{
    public Shoot s;
    public void Melee()
    {
        s.MeleeAttack();
    }
}
