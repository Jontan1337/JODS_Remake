using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform rotateVertical = null;
    [SerializeField]
    private Transform rotateHorizontal = null;
    [SerializeField]
    private Transform playerCamera = null;
    [SerializeField]
    private Transform cameraTarget = null;
    [SerializeField]
    private Transform playerModel = null;
    [SerializeField]
    private float sensitivity = 1f;
    [SerializeField]
    private float cameraSmoothing = 10f;

    Vector3 verticalRotation = Vector3.zero;
    Vector3 horizontalRotation = Vector3.zero;

    float mouseX;
    float mouseY;
    private void Update()
    {
        playerCamera.position = cameraTarget.position;

        mouseX += Input.GetAxisRaw("Mouse X") * sensitivity;
        mouseY += Input.GetAxisRaw("Mouse Y") * sensitivity;

        horizontalRotation = new Vector3(rotateHorizontal.rotation.x, mouseX);
        rotateHorizontal.rotation = Quaternion.Euler(horizontalRotation);
        verticalRotation = new Vector3(-mouseY, horizontalRotation.y);
        rotateVertical.rotation = Quaternion.Euler(verticalRotation);
    }

    private void LateUpdate()
    {
        playerModel.rotation = Quaternion.Euler(0f, playerCamera.rotation.eulerAngles.y, 0f);
    }
}
