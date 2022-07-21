using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private Transform canvasMenu;
    [SerializeField] private Transform canvasInGame;
    [Space]
    [SerializeField] private bool hideCursorOnDisable = true;
    [Space]
    [SerializeField] private bool hasEquipment;
    [SerializeField] private Dropdown equipmentBehaviourDropDown;

    public Action onMenuOpened;
    public Action onMenuClosed;

    [Space]
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
        if (hasEquipment)
        {
            equipmentBehaviourDropDown.onValueChanged.AddListener(GameSettings.Instance.SetPickupBehaviour);
        }
    }

    public override void OnStopAuthority()
    {
        JODSInput.Controls.MainMenu.Escape.performed -= ToggleMenuControls;
        if (hasEquipment)
        {
            equipmentBehaviourDropDown.onValueChanged.RemoveListener(GameSettings.Instance.SetPickupBehaviour);
        }
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

    [TargetRpc]
    public void Rpc_EnableMenu(NetworkConnection target)
    {
        EnableMenu();
    }
    [TargetRpc]
    public void Rpc_DisableMenu(NetworkConnection target)
    {
        DisableMenu();
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
        ShowCursor();
        onMenuOpened?.Invoke();
    }
    public void DisableMenu()
    {
        if (activeMenuCanvas)
            activeMenuCanvas.gameObject.SetActive(false);

        canvasInGame.gameObject.SetActive(true);
        // Reset the target menu canvas back to standard menu.
        if (!activeMenuCanvas || !activeMenuCanvas.gameObject.activeSelf)
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
        if (hideCursorOnDisable) HideCursor();
        onMenuClosed?.Invoke();
    }
    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
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
