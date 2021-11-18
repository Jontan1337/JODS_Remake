using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class JODSSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Text text;

    private const string decimal2 = "n2";

    private void Awake()
    {
        text.text = $"{slider.value.ToString(decimal2)}";
    }

    public void onSliderValueChanged(float value)
    {
        text.text = $"{value.ToString(decimal2)}";
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
