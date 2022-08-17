using UnityEngine;
using System;

public class JODSInput : MonoBehaviour
{
    public static Action onMovementDisabled;
    public static Action onCameraDisabled;

    private static int overrides_movement = 0;
    private static int overrides_jump = 0;
    private static int overrides_camera = 0;
    private static int overrides_interact = 0;
    private static int overrides_hotbarcontrol = 0;
    private static int overrides_lmb = 0;
    private static int overrides_rmb = 0;
    private static int overrides_reload = 0;
    private static int overrides_drop = 0;

    public static Controls Controls { get; private set; }

    //private void OnGUI()
    //{
    //    GUI.TextField(new Rect(20, 60, 150, 20), overrides_movement.ToString());
    //    GUI.TextField(new Rect(20, 80, 150, 20), overrides_jump.ToString());
    //    GUI.TextField(new Rect(20, 100, 150, 20), overrides_camera.ToString());
    //}

    private void Awake()
    {
        Controls = new Controls();
    }

    private void OnEnable()
    {
        if (Controls == null) return;
        Controls.Enable();
    }

    private void OnDisable()
    {
        if (Controls == null) return;
        Controls.Disable();
    }

    // MOVEMENT
    public static void EnableMovement()
    {
        overrides_movement++;
        if (overrides_movement == 0)
            Controls.Survivor.Movement.Enable();
    }
    public static void DisableMovement()
    {
        overrides_movement--;
        Controls.Survivor.Movement.Disable();
        onMovementDisabled?.Invoke();
    }
    public static void EnableJump()
    {
        overrides_jump++;
        if (overrides_jump == 0)
            Controls.Survivor.Jump.Enable();
    }
    public static void DisableJump()
    {
        overrides_jump--;
        Controls.Survivor.Jump.Disable();
    }
    public static void EnableCamera()
    {
        overrides_camera++;
        if (overrides_camera == 0)
            Controls.Survivor.Camera.Enable();
    }
    public static void DisableCamera()
    {
        overrides_camera--;
        Controls.Survivor.Camera.Disable();
        onCameraDisabled?.Invoke();
    }
    // INTERACTION
    public static void EnableInteract()
    {
        overrides_interact++;
        if (overrides_interact == 0)
            Controls.Survivor.Interact.Enable();
    }
    public static void DisableInteract()
    {
        overrides_interact--;
        Controls.Survivor.Interact.Disable();
    }
    public static void EnableHotbarControl()
    {
        overrides_hotbarcontrol++;
        if (overrides_hotbarcontrol == 0)
            Controls.Survivor.Hotbarselecting.Enable();
    }
    public static void DisableHotbarControl()
    {
        overrides_hotbarcontrol--;
        Controls.Survivor.Hotbarselecting.Disable();
    }
    public static void EnableLMB()
    {
        overrides_lmb++;
        if (overrides_lmb == 0)
            Controls.Survivor.LMB.Enable();
    }
    public static void DisableLMB()
    {
        overrides_lmb--;
        Controls.Survivor.LMB.Disable();
    }
    public static void EnableRMB()
    {
        overrides_rmb++;
        if (overrides_rmb == 0)
            Controls.Survivor.RMB.Enable();
    }
    public static void DisableRMB()
    {
        overrides_rmb--;
        Controls.Survivor.RMB.Disable();
    }
    public static void EnableReload()
    {
        overrides_reload++;
        if (overrides_reload == 0)
            Controls.Survivor.Reload.Enable();
    }
    public static void DisableReload()
    {
        overrides_reload--;
        Controls.Survivor.Reload.Disable();
    }
    public static void EnableDrop()
    {
        overrides_drop++;
        if (overrides_drop == 0)
            Controls.Survivor.Drop.Enable();
    }
    public static void DisableDrop()
    {
        overrides_drop--;
        Controls.Survivor.Drop.Disable();
    }
    // Menu/UI
    public static void EnableEscape()
    {
        Controls.MainMenu.Escape.Enable();
    }
    public static void DisableEscape()
    {
        Controls.MainMenu.Escape.Disable();
    }
}
