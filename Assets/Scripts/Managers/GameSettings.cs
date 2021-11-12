using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    private static GameSettings instance;

    [Header("Controls")]
    public float mouseSensitivity = 1f;
    public float mouseAcceleration = 0f;
    public float maxMouseAcceleration = 0f;
    public float mouseEasingSpeed = 0f;

    public static GameSettings Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        print(Screen.currentResolution.refreshRate);
    }

    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
    }
    public void SetMouseAcceleration(float value)
    {
        mouseAcceleration = value;
    }
    public void SetMaxMouseAcceleration(float value)
    {
        maxMouseAcceleration = value;
    }
    public void SetMouseEasingSpeed(float value)
    {
        mouseEasingSpeed = value;
    }
}
