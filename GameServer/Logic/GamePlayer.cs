using JLM.NetSocket;

namespace GameServer.Logic
{
    public class GamePlayer
    {
        private NetClient net;
        public string Name { get; set; }

        public GamePlayer(NetClient client)
        {
            net = client;
        }

        public void Close(string reason)
        {
            if (net != null)
                net.Close(reason);
        }
    }
}