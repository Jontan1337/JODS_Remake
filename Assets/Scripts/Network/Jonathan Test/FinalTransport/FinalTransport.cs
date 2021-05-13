using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Net.Sockets;

namespace FinalNetwork
{
    [DisallowMultipleComponent]
    public class FinalTransport : Transport
    {
        [SerializeField]
        private int maxPlayers = 4;
        [SerializeField]
        private string ipaddress;
        [SerializeField]
        private int port = 26950;
        [SerializeField]
        private static int serverTicks = 60;

        private static Client myClient;

        public static int ServerTicks { get => serverTicks; }

        public override bool Available()
        {
            throw new NotImplementedException();
        }

        private void Awake()
        {
            
        }

        #region Client
        public override void ClientConnect(string address)
        {
            int newClientId = Server.clients.Count + 1;
            myClient = new Client(newClientId);
            TcpClient socket = new TcpClient();
            myClient.tcp.Connect(socket);
        }

        public override bool ClientConnected()
        {
            throw new NotImplementedException();
        }

        public override void ClientDisconnect()
        {
            if (myClient != null)
            {
                myClient.Disconnect();
            }
        }

        public override void ClientSend(int channelId, ArraySegment<byte> segment)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Server

        public override bool ServerActive()
        {
            throw new NotImplementedException();
        }

        public override bool ServerDisconnect(int connectionId)
        {
            throw new NotImplementedException();
        }

        public override string ServerGetClientAddress(int connectionId)
        {
            return Server.GetClientAddress(connectionId).ToString();
        }

        public override void ServerSend(int connectionId, int channelId, ArraySegment<byte> segment)
        {
            //FinalTransport.ServerSend.
        }

        public override void ServerStart()
        {
            Server.Start(maxPlayers, port);
        }

        public override void ServerStop()
        {
            Server.Stop();
        }

        public override Uri ServerUri()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Common

        public override int GetMaxPacketSize(int channelId = 0)
        {
            throw new NotImplementedException();
        }

        public override void Shutdown()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}