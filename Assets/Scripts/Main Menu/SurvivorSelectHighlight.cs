using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivorSelectHighlight : MonoBehaviour
{
    #region Singleton
    public static SurvivorSelectHighlight instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("UI")]
    [SerializeField] private Text survivorNameText = null;
    [SerializeField] private Text survivorDescriptionText = null;
    [SerializeField] private Text survivorSpecialText = null;
    [SerializeField] private GameObject descriptionGroup = null;
    [SerializeField] private GameObject specialGroup = null;
    private void Start()
    {
        ActivateUI(false);
    }
    public void Highlight(SurvivorSO so)
    {
        if (so != null)
        {
            ActivateUI(true);
            survivorNameText.text = so.survivorName;
            survivorDescriptionText.text = so.classDescription;
            survivorSpecialText.text = so.classSpecialDescription;
        }
        else
        {
            ActivateUI(false);
        }
    }
    private void ActivateUI(bool active)
    {
        survivorNameText.enabled = active;
        survivorDescriptionText.enabled = active;
        survivorSpecialText.enabled = active;

        descriptionGroup.SetActive(active);
        specialGroup.SetActive(active);
    }
}
