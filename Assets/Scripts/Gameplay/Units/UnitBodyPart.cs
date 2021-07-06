using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject attachedPart;
    public Transform partTransform;
    [Space]
    public GameObject childBloodEmitter;
    public GameObject bodyBloodEmitter;

    public int GetHealth => unitBase.Health;
    public bool IsDead => unitBase.isDead;

    #endregion


    public bool Detach(DamageTypes damageType)
    {
        if (!detachable) return false;
        if (unitBase.Svr_Dismember(damageType, attachedPart, partTransform.position, partTransform.rotation))
        {
            Collider[] cols = GetComponents<Collider>();
            foreach (Collider col in cols)
            {
                col.enabled = false;
            }

            //childBloodEmitter.SetActive(true);
            bodyBloodEmitter.SetActive(true);
            return true;
        }
        return false;
    }

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
            //detachedPart.GetComponent<Rigidbody>().isKinematic = true;
            //detachedPart.gameObject.SetActive(false);
            bodyBloodEmitter.SetActive(false);
        }
    }
    public Teams Team => throw new System.NotImplementedException();

    public Color ParticleColor => unitBase.ParticleColor;
}
