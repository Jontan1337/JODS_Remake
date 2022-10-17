using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;

public class LobbyFade : NetworkBehaviour
{
    [SerializeField] private Image fadeImage = null;

    private void Start()
    {
        //var e = FindObjectsOfType<LobbyFade>();
        //if (e.Length > 1)
        //{
        //    Destroy(gameObject);
        //}
        Lobby.OnServerGameStarted += delegate { DontDestroyOnLoad(this); };
    }
    private void OnDisable()
    {
        Lobby.OnServerGameStarted -= delegate { DontDestroyOnLoad(this); };
    }

    public void BeginFade(float time)
    {
        RpcBeginFade(time);
    }

    [ClientRpc]
    private void RpcBeginFade(float time)
    {
        StartCoroutine(FadeCo(time));
    }

    private IEnumerator FadeCo(float time)
    {
        Color col = fadeImage.color;

        float curTime = 0;

        while (curTime < time)
        {
            curTime += Time.deltaTime;
            col.a = curTime / time;
            yield return null;
            fadeImage.color = col;
        }
    }
}
