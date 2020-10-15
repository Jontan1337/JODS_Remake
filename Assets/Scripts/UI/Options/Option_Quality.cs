using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option_Quality : MonoBehaviour
{
    private Dropdown d;
    // Start is called before the first frame update
    void Start()
    {
        d = GetComponent<Dropdown>();
        List<string> names = new List<string>();
        foreach (string n in QualitySettings.names)
        {
            names.Add(n);
        }
        //names.Reverse();
        d.AddOptions(names);
        d.value = QualitySettings.GetQualityLevel();
    }
    public void ChangeQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
        Debug.Log("Quality set to " + QualitySettings.names[quality]);
    }
}
