using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DestroyAfterTime : MonoBehaviour
{
    public float time;
    void Start()
    {
        Destroy(gameObject, time);
        //StartCoroutine(DestroyUnit(time));
    }

    IEnumerator DestroyUnit(float tiem)
    {
        yield return new WaitForSeconds(tiem);

        NetworkServer.Destroy(gameObject);
    }
}
