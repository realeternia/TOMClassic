using JLM.NetSocket;

namespace GameServer
{
    public class NetworkImplement
    {
        public void CheckPacket(PacketBase packet, NetBase net)
        {
            switch (packet.PackRealId)
            {
                case PacketLogin.PackId: OnPacketLogin(net, packet as PacketLogin); break;
                case PacketSave.PackId: OnPacketSave(net, packet as PacketSave); break;
            }
        }

        public void OnPacketLogin(NetBase net, PacketLogin login)
        {
            Logger.Log("OnPacketLogin " + login.Name);
            var datas = DbManager.LoadFromDB(login.Name);
            net.Send(new PacketLoginResult(datas,1).Data);
        }

        public void OnPacketSave(NetBase net, PacketSave save)
        {
            Logger.Log("OnPacketSave " + save.Passport);
            DbManager.SaveToDB(save.Passport, save.SaveData);
        }
    }
}