using System;
using System.IO;
using System.Threading;
using GameServer.Logic;
using GameServer.Rpc;
using GameServer.Storage;
using GameServer.Tools;
using JLM.NetSocket;

namespace GameServer
{
    public partial class GameServer
    {
        private static GameServer instance;
        public static GameServer Instance
        {
            get { return instance ?? (instance = new GameServer()); }
        }

        private NetServer server;
        public PlayerManager PlayerManager { get; private set; }
        public RankManager RankManager { get; private set; }
        private int clientIndex = 100;

        private C2SImplement netImpl;

        public GameServer()
        {
            server = new NetServer();
            this.server.Connected += new EventHandler<NetSocketConnectedEventArgs>(server_Connected);
            this.server.ConnectionRequested += new EventHandler<NetSockConnectionRequestEventArgs>(server_ConnectionRequested);
            this.server.DataArrived += new EventHandler<NetSockDataArrivalEventArgs>(server_DataArrived);
            this.server.Disconnected += new EventHandler<NetSocketDisconnectedEventArgs>(server_Disconnected);
            this.server.StateChanged += new EventHandler<NetSockStateChangedEventArgs>(server_StateChanged);

            netImpl = new C2SImplement();
            PlayerManager = new PlayerManager();
            RankManager = new RankManager();
        //    Disconnected = new EventHandler<NetSocketDisconnectedEventArgs>(local_Disconnected);
        }

        public void Run()
        {
            Logger.Log("Server Start");
            ServerInfoManager.Load();
            if (!Directory.Exists("log"))
                Directory.CreateDirectory("log");
            this.server.Listen(5555);

            while (true)
            {
                server.Oneloop();

                Thread.Sleep(50);
            }
        }

        private void server_StateChanged(object sender, NetSockStateChangedEventArgs e)
        {
            Logger.Log("State: " + e.PrevState.ToString() + " -> " + e.NewState.ToString());
        }

        private void server_DataArrived(object sender, NetSockDataArrivalEventArgs e)
        {
            netImpl.CheckPacket(e.Data, e.Net as NetClient);
        }

        private void server_ConnectionRequested(object sender, NetSockConnectionRequestEventArgs e)
        {
            Logger.Log("Connection Requested: " + ((System.Net.IPEndPoint)e.Client.RemoteEndPoint).Address.ToString());
            this.server.Accept(e.Client);
        }

        private void server_Connected(object sender, NetSocketConnectedEventArgs e)
        {
            var clientSock = sender as NetClient;
            clientSock.ClientId = clientIndex++;
            PlayerManager.AddPlayer(clientSock.ClientId, clientSock);
            Logger.Log("Connected: " + e.SourceIP + " id="+ clientSock.ClientId);
        }
        private void server_Disconnected(object sender, NetSocketDisconnectedEventArgs e)
        {
            var clientSock = sender as NetClient;
            PlayerManager.RemovePlayer(clientSock.ClientId);
            Logger.Log("Disconnected: id=" + clientSock.ClientId + " r=" + e.Reason);
        }

    }
}