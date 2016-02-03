using System;
using System.Collections.Generic;

namespace ConfigDatas
{
    public struct RLIdValue
    {
        public int Id;
        public int Value;

        public static RLIdValue Parse(string str)
        {
            RLIdValue data = new RLIdValue();
            string[] datas = str.Split(';');

            data.Id = int.Parse(datas[0]);
            data.Value = int.Parse(datas[1]);
            return data;
        }
    }

    public struct RLIdValueList
    {
        private RLIdValue[] list;

        public RLIdValue this[int index]
        {
            get { return list[index]; }
            set { list[index] = value; }
        }

        public RLIdValueList(RLIdValue[] data)
        {
            list = data;
        }

        public int Count
        {
            get { return list.Length; }
        }

        public static RLIdValueList Parse(string str)
        {
            RLIdValueList data = new RLIdValueList();
            string[] datas = str.Split('|');

            if (str != "" && datas.Length > 0)
            {
                data.list = new RLIdValue[datas.Length];
                for (int i = 0; i < datas.Length; i++)
                {
                    data.list[i] = RLIdValue.Parse(datas[i]);
                }
            }
            else
            {
                data.list = new RLIdValue[] {};
            }

            return data;
        }
    }

    public struct RLVector3
    {
        public int X;
        public int Y;
        public int Z;

        public static RLVector3 Parse(string str)
        {
            RLVector3 data = new RLVector3();
            string[] datas = str.Split(';');

            data.X = int.Parse(datas[0]);
            data.Y = int.Parse(datas[1]);
            if (datas.Length == 3)
                data.Z = int.Parse(datas[2]);
            return data;
        }
    }

    public struct RLVector3List
    {
        private RLVector3[] list;

        public RLVector3 this[int index]
        {
            get { return list[index]; }
            set { list[index] = value; }
        }

        public RLVector3List(RLVector3[] data)
        {
            list = data;
        }

        public int Count
        {
            get { return list == null ? 0 : list.Length; }
        }

        public static RLVector3List Parse(string str)
        {
            RLVector3List data = new RLVector3List();
            string[] datas = str.Split('|');

            if (str != "" && datas.Length > 0)
            {
                data.list = new RLVector3[datas.Length];
                for (int i = 0; i < datas.Length; i++)
                {
                    data.list[i] = RLVector3.Parse(datas[i]);
                }
            }
            else
            {
                data.list = new RLVector3[] {};
            }

            return data;
        }
    }

    public struct RLIItemRateCount
    {
        public int Type;
        public int Id;
        public int RollMin;
        public int RollMax;
        public int Count;

        public static RLIItemRateCount Parse(string str)
        {
            RLIItemRateCount data = new RLIItemRateCount();
            string[] datas = str.Split(';');

            data.Type = int.Parse(datas[0]);
            data.Id = int.Parse(datas[1]);
            data.RollMin = int.Parse(datas[2]);
            data.RollMax = int.Parse(datas[3]);
            data.Count = int.Parse(datas[4]);
            return data;
        }
    }

    public struct RLIItemRateCountList
    {
        private RLIItemRateCount[] list;

        public RLIItemRateCount this[int index]
        {
            get { return list[index]; }
            set { list[index] = value; }
        }

        public int Count
        {
            get { return list.Length; }
        }

        public static RLIItemRateCountList Parse(string str)
        {
            RLIItemRateCountList data = new RLIItemRateCountList();
            string[] datas = str.Split('|');
            data.list = new RLIItemRateCount[datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                data.list[i] = RLIItemRateCount.Parse(datas[i]);
            }

            return data;
        }
    }

    public enum DamageTypes
    {
        Magic,
        Physical,
        All
    };

    public struct RLXY
    {
        public int X;
        public int Y;

        public static RLXY Parse(string str)
        {
            RLXY data = new RLXY();
            string[] datas = str.Split(';');

            data.X = int.Parse(datas[0]);
            data.Y = int.Parse(datas[1]);
            return data;
        }
    }

    public class SkillActiveType
    {
        public static int Active = 0;
        public static int Passive = 1;
        public static int Both = 2;

        public static int Parse(string data)
        {
            if (data == "Active")
            {
                return Active;
            }
            if (data == "Passive")
            {
                return Passive;
            }
            return Both;
        }
    }

    public enum SkillMarks
    {
        HitHigh = 1,
        HitLow = 2,
        AtkDefBonus = 3,
    }

    public enum PlayerManaTypes
    {
        None,
        Mana = 1,//魔力
        Power = 2,//力量
        LeaderShip = 3,//领导
    }
}
