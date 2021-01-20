using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookController : MonoBehaviour
{
    float minRotY = -75f;
    float maxRotY = 75F;
    public float rotY;
    public float rotX;
    public float sensitivity;
    public new Camera camera;

    void Update()
    {
        rotY += Input.GetAxis("Mouse Y") * sensitivity;
        rotX += Input.GetAxis("Mouse X") * sensitivity;
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        rotY = Mathf.Clamp(rotY, minRotY, maxRotY);
        camera.transform.rotation = Quaternion.Euler(-rotY, rotX, 0f);
    }
}
