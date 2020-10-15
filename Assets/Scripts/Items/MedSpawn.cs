using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedSpawn : MonoBehaviour
{
    public Medkit med;
    public bool big;

    private void Start()
    {
        med.gameObject.SetActive(false);
        big = med.big;
    }

    public void SpawnMed()
    {
        if (med.gameObject.activeSelf)
        {
            return;
        }

        med.gameObject.SetActive(true);
        med.uses = big ? Random.Range(3,5) : 1;
    }
}
