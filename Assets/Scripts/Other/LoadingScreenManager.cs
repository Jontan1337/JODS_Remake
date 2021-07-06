using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LoadingScreenManager : NetworkBehaviour
{
    #region Singleton
    public static LoadingScreenManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    private void Start()
    {
        DontDestroyOnLoad(this);
        loadingScreen.SetActive(false);
    }

    [SerializeField] private GameObject loadingScreen = null;

    public void ShowLoadingScreen(bool enable)
    {
        loadingScreen.SetActive(enable);
    }

    [ClientRpc]
    public void Rpc_ShowLoadingScreen(bool enable)
    {
        print("Rpc_ShowLoadingScreen");

        loadingScreen.SetActive(enable);
    }

    [TargetRpc]
    public void Rpc_Target_ShowLoadingScreen(NetworkConnection target, bool enable)
    {
        loadingScreen.SetActive(enable);
    }

    [Server]
    public void Svr_ShowLoadingScreen(bool enable)
    {
        print("Svr_ShowLoadingScreen");
        Rpc_ShowLoadingScreen(enable);
    }
}
