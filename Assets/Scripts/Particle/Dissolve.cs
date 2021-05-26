using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : Timer
{
    private Material material;

    public override void StartTimer(bool start, float _stopTime = 5f, Material mat = null)
    {
        material = mat;

        base.StartTimer(start, _stopTime);
    }
    public override void StartTimer(bool start, float _stopTime = 5f)
    {
        material = GetComponent<MeshRenderer>().material;

        base.StartTimer(start, _stopTime);
    }

    protected override void Tick()
    {
        material.SetFloat("_DissolveAmount", timerProgress / 100);
    }

    protected override void Finish()
    {
        Destroy(gameObject);
    }
}
