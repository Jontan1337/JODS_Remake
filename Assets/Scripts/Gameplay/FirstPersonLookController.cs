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
	[SerializeField] private float maxAcceleration = 1f;
	[SerializeField] private float mouseEasingSpeed = 500f;
	[SerializeField] private float minRotY = -75f;
	[SerializeField] private float maxRotY = 75f;

	private Vector2 rotation;
	private float targetRotX, targetRotY = 0f;
	private float smoothTargetRotX, smoothTargetRotY = 0f;

	private void Awake()
	{
		SetMouseSettings();
	}

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

	private void SetMouseSettings()
    {
		sensitivity = GameSettings.Instance.mouseSensitivity;
		acceleration = GameSettings.Instance.mouseAcceleration;
		maxAcceleration = GameSettings.Instance.maxMouseAcceleration;
		mouseEasingSpeed = GameSettings.Instance.mouseEasingSpeed;
	}

    void Look(InputAction.CallbackContext context)
	{
		Vector2 mouseDelta = context.ReadValue<Vector2>();
		rotation.x = mouseDelta.x * sensitivity;
		rotation.y = mouseDelta.y * sensitivity;
	}

	public void DoRotation()
	{
		if (!rotateVertical || !rotateHorizontal) return;
		float acceleration = 1f;
		if (this.acceleration != 0)
        {
			acceleration = Mathf.Clamp(rotation.magnitude * this.acceleration, 0f, maxAcceleration) * 0.1f;
        }
        // Set target rotations.
        // Clamp target rotation X.
        targetRotX = Mathf.Clamp(targetRotX += -rotation.y * acceleration, minRotY, maxRotY);
		targetRotY += rotation.x * acceleration;
		// Lerp the target rotations from current target rotation to camera's rotation + target rotation.
		smoothTargetRotX = Mathf.Lerp(smoothTargetRotX, rotateVertical.rotation.x + targetRotX, Time.deltaTime * mouseEasingSpeed);
        // Lerp the target rotations from current target rotation to body's rotation + target rotation.
        smoothTargetRotY = Mathf.Lerp(smoothTargetRotY, rotateHorizontal.rotation.y + targetRotY, Time.deltaTime * mouseEasingSpeed);
		// NOTE: The order of the rotations is important as it is.
        // Rotate the body left and right.
        rotateHorizontal.eulerAngles = new Vector3(0f, smoothTargetRotY, 0f);
		// Rotate the camera up and down.
		rotateVertical.eulerAngles = new Vector3(smoothTargetRotX, rotateHorizontal.eulerAngles.y, 0f);
	}
}
