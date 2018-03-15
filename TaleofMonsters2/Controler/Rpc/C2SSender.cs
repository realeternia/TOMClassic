using JLM.NetSocket;

namespace TaleofMonsters.Controler.Rpc
{
    public class C2SSender
    {
        private NetClient client;
        public C2SSender(NetClient client)
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