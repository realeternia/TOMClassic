using JLM.NetSocket;
using NarlonLib.Log;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.CMain;

namespace TaleofMonsters.Rpc
{
    public class S2CImplement
    {
        public void CheckPacket(PacketBase packet, NetBase net)
        {
            switch (packet.PackRealId)
            {
                case PacketS2CLoginResult.PackId: OnPacketLogin(packet as PacketS2CLoginResult);break;
                case PacketS2CRankResult.PackId: OnPacketRankResult(packet as PacketS2CRankResult); break;
                case PacketS2CReplyHeartbeat.PackId: OnPacketReplyHeartbeat(packet as PacketS2CReplyHeartbeat); break;
            }
        }

        public void OnPacketLogin(PacketS2CLoginResult s2CLogin)
        {
            if (s2CLogin.SaveData.Length == 0)
            {
                UserProfile.Profile = new Profile();
                UserProfile.Profile.Pid = s2CLogin.PlayerId;
            }
            else
            {
                object tmp;
                DbSerializer.BytesToCustomType(s2CLogin.SaveData, out tmp, typeof(Profile));
                UserProfile.Profile = (Profile)tmp;
                TalePlayer.C2SSender.UpdatePlayerInfo(UserProfile.Profile.Name, UserProfile.Profile.InfoBasic.Head);
            }
            MainForm.Instance.LoginResult();
        }

        public void OnPacketRankResult(PacketS2CRankResult s2CData)
        {
            NetDataCache.RankList = s2CData.RankList;
            PanelManager.SendMessage(typeof(RankForm), 1);
        }
        public void OnPacketReplyHeartbeat(PacketS2CReplyHeartbeat s2CData)
        {
           // NLog.Debug("HB RECV");
        }
    }
}