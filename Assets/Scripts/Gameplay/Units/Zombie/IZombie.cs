using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IZombie
{
    int InfectionAmount
    {
        get;set;
    }

    InfectionSO Infection
    {
        get;set;
    }

    void Infect(Transform target);
}
