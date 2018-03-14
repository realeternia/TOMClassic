#region Header
// **********
// ServUO - MessagePump.cs
// **********
#endregion

#region References

using System;
using System.Collections.Generic;
using System.Threading;

#endregion

namespace JLM.NetSocket
{
    public class MessagePump
    {
        private NetBase client;
        private ByteQueue receiveData;

        public MessagePump(NetBase client, ByteQueue queue)
        {
            this.client = client;
            receiveData = queue;
        }

        public void HandleReceive()
        {
            if (receiveData == null || receiveData.GetPacketLength() <= 0)
            {
                return;
            }

            lock (receiveData)
            {
                while (receiveData.GetPacketLength() > 4)
                {
                    if (receiveData.IsLegalPacket)
                    {
                        var totalSize = receiveData.GetPacketLength() + 4;
                        byte[] outData = new byte[totalSize];
                        receiveData.Dequeue(outData, 0, totalSize);
                        NetSockDataArrivalEventArgs arg = new NetSockDataArrivalEventArgs(client, PacketManager.GetPacket(outData));
                        if (client.DataArrived != null)
                            client.DataArrived(null, arg);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}