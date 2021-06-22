using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DestroyAfterTime : MonoBehaviour
{
    public float time;
    void Start()
    {
        //StartCoroutine(DestroyUnit(time));
    }

    [Server]
	public void Svr_Destroy(float time)
	{
        StartCoroutine(DestroyUnit(time));
	}

	IEnumerator DestroyUnit(float time)
    {
        yield return new WaitForSeconds(time);

        NetworkServer.Destroy(gameObject);
    }
}
