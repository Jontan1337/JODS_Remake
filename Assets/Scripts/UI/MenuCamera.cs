using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class MenuCamera : MonoBehaviour
{
    private PostProcessingProfile PP;
    void Start()
    {
        GetComponent<PostProcessingBehaviour>().enabled = true;
        PP = GetComponent<PostProcessingBehaviour>().profile;
        ColorGradingModel.Settings col = PP.colorGrading.settings;
        col.basic.hueShift = 0;
        PP.colorGrading.settings = col;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ColorGradingModel.Settings col = PP.colorGrading.settings;
            col.basic.hueShift = Random.Range(-180, 180);
            PP.colorGrading.settings = col;
        }
    }
}
