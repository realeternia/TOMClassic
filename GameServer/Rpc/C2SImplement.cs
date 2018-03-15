using GameServer.Storage;
using GameServer.Tools;
using JLM.NetSocket;

namespace GameServer.Rpc
{
    public class C2SImplement
    {
        public void CheckPacket(PacketBase packet, NetBase net)
        {
            switch (packet.PackRealId)
            {
                case PacketLogin.PackId: OnPacketLogin(net, packet as PacketLogin); break;
                case PacketSave.PackId: OnPacketSave(net, packet as PacketSave); break;
                default: Logger.Log(string.Format("CheckPacket error id={0}", packet.PackRealId));
                    net.Close("error packet");
                    break;
            }
        }

        public void OnPacketLogin(NetBase net, PacketLogin login)
        {
            Logger.Log("OnPacketLogin " + login.Name);
            var datas = DbManager.LoadFromDB(login.Name);
            net.Send(new PacketLoginResult(datas.Length==0 ? ServerInfoManager.GetPlayerPid() : 0, datas).Data);
        }

        public void OnPacketSave(NetBase net, PacketSave save)
        {
            Logger.Log("OnPacketSave " + save.Passport);
            DbManager.SaveToDB(save.Passport, save.SaveData);
        }
    }
}