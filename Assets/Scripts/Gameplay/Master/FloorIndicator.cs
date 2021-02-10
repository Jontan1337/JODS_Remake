using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorIndicator : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Color deselected;
    [SerializeField] private Color selected;

    public void Select(bool select)
    {
        image.color = select ? selected : deselected;
    }
}
