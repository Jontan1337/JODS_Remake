using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private List<Camera> playerCameras = null;
    [SerializeField] private UniversalAdditionalCameraData universalCamData;
    [SerializeField] private bool mainCamera;
    [SerializeField] private float fieldOfView = 60f;

    public float FieldOfView { get => fieldOfView; }

    private void OnValidate()
    {
        SetFOV(fieldOfView, 0f);
    }

    public void SetFOV(float value, float duration)
    {
        //playerCamFOV = value;
        foreach (Camera camera in playerCameras)
        {
            camera.DOFieldOfView(value, duration);
        }
    }
}
