using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option_Volume : MonoBehaviour
{
    public void SetVolume(float value)
    {
        AudioListener.volume = value;
    }
}
