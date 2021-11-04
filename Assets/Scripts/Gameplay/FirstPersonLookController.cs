using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonLookController : MonoBehaviour, IBindable
{
	public Transform rotateHorizontal = null;
	public Transform rotateVertical = null;

	[SerializeField] private float sensitivity;
    [SerializeField] private float smoothAcceleration = 100;
	[SerializeField] private float minRotY = -75f;
	[SerializeField] private float maxRotY = 75F;

	private Vector2 rotation;
	private float targetRotX, targetRotY = 0f;
	private float smoothTargetRotX, smoothTargetRotY = 0f;

    public void Bind()
	{
		JODSInput.Controls.Survivor.Camera.performed += Look;
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void Unbind()
	{
		JODSInput.Controls.Survivor.Camera.performed -= Look;
	}

	void Look(InputAction.CallbackContext context)
	{
		Vector2 mouseDelta = context.ReadValue<Vector2>();
		rotation.x = mouseDelta.x * sensitivity;
		rotation.y = mouseDelta.y * sensitivity;
	}

	public void DoRotation()
	{
		// Set target rotations for the Lerp.
		// Clamp target rotation X.
		targetRotX = Mathf.Clamp(targetRotX += -rotation.y, minRotY, maxRotY);
		targetRotY += rotation.x;
		// Lerp the target rotations from current target rotation to camera's rotation + target rotation.
		smoothTargetRotX = Mathf.Lerp(smoothTargetRotX, rotateVertical.rotation.x + targetRotX, Time.deltaTime * smoothAcceleration);
		// Lerp the target rotations from current target rotation to body's rotation + target rotation.
		smoothTargetRotY = Mathf.Lerp(smoothTargetRotY, rotateHorizontal.rotation.y + targetRotY, Time.deltaTime * smoothAcceleration);
		// Rotate the camera up and down.
		rotateVertical.eulerAngles = new Vector3(smoothTargetRotX, smoothTargetRotY, 0f);
		// Rotate the body left and right.
		rotateHorizontal.eulerAngles = new Vector3(0f, smoothTargetRotY, 0f);
	}
}
