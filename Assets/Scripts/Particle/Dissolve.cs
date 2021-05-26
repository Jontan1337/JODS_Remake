using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : Timer
{
    private Material material;
    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
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
