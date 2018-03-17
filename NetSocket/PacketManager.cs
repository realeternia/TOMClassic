using System;

namespace JLM.NetSocket
{
    public class PacketManager
    {
        public static PacketBase GetPacket(byte[] datas)
        {
            if (datas.Length <= 8) //size = 4, id = 4
                return new PacketBase();

            var packId = (uint)(datas[4] | datas[5] << 8 | datas[6] << 16 | datas[7] << 24);
            byte[] newData = new byte[datas.Length - 4];
            Buffer.BlockCopy(datas, 4, newData, 0, newData.Length);

            switch (packId)
            {
                case PacketC2SLogin.PackId: return new PacketC2SLogin(newData);
                case PacketC2SSave.PackId: return new PacketC2SSave(newData);
                case PacketC2SLevelExpChange.PackId: return new PacketC2SLevelExpChange(newData);
                case PacketC2SGetRank.PackId: return new PacketC2SGetRank(newData);

                case PacketS2CLoginResult.PackId: return new PacketS2CLoginResult(newData);
                case PacketS2CRankResult.PackId: return new PacketS2CRankResult(newData);
            }
            return new PacketBase();
        }
    }
}