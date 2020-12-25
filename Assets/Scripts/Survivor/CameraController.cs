using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform rotateVertical;
    [SerializeField]
    private Transform rotateHorizontal;
    [SerializeField]
    private float sensitivity = 1f;

    Vector3 verticalRotation = Vector3.zero;
    Vector3 horizontalRotation = Vector3.zero;

    float mouseX;
    float mouseY;
    private void Update()
    {
        mouseX += Input.GetAxisRaw("Mouse X");
        mouseY += Input.GetAxisRaw("Mouse Y");

        horizontalRotation = new Vector3(rotateHorizontal.rotation.x, mouseX);
        rotateHorizontal.rotation = Quaternion.Euler(horizontalRotation);
        verticalRotation = new Vector3(-mouseY, horizontalRotation.y);
        rotateVertical.rotation = Quaternion.Euler(verticalRotation);
    }
}
