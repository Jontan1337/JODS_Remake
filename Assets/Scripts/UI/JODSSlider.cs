using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JODSSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Text text;

    public void onSliderValueChanged(float value)
    {
        text.text = $"{value.ToString("n2")}";
    }

    public void onTextChanged(string value)
    {
        try
        {
            slider.value = Convert.ToInt32(value);
        }
        catch (Exception e)
        {
            slider.value = float.NaN;
            Debug.LogWarning($"Error: {e.Message}");
        }
    }
}
