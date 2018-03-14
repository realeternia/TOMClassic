using System;
using TaleofMonsters.Core;
using System.IO;
using JLM.NetSocket;
using NarlonLib.Log;
using TaleofMonsters.Controler.Rpc;
using TaleofMonsters.Controler.World;

namespace TaleofMonsters.DataType.User
{
    internal static class UserProfile
    {
        public static string ProfileName { get; set; }
        public static Profile Profile { get; set; }
        private static NetClient client;
        private static NetworkImplement netImpl = new NetworkImplement();
        public static NetworkSender Sender;

        public static InfoBasic InfoBasic
        {
            get { return Profile.InfoBasic; }
        }

        public static InfoBag InfoBag
        {
            get { return Profile.InfoBag; }
        }

        public static InfoEquip InfoEquip
        {
            get { return Profile.InfoEquip; }
        }

        public static InfoCard InfoCard
        {
            get { return Profile.InfoCard; }
        }

        public static InfoDungeon InfoDungeon
        {
            get { return Profile.InfoDungeon; }
        }

        public static InfoGismo InfoGismo
        {
            get { return Profile.InfoGismo; }
        }

        public static InfoRival InfoRival
        {
            get { return Profile.InfoRival; }
        }

        public static InfoRecord InfoRecord
        {
            get { return Profile.InfoRecord; }
        }

        public static InfoQuest InfoQuest
        {
            get { return Profile.InfoQuest; }
        }

        public static InfoWorld InfoWorld
        {
            get { return Profile.InfoWorld; }
        }

        public static void Oneloop()
        {
            if (client != null)
                client.Oneloop();
        }

        public static void Connect()
        {
            System.Net.IPEndPoint end = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 5555);
            client = new NetClient();
            Sender = new NetworkSender(client);
            client.Connected += new EventHandler<NetSocketConnectedEventArgs>(client_Connected);
            client.DataArrived += DataArrived;
            client.Connect(end);
        }

        public static void Save()
        {
            var dts = DbSerializer.CustomTypeToBytes(Profile, typeof(Profile));
            Sender.Save(ProfileName, dts);
        }

        private static void DataArrived(object sender, NetSockDataArrivalEventArgs arg)
        {
            netImpl.CheckPacket(arg.Data, arg.Net);
        }

        private static void client_Connected(object sender, NetSocketConnectedEventArgs e)
        {
            NLog.Debug("Connected: " + e.SourceIP);
            Sender.Login(ProfileName);
        }
    }
}
