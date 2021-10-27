using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : Timer
{
    public Material[] materials;

    [Header("Dissolve Settings")]
    [SerializeField] private bool getObjectMaterial = false;
    [SerializeField] private bool destroyOnFinish = false;

    public override void Start()
    {
        if (getObjectMaterial)
        {
            materials = GetComponent<MeshRenderer>().materials;
        }
        base.Start();
    }

    public override void StartTimer(bool start, float _stopTime = 5f, float delay = 0, Material[] mats = null)
    {
        materials = mats;

        SetMaterialDefaults();

        base.StartTimer(start, _stopTime, delay);
    }
    public override void StartTimer(bool start, float _stopTime = 5f, float delay = 0)
    {
        
        materials = GetComponent<MeshRenderer>().materials;

        SetMaterialDefaults();

        base.StartTimer(start, _stopTime, delay);
    }

    private void SetMaterialDefaults()
    {
        foreach (Material material in materials)
        {
            if (material == null) continue;
            material.SetInt("_Dissolve", 1);
            material.SetFloat("_DissolveAmount", 0);
        }
    }

    protected override void Tick()
    {
        foreach (Material material in materials)
        {
            if (material == null) continue;
            material.SetFloat("_DissolveAmount", timerProgress / 100);
        }
    }

    protected override void Finish()
    {
        if (destroyOnFinish) { Destroy(gameObject); }
    }
}
