﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private Transform canvasMenu = null;
    [SerializeField] private Transform canvasInGame = null;
    [Space]
    [SerializeField] private bool hideCursorOnDisable = true;
    [Space]
    [SerializeField] private bool hasEquipment = false;
    [SerializeField] private Dropdown equipmentBehaviourDropDown = null;

    public Action onMenuOpened;
    public Action onMenuClosed;

    [Space]
    public Transform activeMenuCanvas;

    private static PlayerManager instance;
    private ActiveSurvivorClass activeSClass;
    private SurvivorStatManager characterStatManager;


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
        activeSClass = GetComponent<ActiveSurvivorClass>();
    }

    private async void FindComponents()
    {
        await JODSTime.WaitTime(0.1f);
    }

    public override void OnStopAuthority()
    {
        JODSInput.Controls.MainMenu.Escape.performed -= ToggleMenuControls;
        if (hasEquipment)
        {
            equipmentBehaviourDropDown.onValueChanged.RemoveListener(GameSettings.Instance.SetPickupBehaviour);
        }
    }
    public override void OnStartClient()
    {
        if (isServer)
        {
            characterStatManager = GetComponent<SurvivorStatManager>();
            characterStatManager.onDownChanged.AddListener(delegate (bool isDown) { OnDownChanged(isDown); });
        }
    }

    private void OnDownChanged(bool isDown)
    {
        if (isDown)
        {
            Rpc_DisableEverythingButMenuAndCamera(connectionToClient);
            Rpc_DisableCamera(connectionToClient);
        }
        else
        {
            Rpc_EnableEverythingButMenuAndCamera(connectionToClient);
            Rpc_EnableCamera(connectionToClient);
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

        // Reset the target menu canvas back to standard menu.
        if (!activeMenuCanvas || !activeMenuCanvas.gameObject.activeSelf)
        {
            activeMenuCanvas = canvasMenu;
        }
        canvasInGame.gameObject.SetActive(true);
        JODSInput.EnableCamera();
        JODSInput.EnableLMB();
        JODSInput.EnableRMB();
        JODSInput.EnableDrop();
        JODSInput.EnableReload();
        JODSInput.EnableInteract();
        //JODSInput.EnableMovement();
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

    [TargetRpc]
    public void Rpc_EnableEverythingButMenuAndCamera(NetworkConnection target)
    {
        JODSInput.EnableMovement();
        JODSInput.EnableJump();
        JODSInput.EnableDrop();
        JODSInput.EnableInteract();
        JODSInput.EnableReload();
        JODSInput.EnableLMB();
        JODSInput.EnableRMB();
        JODSInput.EnableHotbarControl();

    }
    [TargetRpc]
    public void Rpc_DisableEverythingButMenuAndCamera(NetworkConnection target)
    {
        JODSInput.DisableMovement();
        JODSInput.DisableJump();
        JODSInput.DisableDrop();
        JODSInput.DisableInteract();
        JODSInput.DisableReload();
        JODSInput.DisableLMB();
        JODSInput.DisableRMB();
        JODSInput.DisableHotbarControl();
    }

    [TargetRpc]
    public void Rpc_EnableCamera(NetworkConnection target)
    {
        JODSInput.EnableCamera();
    }
    [TargetRpc]
    public void Rpc_DisableCamera(NetworkConnection target)
    {
        JODSInput.DisableCamera();
    }
}
