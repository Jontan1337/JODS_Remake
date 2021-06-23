using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private List<Camera> playerCameras = null;
    [SerializeField] private UniversalAdditionalCameraData universalCamData;
    [SerializeField] private bool mainCamera;
    [SerializeField] private float playerCamFOV = 60f;

    private void OnValidate()
    {
        SetFOV(playerCamFOV);
    }

    public void SetFOV(float value)
    {
        //playerCamFOV = value;
        foreach (Camera camera in playerCameras)
        {
            camera.fieldOfView = playerCamFOV;
        }
    }
}
