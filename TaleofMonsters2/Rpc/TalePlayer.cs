using System;
using System.Collections;
using JLM.NetSocket;
using NarlonLib.Core;
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

        private static NLTimerManager timerManager;
        private static NLCoroutineManager coroutineManager;

        private static DateTime lastHeartbeatTime = DateTime.Now;
        private static bool hasConnect;

        static TalePlayer()
        {
            timerManager = new NLTimerManager();
            coroutineManager = new NLCoroutineManager(timerManager);
        }

        public static void Oneloop()
        {
            timerManager.DoTimer();

            if (hasConnect && client != null)
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
                    CheckHeartbeat();
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

            System.Net.IPEndPoint end = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("193.112.9.47"), 5555);
            client = new NetClient();
            C2SSender = new C2SSender(client);
            client.Connected += new EventHandler<NetSocketConnectedEventArgs>(client_Connected);
            client.DataArrived += DataArrived;
            if (!client.Connect(end))
                MainForm.Instance.ShowDisconnectSafe("无法连接到服务器");
        }

        private static void client_Connected(object sender, NetSocketConnectedEventArgs e)
        {
            NLog.Debug("Connected: " + e.SourceIP);
            C2SSender.Login(UserProfile.ProfileName);
            hasConnect = true;
        }

        public static void Close()
        {
            hasConnect = false;
            if (client != null && client.State == SocketState.Connected)
                client.Close("Connect");
        }

        public static void Start(IEnumerator routine)
        {
            coroutineManager.StartCoroutine(routine);
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

        private static void CheckHeartbeat()
        {
            if ((DateTime.Now - lastHeartbeatTime).TotalSeconds > 10)
            {
                C2SSender.SendHeartbeat();
                lastHeartbeatTime = DateTime.Now;
            }
        }

    }
}