using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option_Sensitivity : MonoBehaviour
{
    private Slider s;
    // Start is called before the first frame update
    void Start()
    {
        s = GetComponent<Slider>();
        s.value = PlayerPrefs.GetFloat("Mouse Sensitivity"); 
    }

    public void ChangeSensitivity()
    {
        PlayerPrefs.SetFloat("Mouse Sensitivity", s.value);
    }
}
