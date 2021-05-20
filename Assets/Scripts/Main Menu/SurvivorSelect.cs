using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorSelect : MonoBehaviour
{
    public SurvivorSO survivor;
    [Space]
    [SerializeField] private bool selected;
    [Header("Visual")]
    [SerializeField] private GameObject selectedVisual;

    private void Start()
    {
        Select(false);
    }
    public void Select(bool value)
    {
        selected = value;
        selectedVisual.SetActive(value);
    }
    public void Select()
    {
        bool value = !selected;
        selected = value;
        selectedVisual.SetActive(value);
    }
}
