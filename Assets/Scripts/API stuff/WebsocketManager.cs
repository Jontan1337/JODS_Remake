using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WebsocketManager : MonoBehaviour
{
    public static WebsocketManager instance;

    private void Awake()
    {
        instance = this;
    }

    private ClientWebSocket webSocket = null;

    public async Task Connect()
    {
        try
        {
            webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri("ws://localhost:7777"), CancellationToken.None);

            Debug.LogWarning(webSocket.State);

            Receive();
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

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

            Debug.LogWarning(message);
        }
    }

    public async Task Send(WSRequests request)
    {
        try
        {
            //First we convert the ScoreboardRequest into a JSON object
            //Then we encode that JSON into bytes
            var encoded = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(request));
            //Make an ArraySegment out of those bytes
            ArraySegment<byte> buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            //Then we send the ArraySegment to the websocket.
            await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

            Debug.LogWarning("message sent");
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }
}
