﻿using UnityEngine;

public class JODSInput : MonoBehaviour
{
    public static Controls Controls { get; private set; }

    private void Awake()
    {
        Controls = new Controls();
    }
}
