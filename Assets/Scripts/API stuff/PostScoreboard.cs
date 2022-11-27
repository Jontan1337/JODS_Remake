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

    public async void UserPostScoreboard(BasePlayerData playerData)
    {
        WebsocketManager websocketManager = WebsocketManager.Instance;

        if (websocketManager == null)
        {
            Debug.LogError("WebsocketManager Singleton not found!");
            return;
        }

        webSocket = websocketManager.WebSocket;

        if (webSocket.State != WebSocketState.Open)
        {
            Debug.LogError("Websocket Connection Closed!");
            return;
        }

        Debug.Log("Websocket State: " + webSocket.State);

        await Send(playerData);
    }

    private ClientWebSocket webSocket = null;
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
                deaths = survivorData.Downs,
                specialUsed = survivorData.SpecialUsed
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

            Debug.Log("Scoreboard sent");
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }         
    }
}
