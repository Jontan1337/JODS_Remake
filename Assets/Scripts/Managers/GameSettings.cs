using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    private static GameSettings instance;

    [Header("Controls")]
    public float mouseSensitivity = 1f;
    public float mouseAcceleration = 0f;
    public float maxMouseAcceleration = 10f;
    public float mouseEasingSpeed = 500f;

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
    public void SetMouseMaxAcceleration(float value)
    {
        maxMouseAcceleration = value;
    }
    public void SetMouseEasingSpeed(float value)
    {
        mouseEasingSpeed = value;
    }
}
