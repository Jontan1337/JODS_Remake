using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : Timer
{
    public Material[] materials;

    [Header("Dissolve Settings")]
    [SerializeField] private bool getObjectMaterial = false;

    public override void Start()
    {
        if (getObjectMaterial)
        {
            materials = GetComponent<MeshRenderer>().sharedMaterials;
        }
        base.Start();
    }

    public override void StartTimer(bool start, float _stopTime = 5f, float delay = 0, Material[] mats = null)
    {
        materials = mats;

        foreach (Material material in materials)
        {
            material.SetInt("_Dissolve", 1);
        }

        base.StartTimer(start, _stopTime, delay);
    }
    public override void StartTimer(bool start, float _stopTime = 5f, float delay = 0)
    {
        materials = GetComponent<MeshRenderer>().sharedMaterials;

        foreach (Material material in materials)
        {
            material.SetInt("_Dissolve", 1);
        }

        base.StartTimer(start, _stopTime, delay);
    }

    protected override void Tick()
    {
        foreach (Material material in materials)
        {
            material.SetFloat("_DissolveAmount", timerProgress / 100);
        }
    }

    protected override void Finish()
    {
        Destroy(gameObject);
    }
}
