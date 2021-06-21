using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBodyPart : MonoBehaviour, IDamagable, IDetachable
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
    public UnitBase unitBase;

    public BodyParts bodyPart = BodyParts.Torso;

    public bool detachable = false;
    [Space]
    public GameObject attachedPart;
    public GameObject detachedPart;
    #endregion


    public void Detach(DamageTypes damageType)
    {
        if (!detachable) return;
        unitBase.Svr_Dismember(damageType, attachedPart, detachedPart);
    }

    public void Svr_Damage(int damage, Transform target = null)
    {
        unitBase.Svr_Damage(Mathf.RoundToInt(damage * multiplier), target);
    }

	int IDamagable.GetHealth()
	{
        return unitBase.GetHealth();
	}

	bool IDamagable.IsDead()
	{
        return unitBase.IsDead();
	}

	void Start()
    {
        unitBase = transform.root.GetComponent<UnitBase>();

        multiplier = multiplierArray[(int)bodyPart];

        if (detachable)
        {
            //Dismemberment default states
            attachedPart.SetActive(true);
            detachedPart.GetComponent<Rigidbody>().isKinematic = true;
            detachedPart.gameObject.SetActive(false);
        }
    }
    public Teams Team => throw new System.NotImplementedException();

}
