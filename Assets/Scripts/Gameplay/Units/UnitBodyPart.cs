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
    private float[] multiplierArray = new float[]
    {
        1f,  // Torso
        2f,  // Head
        0.6f,  // Arm
        0.75f   // Leg
    };

    private float multiplier = 0;

    [SerializeField]
    private BodyPart bodyPart;

    private UnitBase unitBase;

    public Teams Team => throw new System.NotImplementedException();

    public void Svr_Damage(int damage)
    {
        unitBase.Svr_Damage(Mathf.RoundToInt(damage * multiplier));
    }

    void Start()
    {
        unitBase = transform.root.GetComponent<UnitBase>();

        multiplier = multiplierArray[(int)bodyPart];
    }

}
