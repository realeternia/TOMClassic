using System.Collections.Generic;
using System.IO;

namespace JLM.NetSocket
{
    public class PacketBase
    {
        public virtual int PackRealId { get; }
        public virtual byte[] Data { get; }
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

        public PacketC2SLevelExpChange(int job, int lv, int exp)
        {
            Job = job;
            Level = lv;
            Exp = exp;
        }

        public PacketC2SLevelExpChange(byte[] bts)
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

        public PacketC2SGetRank(int lv)
        {
            Type = lv;
        }

        public PacketC2SGetRank(byte[] bts)
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
    public class PacketC2SSendPlayerInfo : PacketBase
    {
        public const int PackId = 100005;
        public string Name;
        public int HeadId;

        public override int PackRealId { get { return PackId; } }

        public PacketC2SSendPlayerInfo(string name, int headId)
        {
            Name = name;
            HeadId = headId;
        }

        public PacketC2SSendPlayerInfo(byte[] bts)
        {
            TBinaryReader sr = new TBinaryReader(bts);
            sr.ReadInt32(); //包id
            Name = sr.ReadString();
            HeadId = sr.ReadInt32();
        }

        public override byte[] Data
        {
            get
            {
                TBinaryWriter sw = new TBinaryWriter();
                sw.Write(PackId);
                sw.Write(Name);
                sw.Write(HeadId);
                return sw.GetBytes();
            }
        }
    }
    public class PacketC2SSendHeartbeat : PacketBase
    {
        public const int PackId = 100006;
        public int Token;

        public override int PackRealId { get { return PackId; } }

        public PacketC2SSendHeartbeat()
        {

        }

        public PacketC2SSendHeartbeat(byte[] bts)
        {
            TBinaryReader sr = new TBinaryReader(bts);
            sr.ReadInt32(); //包id
            Token = sr.ReadInt32();
        }

        public override byte[] Data
        {
            get
            {
                TBinaryWriter sw = new TBinaryWriter();
                sw.Write(PackId);
                sw.Write(Token);
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

        public PacketS2CLoginResult(int playerId, byte[] data)
        {
            PlayerId = playerId;
            SaveData = data;
        }

        public PacketS2CLoginResult(byte[] bts)
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
    public class PacketS2CRankResult : PacketBase
    {
        public const int PackId = 200002;

        public List<RankData> RankList;

        public override int PackRealId { get { return PackId; } }

        public PacketS2CRankResult(List<RankData> data) 
        {
            RankList = data;
        }

        public PacketS2CRankResult(byte[] bts)
        {
            TBinaryReader sr = new TBinaryReader(bts);
            sr.ReadInt32(); //包id
            RankList = new List<RankData>();
            var length = sr.ReadInt32();
            for (int i = 0; i < length; i++)
            {
                var dData = new RankData();
                dData.Read(sr);
                RankList.Add(dData);
            }
        }

        public override byte[] Data
        {
            get
            {
                TBinaryWriter sw = new TBinaryWriter();
                sw.Write(PackId);
                sw.Write(RankList.Count);
                foreach (var rankData in RankList)
                    rankData.Write(sw);
                return sw.GetBytes();
            }
        }
    }
    public class PacketS2CReplyHeartbeat : PacketBase
    {
        public const int PackId = 200003;
        public int Token;
        public override int PackRealId { get { return PackId; } }

        public PacketS2CReplyHeartbeat()
        {

        }

        public PacketS2CReplyHeartbeat(byte[] bts)
        {
            TBinaryReader sr = new TBinaryReader(bts);
            sr.ReadInt32(); //包id
            Token = sr.ReadInt32();
        }

        public override byte[] Data
        {
            get
            {
                TBinaryWriter sw = new TBinaryWriter();
                sw.Write(PackId);
                sw.Write(Token);
                return sw.GetBytes();
            }
        }
    }
}