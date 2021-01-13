using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRandomMaterials : MonoBehaviour
{
    public Material[] shirtMats;
    public int shirtNum;
    public Material[] pantsMats;
    public int pantsNum;
    public Material[] skinMats;
    public int skinNum;
    public SkinnedMeshRenderer meshRenderer;

    void Start()
    {
        Material[] mats = meshRenderer.materials;
        if (shirtMats.Length != 0)
        {
            bool change = (Random.value >= 0.2f);
            if (change)
            {
                mats[shirtNum] = shirtMats[Random.Range(0, shirtMats.Length)];
            }
        }
        if (pantsMats.Length != 0)
        {
            bool change = (Random.value >= 0.2f);
            if (change)
            {
                mats[pantsNum] = pantsMats[Random.Range(0, pantsMats.Length)];
            }
        }
        if (skinMats.Length != 0)
        {
            bool change = (Random.value >= 0.2f);
            if (change)
            {
                mats[skinNum] = skinMats[Random.Range(0, skinMats.Length)];
            }
        }
        meshRenderer.materials = mats;
    }
}
