﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitBodyPart : MonoBehaviour, IDamagable, IDetachable, IParticleEffect
{
    private readonly float[] multiplierArray = new float[]
    {
        1f,  // Torso
        2f,  // Head
        0.6f,  // Arm
        0.75f   // Leg
    };
    private float multiplier = 0;


    #region Fields
    private UnitBase unitBase;

    public BodyParts bodyPart = BodyParts.Torso;

    public bool detachable = false;
    [Space]
    public bool onlyDetachOnDeath = false;
    [Space]
    public GameObject attachedPart;
    public Transform partTransform;
    [Space]
    public GameObject bodyBloodEmitter;

    public int GetHealth => unitBase.Health;
    public bool IsDead => unitBase.isDead;

    #endregion

    public void Detach(DamageTypes damageType)
    {
        if (!detachable) return;
        if (attachedPart == null || partTransform == null) return;

        if (CanDetach())
        {
            Collider[] cols = GetComponents<Collider>();
            foreach (Collider col in cols)
            {
                col.enabled = false;
            }
            bodyBloodEmitter.SetActive(true);

            Vector3 randomForce = new Vector3(Random.Range(-50, 50), Random.Range(-20, 20), Random.Range(-50, 50));

            SkinnedMeshRenderer oldSkinMeshRenderer = attachedPart.GetComponent<SkinnedMeshRenderer>();

            attachedPart.SetActive(false);

            GameObject newPart = ObjectPool.Instance.SpawnFromLocalPool(Tags.BodyPart, partTransform.position, partTransform.rotation, 8f);
            if (newPart == null)
            {
                Debug.LogError("No body part obtained from local pool");
                return;
            }
            newPart.transform.SetParent(null);
            newPart.GetComponent<MeshRenderer>().material = new Material(oldSkinMeshRenderer.sharedMaterial);
            newPart.GetComponent<MeshFilter>().mesh = oldSkinMeshRenderer.sharedMesh;
            newPart.GetComponent<MeshCollider>().sharedMesh = oldSkinMeshRenderer.sharedMesh;
            
            //newPart.GetComponent<Dissolve>().StartTimer(true, 5, 3);

            if (newPart.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.AddForce(randomForce / 2);
                rb.AddTorque(randomForce);
            }
        }
    }

    private bool CanDetach()
    {
        return (onlyDetachOnDeath && IsDead || !onlyDetachOnDeath);
    }

    public GameObject GetUnitBase() => unitBase.gameObject;

    public void Svr_Damage(int damage, Transform target = null)
    {
        unitBase.Svr_Damage(Mathf.RoundToInt(damage * multiplier), target);
    }

	void Start()
    {
        unitBase = transform.root.GetComponent<UnitBase>();

        multiplier = multiplierArray[(int)bodyPart];

        if (detachable)
        {
            //Dismemberment default states
            attachedPart.SetActive(true);
            bodyBloodEmitter.SetActive(false);
        }
    }

    public Teams Team => throw new System.NotImplementedException();

    public Color ParticleColor => unitBase.ParticleColor;
}
