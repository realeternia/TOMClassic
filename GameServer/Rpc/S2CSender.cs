using System.Collections.Generic;
using JLM.NetSocket;

namespace GameServer.Rpc
{
    public class S2CSender
    {
        private NetClient client;
        public S2CSender(NetClient client)
        {
            this.client = client;
        }

        public void GetRankResult(List<RankData> rankList)
        {
            var data = new PacketS2CRankResult(rankList).Data;
            client.Send(data);
        }
        public void ReplyHeartbeat()
        {
            var data = new PacketS2CReplyHeartbeat().Data;
            client.Send(data);
        }
    }
}