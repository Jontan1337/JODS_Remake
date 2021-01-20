using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookController : MonoBehaviour
{
    float minRotY = -75f;
    float maxRotY = 75F;
    public float rotY;
    public float rotX;
    float lookSpeed = 3.0f;
    float sensitivity = 1f;
    public new Camera camera;

    void Update()
    {
        rotY += Input.GetAxis("Mouse Y") * lookSpeed * sensitivity;
        rotX += Input.GetAxis("Mouse X") * lookSpeed * sensitivity;
        transform.Rotate(0, lookSpeed * Input.GetAxis("Mouse X") * sensitivity, 0);
        rotY = Mathf.Clamp(rotY, minRotY, maxRotY);
        camera.transform.rotation = Quaternion.Euler(-rotY, rotX, 0f);
    }
}
