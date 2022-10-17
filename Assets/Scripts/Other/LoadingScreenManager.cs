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
        if (Instance)
        {
            Destroy(gameObject);
        }
        Instance = this;
        Lobby.OnServerGameStarted += delegate { DontDestroyOnLoad(this); };
        Lobby.OnClientGameStarted += delegate { DontDestroyOnLoad(this); };
    }

    private void OnDisable()
    {
        Lobby.OnServerGameStarted -= delegate { DontDestroyOnLoad(this); };
        Lobby.OnClientGameStarted -= delegate { DontDestroyOnLoad(this); };
    }
    #endregion

    private void Start()
    {
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
        Rpc_ShowLoadingScreen(enable);
    }
}
