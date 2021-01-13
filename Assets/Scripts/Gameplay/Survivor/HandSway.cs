using UnityEngine;

public class HandSway : MonoBehaviour
{
    [SerializeField] private float swayVelocityX = 0.05f;
    [SerializeField] private float swayVelocityY = 0.08f;
    [SerializeField] private float swaySmoothing = 5f;
    [SerializeField] private new Transform camera = null;
    [SerializeField] private MenuHandler menu = null;

    private Quaternion newRotation;
    private Vector3 originalPos;
    private void Start()
    {
        originalPos = transform.localPosition;
    }


    // Update is called once per frame
    void Update()
    {
        float mouseX = -Input.GetAxis("Mouse X") * swayVelocityX;
        float mouseY = Input.GetAxis("Mouse Y") * swayVelocityY;
        if (menu.IsOpen)
        {
            mouseX = 0;
            mouseY = 0;
        }
        newRotation = new Quaternion(camera.localRotation.x + mouseY, 0, camera.localRotation.y + mouseX, 1f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, newRotation, Time.deltaTime * swaySmoothing);
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * swaySmoothing);
    }
}
