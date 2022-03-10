using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingManager : MonoBehaviour
{
    public static PostProcessingManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public Volume GetVolume => GetComponent<Volume>();
}
