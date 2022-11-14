using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WebsocketManager : MonoBehaviour
{
    public static WebsocketManager Instance;

    public WSRequests response;

    public WSResponse<LoginRequest> playerProfile;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(this);
        Instance = this;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Lobby")
        {

        }
    }

    private ClientWebSocket webSocket = null;
    public ClientWebSocket WebSocket
    {
        get => webSocket;
    }
    
    public async Task Connect()
    {
        try
        {
            webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri("ws://localhost:7777"), CancellationToken.None);

            Debug.LogWarning("Websocket: " +webSocket.State);

            Receive();
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    public static Action<WSResponse<UserStats>> onGetStats;

    private async Task Receive()
    {
        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[8192]);

        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result;

            do
            {
                result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
            }
            while (!result.EndOfMessage);

            string message = System.Text.Encoding.UTF8.GetString(buffer.Array, 0, result.Count);


            response = JsonUtility.FromJson<WSRequests>(message);

            Debug.LogWarning(response.type + " response: received | " + JsonUtility.FromJson<WSResponse<LoginRequest>>(message).code);
            Debug.LogWarning(message);

            switch (response.type)
            {
                case "login":
                    LoginManager.Instance.LoginResponse(JsonUtility.FromJson<WSResponse<LoginRequest>>(message));
                    break;

                case "setActive":
                    await Send(response);
                    break;

                case "disconnect":
                    SceneManager.LoadScene(0);
                    Destroy(gameObject);
                    break;

                case "getStats":
                    onGetStats?.Invoke(JsonUtility.FromJson<WSResponse<UserStats>>(message));
                    break;

                default: break;
            }
        }
    }

    public async Task Send(WSRequests request)
    {
        try
        {
            //First we convert the WSRequest into a JSON object
            //Then we encode that JSON into bytes
            var encoded = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(request));
            //Make an ArraySegment out of those bytes
            ArraySegment<byte> buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            //Then we send the ArraySegment to the websocket.
            await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

            Debug.LogWarning(request.type + " request: sent");
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    private void OnApplicationQuit()
    {
        if (webSocket == null) return;
        if (webSocket.State != WebSocketState.Open) return;

        var request = new DisconnectRequest()
        {
            type = RequestType.disconnect.ToString(),
            reason = "Application Quit"
        };

         Send(request);
    }
}
