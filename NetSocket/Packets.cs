using System;
using System.IO;

namespace JLM.NetSocket
{
    public class PacketBase
    {
        public virtual int PackRealId { get; }
        public virtual byte[] Data { get; }
    }

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
            }
            return new PacketBase();
        }
    }

    public class PacketC2SLogin : PacketBase
    {
        public const int PackId = 100001;
        public string Name;

        public override int PackRealId { get { return PackId; } }

        public PacketC2SLogin(string name) 
        {
            Name = name;
        }

        public PacketC2SLogin(byte[] bts)
        {
            TBinaryReader sr = new TBinaryReader(bts);
            sr.ReadInt32(); //包id
            Name = sr.ReadString();
        }

        public override byte[] Data
        {
            get
            {
                TBinaryWriter sw = new TBinaryWriter();
                sw.Write(PackId);
                sw.Write(Name);
                return sw.GetBytes();
            }
        }
    }
    public class PacketC2SSave : PacketBase
    {
        public const int PackId = 100002;

        public string Passport;
        public byte[] SaveData;

        public override int PackRealId { get { return PackId; } }
        
        public PacketC2SSave(string passport, byte[] data)
        {
            Passport = passport;
            SaveData = data;
        }

        public PacketC2SSave(byte[] bts)
        {
            TBinaryReader sr = new TBinaryReader(bts);
            sr.ReadInt32(); //包id
            Passport = sr.ReadString();
            SaveData = sr.ReadByteArray();
        }

        public override byte[] Data
        {
            get
            {
                TBinaryWriter sw = new TBinaryWriter();
                sw.Write(PackId);
                sw.Write(Passport);
                sw.Write(SaveData);
                return sw.GetBytes();
            }
        }
    }
    public class PacketC2SLevelExpChange : PacketBase
    {
        public const int PackId = 100003;
        public int Job;
        public int Level;
        public int Exp;

        public override int PackRealId { get { return PackId; } }

        private PacketC2SLevelExpChange() { }

        public PacketC2SLevelExpChange(int job, int lv, int exp)
        {
            Job = job;
            Level = lv;
            Exp = exp;
        }

        public PacketC2SLevelExpChange(byte[] bts) : this()
        {
            TBinaryReader sr = new TBinaryReader(bts);
            sr.ReadInt32(); //包id
            Job = sr.ReadInt32();
            Level = sr.ReadInt32();
            Exp = sr.ReadInt32();
        }

        public override byte[] Data
        {
            get
            {
                TBinaryWriter sw = new TBinaryWriter();
                sw.Write(PackId);
                sw.Write(Job);
                sw.Write(Level);
                sw.Write(Exp);
                return sw.GetBytes();
            }
        }
    }
    public class PacketC2SGetRank : PacketBase
    {
        public const int PackId = 100004;
        public int Type;

        public override int PackRealId { get { return PackId; } }

        private PacketC2SGetRank() { }

        public PacketC2SGetRank(int lv)
        {
            Type = lv;
        }

        public PacketC2SGetRank(byte[] bts) : this()
        {
            TBinaryReader sr = new TBinaryReader(bts);
            sr.ReadInt32(); //包id
            Type = sr.ReadInt32();
        }

        public override byte[] Data
        {
            get
            {
                TBinaryWriter sw = new TBinaryWriter();
                sw.Write(PackId);
                sw.Write(Type);
                return sw.GetBytes();
            }
        }
    }

    public class PacketS2CLoginResult : PacketBase
    {
        public const int PackId = 200001;

        public int PlayerId;
        public byte[] SaveData;

        public override int PackRealId { get { return PackId; } }

        private PacketS2CLoginResult() { }

        public PacketS2CLoginResult(int playerId, byte[] data) : this()
        {
            PlayerId = playerId;
            SaveData = data;
        }

        public PacketS2CLoginResult(byte[] bts) : this()
        {
            TBinaryReader sr = new TBinaryReader(bts);
            sr.ReadInt32(); //包id
            PlayerId = sr.ReadInt32();
            SaveData = sr.ReadByteArray();
        }

        public override byte[] Data
        {
            get
            {
                TBinaryWriter sw = new TBinaryWriter();
                sw.Write(PackId);
                sw.Write(PlayerId);
                sw.Write(SaveData);
                return sw.GetBytes();
            }
        }
    }
}