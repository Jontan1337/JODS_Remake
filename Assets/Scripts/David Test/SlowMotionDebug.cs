using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionDebug : MonoBehaviour
{
    private bool slowMo = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            slowMo = !slowMo;
            Time.timeScale = slowMo ? 0.1f : 1f;
        }
    }
}
