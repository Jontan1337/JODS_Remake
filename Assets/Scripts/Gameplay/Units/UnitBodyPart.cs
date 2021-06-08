using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyPart
{
    Torso,
    Head,
    Arm,
    Leg
}
public class UnitBodyPart : MonoBehaviour, IDamagable
{
    private readonly float[] multiplierArray = new float[]
    {
        1f,  // Torso
        2f,  // Head
        0.6f,  // Arm
        0.75f   // Leg
    };

    private float multiplier = 0;

    [SerializeField]
    private BodyPart bodyPart = BodyPart.Torso;

    private UnitBase unitBase;

    public Teams Team => throw new System.NotImplementedException();

    public void Svr_Damage(int damage, Transform target = null)
    {
        unitBase.Svr_Damage(Mathf.RoundToInt(damage * multiplier));
    }

	int IDamagable.GetHealth()
	{
		throw new System.NotImplementedException();
	}

	bool IDamagable.IsDead()
	{
		throw new System.NotImplementedException();
	}

	void Start()
    {
        unitBase = transform.root.GetComponent<UnitBase>();

        multiplier = multiplierArray[(int)bodyPart];
    }

}
