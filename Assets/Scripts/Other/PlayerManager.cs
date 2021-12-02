using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private Transform canvasMenu;
    [SerializeField] private Transform canvasInGame;
    [SerializeField] private FirstPersonLookController firstPersonLookController;

    public Action onMenuClosed;

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

    private static PlayerManager instance;

    public static PlayerManager Instance
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

    private void ToggleMenuControls(InputAction.CallbackContext obj)
    {
        // Also make the escape button work as a back button in a menu perhaps??

        if (activeMenuCanvas.gameObject.activeSelf)
        {
            DisableMenu();
        }
        else
        {
            EnableMenu();
        }
    }

    public void EnableMenu()
    {
        activeMenuCanvas.gameObject.SetActive(true);
        canvasInGame.gameObject.SetActive(false);
        JODSInput.DisableCamera();
        //JODSInput.DisableMovement();
        JODSInput.DisableLMB();
        JODSInput.DisableRMB();
        JODSInput.DisableDrop();
        JODSInput.DisableReload();
        JODSInput.DisableInteract();
        firstPersonLookController.ShowCursor();
    }
    public void DisableMenu()
    {
        activeMenuCanvas.gameObject.SetActive(false);
        canvasInGame.gameObject.SetActive(true);
        // Reset the target menu canvas back to standard menu.
        if (!activeMenuCanvas.gameObject.activeSelf)
        {
            activeMenuCanvas = canvasMenu;
        }
        JODSInput.EnableCamera();
        //JODSInput.EnableMovement();
        JODSInput.EnableLMB();
        JODSInput.EnableRMB();
        JODSInput.EnableDrop();
        JODSInput.EnableReload();
        JODSInput.EnableInteract();
        firstPersonLookController.HideCursor();
        onMenuClosed?.Invoke();
    }

    public void QuitToMenu(string sceneName)
    {
        if (netIdentity.isActiveAndEnabled)
        {
            if (isServer)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }
            Destroy(NetworkManager.singleton);
        }
        SceneManager.LoadSceneAsync(sceneName);
    }
}
