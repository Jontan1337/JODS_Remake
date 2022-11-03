using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;

    [SerializeField] private InputField email;
    [SerializeField] private InputField password;
    [SerializeField] private Text errorMessage;
    [SerializeField] private Button loginButton;

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
            print(response.data[0].username);
            errorMessage.text = "";
        }
        else
        {
            errorMessage.text = response.message;
        }
    }
}
