using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookController : NetworkBehaviour
{
    float minRotY = -75f;
    float maxRotY = 75F;
    public float rotY;
    public float rotX;
    public float sensitivity;
    private bool canLook = true;
    public Camera playerCamera;
    [SerializeField]
    private Transform rotateHorizontal;
    [SerializeField]
    private Transform rotateVertical;

    #region NetworkBehaviour Callbacks
    public override void OnStartAuthority()
    {
        Cursor.lockState = CursorLockMode.Locked;
        JODSInput.Controls.Survivor.Camera.performed += Look;
    }
    public override void OnStopAuthority()
    {
        JODSInput.Controls.Survivor.Camera.performed -= Look;
    }
    #endregion

    void Look(InputAction.CallbackContext context)
    {
        Vector2 mouseDelta = context.ReadValue<Vector2>();
        if (!canLook)
        {
            return;
        }
        rotX += mouseDelta.x * sensitivity;
        rotY += mouseDelta.y * sensitivity;
        rotY = Mathf.Clamp(rotY, minRotY, maxRotY);
        //transform.Rotate(0, rotX, 0);
        rotateHorizontal.rotation = Quaternion.Euler(0, rotX, 0);
        playerCamera.transform.rotation = Quaternion.Euler(-rotY, rotX, 0f);
    }

    public void EnableLook()
    {        
        canLook = true;
    }
    public void DisableLook()
    {
        rotY = 0;
        playerCamera.transform.rotation = Quaternion.Euler(0f, rotX, 0f);
        canLook = false;
    }
}
