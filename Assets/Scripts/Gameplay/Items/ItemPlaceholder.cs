using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlaceholder : MonoBehaviour
{
    public Material canPlace;
    public Material cannotPlace;
    public MeshRenderer[] renderers;
    public bool obstructed;


    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        renderers = GetComponentsInChildren<MeshRenderer>(true);
        
        foreach (MeshRenderer r in renderers)
        {
            var mats = r.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = canPlace;
            }
            r.materials = mats;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        obstructed = true;
        ChangeMaterials(obstructed);
    }
    private void OnTriggerExit(Collider other)
    {
        obstructed = false;
        ChangeMaterials(obstructed);
    }
    public void ChangeMaterials(bool obs)
    {
        foreach (MeshRenderer r in renderers)
        {
            var mats = r.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = obs ? cannotPlace : canPlace;
            }
            r.materials = mats;
        }
    }
}
