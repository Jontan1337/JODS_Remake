using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlaceholder : MonoBehaviour
{
    public Material material;
    public MeshRenderer[] renderers;
    public bool obstructed;
    // Start is called before the first frame update
    void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer r in renderers)
        {
            var mats = r.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = material;
            }
            r.materials = mats;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        obstructed = true;
        ChangeColor(obstructed);
    }
    private void OnTriggerExit(Collider other)
    {
        obstructed = false;
        ChangeColor(obstructed);
    }
    public void ChangeColor(bool obs)
    {
        material.SetInt("_IsObstructed", obs? 1 : 0);
    }
}
