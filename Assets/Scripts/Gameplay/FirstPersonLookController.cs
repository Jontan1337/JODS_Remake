using System;
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
	[SerializeField] private float easingSpeed = 500f;
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
		JODSInput.onCameraDisabled += OnCameraDisabled;
		GameSettings.onMouseSensitivityChanged += OnMouseSensitivityChanged;
		GameSettings.onMouseAccelerationChanged += OnMouseAccelerationChanged;
		GameSettings.onMouseMaxAccelerationChanged += OnMouseMaxAccelerationChanged;
		GameSettings.onMouseEasingSpeedChanged += OnMouseEasingChanged;
	}

    private void OnMouseSensitivityChanged(float value)
    {
		sensitivity = value;
    }

    private void OnMouseAccelerationChanged(float value)
    {
		acceleration = value;
    }

    private void OnMouseMaxAccelerationChanged(float value)
    {
		maxAcceleration = value;
    }

    private void OnMouseEasingChanged(float value)
    {
		easingSpeed = value;
    }

    public void Unbind()
	{
		JODSInput.Controls.Survivor.Camera.performed -= Look;
		JODSInput.onCameraDisabled -= OnCameraDisabled;
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
	}

    void Look(InputAction.CallbackContext context)
	{
		Vector2 mouseDelta = context.ReadValue<Vector2>();
		rotation.x = mouseDelta.x * sensitivity;
		rotation.y = mouseDelta.y * sensitivity;
	}

	private void OnCameraDisabled()
    {
		rotation = new Vector2();
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
		smoothTargetRotX = Mathf.Lerp(smoothTargetRotX, rotateVertical.rotation.x + targetRotX, Time.deltaTime * easingSpeed);
        // Lerp the target rotations from current target rotation to body's rotation + target rotation.
        smoothTargetRotY = Mathf.Lerp(smoothTargetRotY, rotateHorizontal.rotation.y + targetRotY, Time.deltaTime * easingSpeed);
		// NOTE: The order of the rotations is important as it is.
        // Rotate the body left and right.
        rotateHorizontal.eulerAngles = new Vector3(0f, smoothTargetRotY, 0f);
		// Rotate the camera up and down.
		rotateVertical.eulerAngles = new Vector3(smoothTargetRotX, rotateHorizontal.eulerAngles.y, 0f);
	}
}
