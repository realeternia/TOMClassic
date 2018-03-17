using GameServer.Rpc;
using JLM.NetSocket;

namespace GameServer.Logic
{
    public class GamePlayer
    {
        private NetClient net;
        public string Passport { get; set; }
        public string Name { get; set; }
        public int HeadId { get; set; }
        public S2CSender S2C { get; private set; }

        public GamePlayer(NetClient client)
        {
            net = client;
            S2C = new S2CSender(net);
        }

        public void Close(string reason)
        {
            if (net != null)
                net.Close(reason);
        }
    }
}