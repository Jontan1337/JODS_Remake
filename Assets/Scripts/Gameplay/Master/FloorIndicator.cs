using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorIndicator : MonoBehaviour
{
    [SerializeField] private Image image = null;
    [SerializeField] private Color deselected = Color.red;
    [SerializeField] private Color selected = Color.green;

    public void Select(bool select)
    {
        image.color = select ? selected : deselected;
    }
}
