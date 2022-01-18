﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StatusEffectApplier : NetworkBehaviour
{
    [Header("Status Effect")]
    [SerializeField] private StatusEffectSO statusEffectToApply = null;

    [Header("Objects In Collider")]
    [SerializeField] private List<GameObject> objectsInCollider = new List<GameObject>();
    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        GameObject root = other.transform.root.gameObject;
        if (root.GetComponent<StatusEffectManager>())
        {
            objectsInCollider.Add(root);
            StartCoroutine(ApplyCoroutine(root));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!isServer) return;
        //Only the server may apply new status effects

        GameObject root = other.transform.root.gameObject;
        if (objectsInCollider.Contains(root))
        {
            objectsInCollider.Remove(root);
        }
    }

    private IEnumerator ApplyCoroutine(GameObject target)
    {
        StatusEffectManager manager = target.GetComponent<StatusEffectManager>();

        while (objectsInCollider.Contains(target))
        {
            //The server applies the status effect to the object every half second
            manager.Svr_ApplyStatusEffect(statusEffectToApply.ApplyEffect(target));

            yield return new WaitForSeconds(0.5f);
        }
    }
}
