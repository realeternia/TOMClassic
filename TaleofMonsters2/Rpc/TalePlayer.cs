using System;
using JLM.NetSocket;
using NarlonLib.Log;
using TaleofMonsters.Controler.World;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Rpc
{
    public static class TalePlayer
    {
        private static NetClient client;

        private static S2CImplement s2cImpl = new S2CImplement();
        public static C2SSender C2SSender;

        public static void Oneloop()
        {
            if (client != null)
            {
                if (client.State == SocketState.Closed || client.State == SocketState.Closing)
                {
                    MainForm.Instance.ShowDisconnectSafe("已经与服务器断开连接");
                    client = null;
                    return;
                }

                try
                {
                    client.Oneloop();
                }
                catch (Exception e)
                {
                    NLog.Debug(e);
                }
            }
        }

        public static void Connect()
        {
            Close();

            System.Net.IPEndPoint end = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 5555);
            client = new NetClient();
            C2SSender = new C2SSender(client);
            client.Connected += new EventHandler<NetSocketConnectedEventArgs>(client_Connected);
            client.DataArrived += DataArrived;
            client.Connect(end);
        }

        public static void Close()
        {
            if (client != null && client.State == SocketState.Connected)
                client.Close("Connect");
        }

        public static void Save()
        {
            var dts = DbSerializer.CustomTypeToBytes(UserProfile.Profile, typeof(Profile));
            C2SSender.Save(UserProfile.ProfileName, dts);
            WorldInfoManager.Save();
        }

        private static void DataArrived(object sender, NetSockDataArrivalEventArgs arg)
        {
            s2cImpl.CheckPacket(arg.Data, arg.Net);
        }

        private static void client_Connected(object sender, NetSocketConnectedEventArgs e)
        {
            NLog.Debug("Connected: " + e.SourceIP);
            C2SSender.Login(UserProfile.ProfileName);
        }
    }
}