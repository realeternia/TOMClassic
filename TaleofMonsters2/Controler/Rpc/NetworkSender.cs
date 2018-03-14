using JLM.NetSocket;

namespace TaleofMonsters.Controler.Rpc
{
    public class NetworkSender
    {
        private NetClient client;
        public NetworkSender(NetClient client)
        {
            this.client = client;
        }

        public void Login(string name)
        {
            client.Send(new PacketLogin(name).Data);
        }

        public void Save(string name, byte[] dats)
        {
            var data = new PacketSave(name, dats).Data;
            client.Send(data);
        }
    }
}