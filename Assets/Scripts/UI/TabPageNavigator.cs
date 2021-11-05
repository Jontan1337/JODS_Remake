using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabPageNavigator : MonoBehaviour
{
    public List<Transform> tabPageNavigators = new List<Transform>();
    public List<Transform> tabPages = new List<Transform>();

    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    private void Awake()
    {
        //foreach (Transform button in tabPageNavigators)
        //{
        //    button.GetComponent<Button>().onClick.AddListener(delegate(UnityEngine.Events.UnityAction e) { NavigateTo(e); });
        //}
    }

    public void NavigateTo(Transform tabButton)
    {
        DeactivateAllPages();
        DeselectAllPageNavigators();
        //tabPageNavigators[tabButton.GetSiblingIndex()].gameObject.SetActive(true);
        tabPages[tabButton.GetSiblingIndex()].gameObject.SetActive(true);
    }

    private void DeselectAllPageNavigators()
    {
        //foreach (Transform navigator in tabPageNavigators)
        //{
        //    navigator.GetComponent<Button>()..colors.normalColor = inactiveColor;
        //}
    }

    private void DeactivateAllPages()
    {
        foreach (Transform page in tabPages)
        {
            page.gameObject.SetActive(false);
        }
    }
}
