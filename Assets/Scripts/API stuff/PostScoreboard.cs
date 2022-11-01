using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System;
using System.Threading;

public class PostScoreboard : MonoBehaviour
{
    //Create a singleton for this component
    public static PostScoreboard Instance;
    private void Awake()
    {
        Instance = this;
    }

    // We need a reference to the users profile here, to identify which user's scoreboard we're uploading.
    // For now we just hardcode it
    public async void UserPostScoreboard(BasePlayerData playerData)
    {
        await Connect(url);

        await Send(playerData);
    }

    private ClientWebSocket webSocket = null;

    private readonly string url = "ws://localhost:7777";

    public async Task Connect(string uri)
    {
        try
        {
            webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);

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

        while(webSocket.State == WebSocketState.Open)
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

    private async Task Send(BasePlayerData playerData)
    {
        //Cache a reference to a ScoreboardRequest
        ScoreboardRequest request = new ScoreboardRequest();


        //Here we check what kind of playerdata we're handling

        //If it's a SurvivorPlayerData
        if (playerData.TryGetComponent(out SurvivorPlayerData survivorData))
        {
            request = new SurvivorScoreboardRequest()
            {
                type = RequestType.uploadScoreboard.ToString(), //What kind of message are we sending
                isMaster = false, //Then we set this bool to false
                highscore = survivorData.Score, //Set the player's score
                classType = survivorData.classType, //and set the classType to the player's classType
                kills = survivorData.Kills,
                //deaths = survivorData.Downs,
                //specialsUsed = survivorData.specialsUsed
            };
        }
        //If it's a MasterPlayerData
        else if (playerData.TryGetComponent(out MasterPlayerData masterData))
        {
            request = new MasterScoreboardRequest()
            {
                type = RequestType.uploadScoreboard.ToString(), //What kind of message are we sending
                isMaster = true, //Then we set this bool to true
                highscore = masterData.Score, //Set the player's score
                unitsPlaced = masterData.UnitsPlaced,
                totalUnitUpgrades = masterData.TotalUnitUpgrades
            };
        }

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

    #region Debug

    private async void DebugSend()
    {
        SurvivorScoreboardRequest request = new SurvivorScoreboardRequest
        {
            type = RequestType.uploadScoreboard.ToString(),
            highscore = 200,
            classType = "Doctor",
            isMaster = false
        };

        try
        {
            var encoded = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(request));

            ArraySegment<byte> buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);

            await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

            Debug.Log("message :'" + request.highscore + "' sent");
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    public bool go;
    public bool send;

    private void OnValidate()
    {
        if (go)
        {
            go = false;
            Task connect = Connect(url);
        }
        if (send)
        {
            send = false;
            DebugSend();
        }
    }

    #endregion
}
