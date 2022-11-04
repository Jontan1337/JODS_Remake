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

    [SerializeField] private InputField email;
    [SerializeField] private InputField password;
    [SerializeField] private Text errorMessage;
    [SerializeField] private Button loginButton;

    [SerializeField, Scene] private string lobbyScene;

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
            type = "login",
            email = email.text,
            password = password.text
        };

        await WebsocketManager.Instance.Send(loginRequest);
    }

    public void LoginResponce(WSResponse<LoginRequest> response)
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
