using GameServer.Storage;
using GameServer.Tools;
using JLM.NetSocket;

namespace GameServer.Rpc
{
    public class C2SImplement
    {
        public void CheckPacket(PacketBase packet, NetClient net)
        {
            switch (packet.PackRealId)
            {
                case PacketC2SLogin.PackId: OnPacketLogin(net, packet as PacketC2SLogin); break;
                case PacketC2SSave.PackId: OnPacketSave(net, packet as PacketC2SSave); break;
                case PacketC2SLevelExpChange.PackId: OnPacketLevelExpChange(net, packet as PacketC2SLevelExpChange); break;
                case PacketC2SGetRank.PackId: OnPacketGetRank(net, packet as PacketC2SGetRank); break;
                case PacketC2SSendPlayerInfo.PackId: OnPacketSendPlayerInfo(net, packet as PacketC2SSendPlayerInfo); break;
                case PacketC2SSendHeartbeat.PackId: OnPacketSendHeartbeat(net, packet as PacketC2SSendHeartbeat); break;
                default: Logger.Log(string.Format("CheckPacket error id={0}", packet.PackRealId));
                    net.Close("error packet");
                    break;
            }
        }

        public void OnPacketLogin(NetClient net, PacketC2SLogin c2SLogin)
        {
            Logger.Log("OnPacketLogin " + c2SLogin.Name);
            GameServer.Instance.PlayerManager.SetName(net.ClientId, c2SLogin.Name);
            var datas = DbManager.LoadFromDB(c2SLogin.Name);
            net.Send(new PacketS2CLoginResult(datas.Length==0 ? ServerInfoManager.GetPlayerPid() : 0, datas).Data);
        }

        public void OnPacketSave(NetBase net, PacketC2SSave c2SSave)
        {
            Logger.Log("OnPacketSave " + c2SSave.Passport);
            DbManager.SaveToDB(c2SSave.Passport, c2SSave.SaveData);
        }

        public void OnPacketLevelExpChange(NetClient net, PacketC2SLevelExpChange c2SData)
        {
            var player = GameServer.Instance.PlayerManager.GetPlayer(net.ClientId);
            if (player != null)
            {
                GameServer.Instance.RankManager.RpcUpdateLevelExp(player.Name, player.HeadId, c2SData.Job, c2SData.Level, c2SData.Exp);
            }
        }
        public void OnPacketGetRank(NetClient net, PacketC2SGetRank c2SData)
        {
            var player = GameServer.Instance.PlayerManager.GetPlayer(net.ClientId);
            if (player != null)
            {
                GameServer.Instance.RankManager.RpcGetRank(player, c2SData.Type);
            }
        }
        public void OnPacketSendPlayerInfo(NetClient net, PacketC2SSendPlayerInfo c2SData)
        {
            var player = GameServer.Instance.PlayerManager.GetPlayer(net.ClientId);
            if (player != null)
            {
                player.Name = c2SData.Name;
                player.HeadId = c2SData.HeadId;
            }
        }
        public void OnPacketSendHeartbeat(NetClient net, PacketC2SSendHeartbeat c2SData)
        {
            var player = GameServer.Instance.PlayerManager.GetPlayer(net.ClientId);
            if (player != null)
            {
                player.S2C.ReplyHeartbeat();
            }
        }
    }
}