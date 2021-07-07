using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbyFade : NetworkBehaviour
{
    [SerializeField] private Image fadeImage = null;

    private void Start()
    {
        DontDestroyOnLoad(this);
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
