using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorSelector : MonoBehaviour
{
    #region Singleton
    public static SurvivorSelector instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField] private bool canSelect = false;


    public bool CanSelect
    {
        get { return canSelect; }
        set { 
            canSelect = value;
            if (value == true)
            {
                RaycastCoroutine = StartCoroutine(SelectCoroutine());
            }
            else if (value == false) StopCoroutine(RaycastCoroutine);
        }
    }

    private Coroutine RaycastCoroutine;
    private IEnumerator SelectCoroutine()
    {
        RaycastHit hit;

        while (canSelect)
        {

            yield return new WaitForSeconds(0.1f);
        }
    }

}
