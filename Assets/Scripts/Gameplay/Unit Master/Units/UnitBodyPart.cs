using System.Collections;
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

    public void OnDetach(int damageTypeInt)
    {
        if (!detachable) return;
        if (attachedPart == null || partTransform == null) return;

        DamageTypes damageType = (DamageTypes)damageTypeInt;

        Collider[] cols = GetComponents<Collider>();
        foreach (Collider col in cols)
        {
            col.enabled = false;
        }
        bodyBloodEmitter.SetActive(true);

        Vector3 randomForce = new Vector3(Random.Range(-50, 50), Random.Range(-20, 20), Random.Range(-50, 50));

        SkinnedMeshRenderer oldSkinMeshRenderer = attachedPart.GetComponent<SkinnedMeshRenderer>();

        attachedPart.SetActive(false);

        Quaternion rotation = partTransform.rotation;
        switch (damageType)
        {
            case DamageTypes.Blunt:
                GameObject bloodSplatter = ObjectPool.Instance.SpawnFromLocalPool(Tags.HeadExplosionBloodSplatter, partTransform.position, rotation, 8f);
                if (bloodSplatter == null)
                {
                    Debug.LogError("No blood splatter obtained from local pool");
                    return;
                }
                break;
            case DamageTypes.Pierce:
                bloodSplatter = ObjectPool.Instance.SpawnFromLocalPool(Tags.HeadExplosionBloodSplatter, partTransform.position, rotation, 8f);
                if (bloodSplatter == null)
                {
                    Debug.LogError("No blood splatter obtained from local pool");
                    return;
                }
                break;
            case DamageTypes.Slash:
                GameObject newPart = ObjectPool.Instance.SpawnFromLocalPool(Tags.BodyPart, partTransform.position, rotation, 8f);
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
                break;
        }
    }
    public void Detach(int damageType)
    {
        if (CanDetach())
        {
            unitBase.Dismember_BodyPart((int)bodyPart, damageType);
        }
    }

    private bool CanDetach()
    {
        return (onlyDetachOnDeath && IsDead || !onlyDetachOnDeath);
    }


    public void Svr_Damage(int damage, Transform target = null)
    {
        unitBase.Svr_Damage(Mathf.RoundToInt(damage * multiplier), target);
    }

    public void Cmd_Damage(int damage)
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {
        unitBase = GetComponentInParent<UnitBase>();

        multiplier = multiplierArray[(int)bodyPart];

        if (detachable)
        {
            //Dismemberment default states
            attachedPart.SetActive(true);
            bodyBloodEmitter.SetActive(false);
        }
    }

    public Teams Team => Teams.Unit;

    public Color ParticleColor => unitBase.ParticleColor;
}
