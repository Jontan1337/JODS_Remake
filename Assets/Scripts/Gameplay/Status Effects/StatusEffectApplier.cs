using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectApplier : MonoBehaviour
{
    [Header("Status Effect")]
    [SerializeField] private StatusEffectSO statusEffectToApply = null;

    [Header("Objects In Collider")]
    [SerializeField] private List<GameObject> objectsInCollider = new List<GameObject>();
    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;
        if (root.GetComponent<StatusEffectManager>())
        {
            objectsInCollider.Add(root);
            StartCoroutine(ApplyCoroutine(root));
        }
    }
    private void OnTriggerExit(Collider other)
    {
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
            manager.ApplyStatusEffect(statusEffectToApply.ApplyEffect(target));
            yield return new WaitForSeconds(0.5f);
        }
    }
}
