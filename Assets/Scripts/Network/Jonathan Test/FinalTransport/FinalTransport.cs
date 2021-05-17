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

        public static int ServerTicks { get => serverTicks; }

        public override bool Available()
        {
            return Application.platform != RuntimePlatform.WebGLPlayer;
        }

        private void Awake()
        {
            
        }

        #region Client
        public override void ClientConnect(string address)
        {
            //int newClientId = Server.clients.Count + 1;
            //Client.instance.ip = address;
            //Client.instance.ConnectToServer();
        }

        public override bool ClientConnected()
        {
            return Client.instance.IsConnected;
        }

        public override void ClientDisconnect()
        {
            Client.instance.tcp.Disconnect();
        }

        public override void ClientSend(int channelId, ArraySegment<byte> segment)
        {
            Packet packet = new Packet();
            packet.Write(segment.Array);
            Client.instance.udp.SendData(packet);
        }

        #endregion

        #region Server

        public override bool ServerActive()
        {
            throw new NotImplementedException();
        }

        public override bool ServerDisconnect(int connectionId)
        {
            if (Server.clients[connectionId] = null)
            {
                Server.clients[connectionId].Disconnect();
                return true;
            }
            else
            {
                return false;
            }
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