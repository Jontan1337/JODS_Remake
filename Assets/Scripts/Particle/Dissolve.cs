using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : Timer
{
    public Material[] materials;

    public override void StartTimer(bool start, float _stopTime = 5f, Material[] mats = null)
    {
        materials = mats;

        base.StartTimer(start, _stopTime);
    }
    public override void StartTimer(bool start, float _stopTime = 5f)
    {
        materials[0] = GetComponent<MeshRenderer>().material;

        base.StartTimer(start, _stopTime);
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
