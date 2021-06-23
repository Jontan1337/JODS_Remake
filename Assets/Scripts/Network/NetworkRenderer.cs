using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkRenderer : NetworkBehaviour
{
    [SerializeField, SyncVar(hook = nameof(SetVisibility))] private bool isVisible;
    [SerializeField] private Renderer objectRenderer;

    private WaitForSeconds WaitForSeconds;

    private void Awake()
    {
        if (TryGetComponent(out objectRenderer))
        {
            objectRenderer.enabled = isVisible;
        }
        else
        {
            Debug.LogError($"Gameobject doesn't have a renderer component", this);
        }
    }

    public override void OnStartServer()
    {
        WaitForSeconds = new WaitForSeconds(syncInterval);
        if (objectRenderer != null)
        {
            StartCoroutine(IEUpdate());
        }
    }

    private IEnumerator IEUpdate()
    {
        while (true)
        {
            yield return WaitForSeconds;
            isVisible = objectRenderer.enabled;
        }
    }

    private void SetVisibility(bool oldValue, bool newValue)
    {
        if (newValue == true)
        {
            ShowItem();
        }
        else {
            HideItem();
        }
    }

    [Command]
    public void Cmd_ShowItem()
    {
        Svr_ShowItem();
    }
    [Command]
    public void Cmd_HideItem()
    {
        Svr_HideItem();
    }
    [Server]
    public void Svr_ShowItem()
    {
        ShowItem();
        isVisible = true;
        //Rpc_ShowItem();
    }
    [Server]
    public void Svr_HideItem()
    {
        HideItem();
        isVisible = false;
        //Rpc_HideItem();
    }
    [ClientRpc]
    public void Rpc_ShowItem()
    {
        ShowItem();
    }
    [ClientRpc]
    public void Rpc_HideItem()
    {
        HideItem();
    }
    private void ShowItem()
    {
        GetComponent<Renderer>().enabled = true;
    }
    private void HideItem()
    {
        GetComponent<Renderer>().enabled = false;
    }
}
