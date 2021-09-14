using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        print(Screen.currentResolution.refreshRate);
    }
}
