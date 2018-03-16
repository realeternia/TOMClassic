using JLM.NetSocket;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Rpc
{
    public class S2CImplement
    {
        public void CheckPacket(PacketBase packet, NetBase net)
        {
            switch (packet.PackRealId)
            {
                case PacketLoginResult.PackId: OnPacketLogin(packet as PacketLoginResult);break;
            }
        }

        public void OnPacketLogin(PacketLoginResult login)
        {
            if (login.SaveData.Length == 0)
            {
                UserProfile.Profile = new Profile();
                UserProfile.Profile.Pid = login.PlayerId;
            }
            else
            {
                object tmp;
                DbSerializer.BytesToCustomType(login.SaveData, out tmp, typeof(Profile));
                UserProfile.Profile = (Profile)tmp;
            }
            MainForm.Instance.LoginResult();
        }
    }
}