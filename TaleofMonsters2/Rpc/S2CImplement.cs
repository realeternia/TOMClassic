using JLM.NetSocket;
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
            }
            MainForm.Instance.LoginResult();
        }

        public void OnPacketRankResult(PacketS2CRankResult s2CData)
        {
            NetDataCache.RankList = s2CData.RankList;
            PanelManager.SendMessage(typeof(RankForm), 1);
        }
    }
}