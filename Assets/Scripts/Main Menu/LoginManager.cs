using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;

    [SerializeField] private InputField email = null;
    [SerializeField] private InputField password = null;
    [SerializeField] private Text errorMessage = null;
    [SerializeField] private Button loginButton = null;

    [SerializeField, Scene] private string lobbyScene = "";

    LoginRequest loginRequest;

    private void Awake()
    {
        Instance = this;
        loginButton.onClick.AddListener(LoginBtn);
    }

    private async void LoginBtn()
    {
        await WebsocketManager.Instance.Connect();

        loginRequest = new LoginRequest()
        {
            type = RequestType.login.ToString(),
            email = email.text,
            password = password.text
        };

        await WebsocketManager.Instance.Send(loginRequest);
    }

    public void LoginResponse(WSResponse<LoginRequest> response)
    {
        // Success!!!
        if (response.code == 0)
        {
            WebsocketManager.Instance.playerProfile = response;
            SceneManager.LoadSceneAsync(lobbyScene);

        }
        else
        {
            errorMessage.text = response.message;
        }
    }
}
