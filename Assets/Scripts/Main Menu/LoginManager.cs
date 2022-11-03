using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private InputField email;
    [SerializeField] private InputField password;
    [SerializeField] private Text errorMessage;
    [SerializeField] private Button loginButton;

    LoginRequest loginRequest;

    private void Start()
    {
        loginButton.onClick.AddListener(LoginBtn);
    }

    private async void LoginBtn()
    {
        await WebsocketManager.instance.Connect();

        loginRequest = new LoginRequest()
        {
            type = "login",
            email = email.text,
            password = password.text
        };

        await WebsocketManager.instance.Send(loginRequest);
    }
}
