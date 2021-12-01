using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;

public class GameUIControls : NetworkBehaviour
{
    [SerializeField] private Transform canvasMenu;
    [SerializeField] private Transform canvasInGame;
    [SerializeField] private FirstPersonLookController firstPersonLookController;

    public Transform activeMenuCanvas;

    public Transform ActiveMenuCanvas
    {
        get => activeMenuCanvas;
        set
        {
            if (isLocalPlayer)
            {
                activeMenuCanvas = value;

            }
        }
    }

    private static GameUIControls instance;

    public static GameUIControls Instance
    {
        get => instance;
    }

    public override void OnStartAuthority()
    {
        instance = this;
        activeMenuCanvas = canvasMenu;
        JODSInput.Controls.MainMenu.Escape.performed += ToggleMenuControls;
    }

    public override void OnStopAuthority()
    {
        JODSInput.Controls.MainMenu.Escape.performed -= ToggleMenuControls;
    }

    public void ToggleMenu(GameObject newMenu)
    {
        
    }

    public void ToggleMenuControls(InputAction.CallbackContext obj)
    {
        // Also make the escape button work as a back button in a menu perhaps??

        activeMenuCanvas.gameObject.SetActive(!activeMenuCanvas.gameObject.activeSelf);
        canvasInGame.gameObject.SetActive(!canvasInGame.gameObject.activeSelf);

        // Reset the target menu canvas.
        if (!activeMenuCanvas.gameObject.activeSelf)
        { // this don't work yet completetly help..
            activeMenuCanvas = canvasMenu;
        }

        if (activeMenuCanvas.gameObject.activeSelf)
        {
            JODSInput.DisableCamera();
            //JODSInput.DisableMovement();
            JODSInput.DisableLMB();
            JODSInput.DisableRMB();
            JODSInput.DisableDrop();
            JODSInput.DisableReload();
            JODSInput.DisableInteract();
            firstPersonLookController.ShowCursor();
        }
        else
        {
            JODSInput.EnableCamera();
            //JODSInput.EnableMovement();
            JODSInput.EnableLMB();
            JODSInput.EnableRMB();
            JODSInput.EnableDrop();
            JODSInput.EnableReload();
            JODSInput.EnableInteract();
            firstPersonLookController.HideCursor();
        }
    }
}
