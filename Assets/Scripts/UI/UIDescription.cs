using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDescription : MonoBehaviour
{
    [Header("Description")]
    [SerializeField] private string title = "";
    [SerializeField, TextArea(2,10)] private string description = "";

    [Header("References")]
    [SerializeField] private Text titleText = null;
    [SerializeField] private Text descriptionText = null;

    public void ShowDescription()
    {
        titleText.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(true);

        titleText.text = title;
        descriptionText.text = description;
    }

    public void HideDescription()
    {
        titleText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);

        titleText.text = title;
        descriptionText.text = description;
    }
}
