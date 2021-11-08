using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonLookController : MonoBehaviour, IBindable
{
	public Transform rotateHorizontal = null;
	public Transform rotateVertical = null;

	[SerializeField] private float sensitivity = 1f;
	[SerializeField] private float acceleration = 1f;
	[SerializeField] private float smoothAcceleration = 500f;
	[SerializeField] private float minRotY = -75f;
	[SerializeField] private float maxRotY = 75f;

	private Vector2 rotation;
	private float targetRotX, targetRotY = 0f;
	private float smoothTargetRotX, smoothTargetRotY = 0f;

    public void Bind()
	{
		JODSInput.Controls.Survivor.Camera.performed += Look;
	}
	public void Unbind()
	{
		JODSInput.Controls.Survivor.Camera.performed -= Look;
	}
	public void HideCursor()
    {
		Cursor.lockState = CursorLockMode.Locked;
	}
	public void ShowCursor()
	{
		Cursor.lockState = CursorLockMode.None;
	}

    private void Awake()
    {
		sensitivity = GameSettings.Instance.mouseSensitivity;
		acceleration = GameSettings.Instance.mouseAcceleration;
    }

    void Look(InputAction.CallbackContext context)
	{
		Vector2 mouseDelta = context.ReadValue<Vector2>();
		rotation.x = mouseDelta.x * sensitivity;
		rotation.y = mouseDelta.y * sensitivity;
	}

	public void DoRotation()
	{
		float acceleration = 1f;
		if (this.acceleration != 0)
        {
			acceleration = Mathf.Clamp(rotation.magnitude * this.acceleration, 0f, 10f) * 0.1f;
        }
        // Set target rotations.
        // Clamp target rotation X.
        targetRotX = Mathf.Clamp(targetRotX += -rotation.y, minRotY, maxRotY);
		targetRotY += rotation.x;
		// Lerp the target rotations from current target rotation to camera's rotation + target rotation.
		smoothTargetRotX = Mathf.Lerp(smoothTargetRotX, rotateVertical.rotation.x + targetRotX, Time.deltaTime * smoothAcceleration);
        // Lerp the target rotations from current target rotation to body's rotation + target rotation.
        smoothTargetRotY = Mathf.Lerp(smoothTargetRotY, rotateHorizontal.rotation.y + targetRotY, Time.deltaTime * smoothAcceleration);
		// NOTE: The order of the rotations is important as it is.
        // Rotate the body left and right.
        rotateHorizontal.eulerAngles = new Vector3(0f, targetRotY, 0f);
		// Rotate the camera up and down.
		rotateVertical.eulerAngles = new Vector3(targetRotX, rotateHorizontal.eulerAngles.y, 0f);
	}
}
