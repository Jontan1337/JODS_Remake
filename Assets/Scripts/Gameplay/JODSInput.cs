using UnityEngine;

public class JODSInput : MonoBehaviour
{
    public static Controls Controls { get; private set; }

    private void Awake()
    {
        Controls = new Controls();
    }

    private void OnEnable()
    {
        Controls.Enable();
    }

    private void OnDisable()
    {
        Controls.Disable();
    }

    // MOVEMENT
    public static void EnableMovement()
    {
        Controls.Survivor.Movement.Enable();
    }
    public static void DisableMovement()
    {
        Controls.Survivor.Movement.Disable();
    }
    public static void EnableJump()
    {
        Controls.Survivor.Jump.Enable();
    }
    public static void DisableJump()
    {
        Controls.Survivor.Jump.Disable();
    }
    public static void EnableCamera()
    {
        Controls.Survivor.Camera.Enable();
    }
    public static void DisableCamera()
    {
        Controls.Survivor.Camera.Disable();
    }
    // INTERACTION
    public static void EnableInteract()
    {
        Controls.Survivor.Interact.Enable();
    }
    public static void DisableInteract()
    {
        Controls.Survivor.Interact.Disable();
    }
    public static void EnableLMB()
    {
        Controls.Survivor.LMB.Enable();
    }
    public static void DisableLMB()
    {
        Controls.Survivor.LMB.Disable();
    }
    public static void EnableRMB()
    {
        Controls.Survivor.RMB.Enable();
    }
    public static void DisableRMB()
    {
        Controls.Survivor.RMB.Disable();
    }
    public static void EnableReload()
    {
        Controls.Survivor.Reload.Enable();
    }
    public static void DisableReload()
    {
        Controls.Survivor.Reload.Disable();
    }
    public static void EnableDrop()
    {
        Controls.Survivor.Drop.Enable();
    }
    public static void DisableDrop()
    {
        Controls.Survivor.Drop.Disable();
    }
}
