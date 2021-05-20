using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("UI")]
    [SerializeField] private Text survivorNameText;
    [SerializeField] private Text survivorDescriptionText;
    [SerializeField] private Text survivorSpecialText;


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
        while (canSelect)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.TryGetComponent(out SurvivorSelect select))
                {
                    survivorNameText.text = select.survivor.survivorName;
                    survivorDescriptionText.text = select.survivor.classDescription;
                    survivorSpecialText.text = select.survivor.classSpecialDescription;
                }
                else
                {
                    survivorNameText.text = "";
                    survivorDescriptionText.text = "";
                    survivorSpecialText.text = "";
                }
            }
            else
            {
                survivorNameText.text = "";
                survivorDescriptionText.text = "";
                survivorSpecialText.text = "";
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

}
