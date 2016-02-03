namespace NarlonLib.Net
{
    class NetPackets
    {
        static OByteArray getDataPack(short procId, OByteArray data)
        {
            OByteArray ba = new OByteArray();
            ba.WriteByte(1); //PROC
            ba.WriteShort(procId);
            ba.WriteInt32(data.Datas.Length);
            ba.WriteBytes(data.Datas);

            return ba;
        }

        public static OByteArray EmptyPacket()
        {
            OByteArray pack = new OByteArray();
            pack.WriteBytes(new byte[23]);

            return pack;
        }

    }
}
