using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour
{
    public int uses;
    public bool big;

	public float UseMedkit(float healthPoints)
	{
        if (big)
        {
            if (healthPoints != 100)
            {
                healthPoints = healthPoints >= 31 ? 100 : healthPoints + 69;
                uses--;
            }
            if (uses <= 0)
            {
                gameObject.SetActive(false);
            }
            return healthPoints;
        }
        else
        {
            if (healthPoints != 100)
            {
                healthPoints = healthPoints >= 58 ? 100 : healthPoints + 42;
                uses--;
            }
            if (uses <= 0)
            {
                gameObject.SetActive(false);
            }
            return healthPoints;
        }	
	}
}
