using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ChatManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Image[] chatGameObjects = null;
    [SerializeField] private InputField chatInputField = null;
    [SerializeField] private Transform messageArea = null;
    [SerializeField] private GameObject messagePrefab = null;

    [Header("Other")]
    [SerializeField] private bool chatIsOpen = false;


    public static Action OnOpenChat;
    public static Action OnCloseChat;

    public override void OnStartServer()
    {
        WSRequests ws = new WSRequests()
        {
            type = "openChatroom"
        };

        WebsocketManager.Instance.Send(ws);

        Lobby.OnServerGameStarted += delegate { DontDestroyOnLoad(this); };

    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        JODSInput.Controls.General.Chat.performed += ctx => OpenChat();

        ChatVisuals(chatIsOpen);

        Lobby.OnClientGameStarted += delegate { DontDestroyOnLoad(this); };
    }

    private void OpenChat()
    {
        //Send message
        if (chatIsOpen && chatInputField.text.Length > 0)
        {
            ChatRequest ws = new ChatRequest()
            {
                type = "sendMessage",
                message = chatInputField.text
            };

            WebsocketManager.Instance.Send(ws);

            Cmd_SendChatMessage(WebsocketManager.Instance.playerProfile.data[0].username, chatInputField.text);
            chatInputField.text = "";
        }

        chatIsOpen = !chatIsOpen;
        ChatVisuals(chatIsOpen);

        if (chatIsOpen)
        {
            OnOpenChat?.Invoke();

            JODSInput.Controls.Survivor.Disable();
            JODSInput.Controls.Master.Disable();

            chatInputField.Select();
        }
        else if (!chatIsOpen)
        {
            OnCloseChat?.Invoke();

            JODSInput.Controls.Survivor.Enable();
            JODSInput.Controls.Master.Enable();
        }
    }

    private void ChatVisuals(bool on)
    {
        foreach (Image img in chatGameObjects)
        {
            Color imgCol = img.color;
            imgCol.a = on ? 0.8f : 0f;
            img.color = imgCol;
        }
    }

    [Command(ignoreAuthority = true)]
    private void Cmd_SendChatMessage(string user, string message)
    {
        Rpc_SendChatMessage(user, message);
    }

    [ClientRpc]
    private void Rpc_SendChatMessage(string user, string message)
    {
        ChatMessage msg = Instantiate(messagePrefab, messageArea).GetComponent<ChatMessage>();
        msg.SetMessage(user, message);
    }
}
