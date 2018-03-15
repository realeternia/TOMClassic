using System;
using System.IO;
using System.Threading;
using GameServer.Rpc;
using GameServer.Storage;
using GameServer.Tools;
using JLM.NetSocket;

namespace GameServer
{
    public partial class GameServer
    {
        private NetServer server = new NetServer();

        private C2SImplement netImpl;

        public GameServer()
        {
            this.server.Connected += new EventHandler<NetSocketConnectedEventArgs>(server_Connected);
            this.server.ConnectionRequested += new EventHandler<NetSockConnectionRequestEventArgs>(server_ConnectionRequested);
            this.server.DataArrived += new EventHandler<NetSockDataArrivalEventArgs>(server_DataArrived);
            this.server.Disconnected += new EventHandler<NetSocketDisconnectedEventArgs>(server_Disconnected);
            this.server.ErrorReceived += new EventHandler<NetSockErrorReceivedEventArgs>(server_ErrorReceived);
            this.server.StateChanged += new EventHandler<NetSockStateChangedEventArgs>(server_StateChanged);

            netImpl = new C2SImplement();
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

        private void server_ErrorReceived(object sender, NetSockErrorReceivedEventArgs e)
        {
            if (e.Exception.GetType() == typeof(System.Net.Sockets.SocketException))
            {
                System.Net.Sockets.SocketException s = (System.Net.Sockets.SocketException)e.Exception;
                Logger.Log("Error: " + e.Function + " - " + s.SocketErrorCode.ToString() + "\r\n" + s.ToString());
            }
            else
                Logger.Log("Error: " + e.Function + "\r\n" + e.Exception.ToString());
        }

        private void server_Disconnected(object sender, NetSocketDisconnectedEventArgs e)
        {
            Logger.Log("Disconnected: " + e.Reason);
        }

        private void server_DataArrived(object sender, NetSockDataArrivalEventArgs e)
        {
            netImpl.CheckPacket(e.Data, e.Net);
        }

        private void server_ConnectionRequested(object sender, NetSockConnectionRequestEventArgs e)
        {
            Logger.Log("Connection Requested: " + ((System.Net.IPEndPoint)e.Client.RemoteEndPoint).Address.ToString());
            this.server.Accept(e.Client);
        }

        private void server_Connected(object sender, NetSocketConnectedEventArgs e)
        {
            Logger.Log("Connected: " + e.SourceIP);
        }
    }
}