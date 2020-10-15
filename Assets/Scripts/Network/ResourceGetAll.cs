using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ResourceGetAll : MonoBehaviour
{
    private NetworkManager nm = null;

    private void Start()
    {
        GetResources();
    }

    public void GetResources()
    {
        nm = GetComponent<NetworkManager>();
        nm.spawnPrefabs.Clear();
        Object[] spawnables = Resources.LoadAll("Spawnables",typeof(GameObject));
        foreach (var s in spawnables) {
            nm.spawnPrefabs.Add((GameObject)s);
                }
    }
}