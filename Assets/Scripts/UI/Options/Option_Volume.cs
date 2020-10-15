using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option_Volume : MonoBehaviour
{
    private Slider v;
    void Start()
    {
        v = GetComponent<Slider>();
        v.value = AudioListener.volume;
    }
    private void Update()
    {
        AudioListener.volume = v.value;
    }
}
