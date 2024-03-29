﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    private static GameSettings instance;

    [Header("Controls")]
    public ItemPickupBehaviour pickupBehaviour;
    [Header("Controls")]
    public float mouseSensitivity = 1f;
    public float mouseAcceleration = 0f;
    public float mouseMaxAcceleration = 10f;
    public float mouseEasingSpeed = 500f;

    public static Action<ItemPickupBehaviour> onItemPickupBehaviourChanged;
    public static Action<float> onMouseSensitivityChanged;
    public static Action<float> onMouseAccelerationChanged;
    public static Action<float> onMouseMaxAccelerationChanged;
    public static Action<float> onMouseEasingSpeedChanged;

    public static GameSettings Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (transform == transform.root) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }

    public void SetPickupBehaviour(int index)
    {
        pickupBehaviour = (ItemPickupBehaviour) index;
        onItemPickupBehaviourChanged?.Invoke(pickupBehaviour);
    }

    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
        onMouseSensitivityChanged?.Invoke(value);
    }
    public void SetMouseAcceleration(float value)
    {
        mouseAcceleration = value;
        onMouseAccelerationChanged?.Invoke(value);
    }
    public void SetMouseMaxAcceleration(float value)
    {
        mouseMaxAcceleration = value;
        onMouseMaxAccelerationChanged?.Invoke(value);
    }
    public void SetMouseEasingSpeed(float value)
    {
        mouseEasingSpeed = value;
        onMouseEasingSpeedChanged?.Invoke(value);
    }
}
