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

    public override void OnStartAuthority()
    {
        JODSInput.Controls.MainMenu.Escape.performed += ToggleMenu;
    }

    public override void OnStopAuthority()
    {
        JODSInput.Controls.MainMenu.Escape.performed -= ToggleMenu;
    }

    private void ToggleMenu(InputAction.CallbackContext obj)
    {
        // Also make the escape button work as a back button in a menu perhaps??

        canvasMenu.gameObject.SetActive(!canvasMenu.gameObject.activeSelf);
        canvasInGame.gameObject.SetActive(!canvasInGame.gameObject.activeSelf);
        if (canvasMenu.gameObject.activeSelf)
        {
            JODSInput.DisableCamera();
            firstPersonLookController.ShowCursor();
        }
        else
        {
            JODSInput.EnableCamera();
            firstPersonLookController.HideCursor();
        }
    }
}
