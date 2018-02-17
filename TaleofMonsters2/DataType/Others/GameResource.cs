using System;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.Others
{
    public class GameResource
    {
        [FieldIndex(Index = 1)]
        public uint Gold;
        [FieldIndex(Index = 2)]
        public uint Lumber; //½¨ÖþÖÆ×÷£¬Å©³¡
        [FieldIndex(Index = 3)]
        public uint Stone; //½¨ÖþÖÆ×÷
        [FieldIndex(Index = 4)]
        public uint Mercury; //¹ºÂò×£¸££¬¹ºÂò·¨Êõ¿¨
        [FieldIndex(Index = 5)]
        public uint Carbuncle;//¹ºÂò¹ÖÎï¿¨£¬»ßÂ¸¹ÖÎï
        [FieldIndex(Index = 6)]
        public uint Sulfur; //Ë¢£¬ Á¶½ð(´ýÊµÏÖ)
        [FieldIndex(Index = 7)]
        public uint Gem; //¹ºÂòÎäÆ÷¿¨

        public GameResource()
            : this(0, 0, 0, 0, 0, 0, 0)
        {
        }

        public GameResource(uint gold, uint lumber, uint stone, uint mercury, uint carbuncle, uint sulfur, uint gem)
        {
            Gold = gold;
            Lumber = lumber;
            Stone = stone;
            Mercury = mercury;
            Carbuncle = carbuncle;
            Sulfur = sulfur;
            Gem = gem;
        }

        internal void Add(GameResourceType type, uint value)
        {
            switch ((int)type)
            {
                case 0: Gold += value; break;
                case 1: Lumber += value; break;
                case 2: Stone += value; break;
                case 3: Mercury += value; break;
                case 4: Carbuncle += value; break;
                case 5: Sulfur += value; break;
                case 6: Gem += value; break;
            }
        }

        internal void Sub(GameResourceType type, uint value)
        {
            switch ((int)type)
            {
                case 0: Gold = Math.Max(0, Gold - value); break;
                case 1: Lumber = Math.Max(0, Lumber - value); break;
                case 2: Stone = Math.Max(0, Stone - value); break;
                case 3: Mercury = Math.Max(0, Mercury - value); break;
                case 4: Carbuncle = Math.Max(0, Carbuncle - value); break;
                case 5: Sulfur = Math.Max(0, Sulfur - value); break;
                case 6: Gem = Math.Max(0, Gem - value); break;
            }
        }

        internal uint Get(GameResourceType type)
        {
            switch ((int)type)
            {
                case 0: return Gold;
                case 1: return Lumber;
                case 2: return Stone;
                case 3: return Mercury;
                case 4: return Carbuncle;
                case 5: return Sulfur;
                case 6: return Gem;
            }
            return 0;
        }

        internal bool Has(GameResourceType type, int value)
        {
            switch ((int)type)
            {
                case 0: return Gold >= value;
                case 1: return Lumber >= value;
                case 2: return Stone >= value;
                case 3: return Mercury >= value;
                case 4: return Carbuncle >= value;
                case 5: return Sulfur >= value;
                case 6: return Gem >= value;
            }
            return false;
        }

        public static GameResource Parse(string str)
        {
            GameResource res = new GameResource();
            string[] datas = str.Split('|');
            foreach (string data in datas)
            {
                if (data == "")
                    break;

                string[] infos = data.Split(';');

                uint value = uint.Parse(infos[1]);
                switch (int.Parse(infos[0]))
                {
                    case 1: res.Gold += value; break;
                    case 2: res.Lumber += value; break;
                    case 3: res.Stone += value; break;
                    case 4: res.Mercury += value; break;
                    case 5: res.Carbuncle += value; break;
                    case 6: res.Sulfur += value; break;
                    case 7: res.Gem += value; break;
                }
            }
            return res;
        }

        public uint[] ToArray()
        {
            uint[] rt = new uint[7];
            rt[0] = Gold;
            rt[1] = Lumber;
            rt[2] = Stone;
            rt[3] = Mercury;
            rt[4] = Carbuncle;
            rt[5] = Sulfur;
            rt[6] = Gem;
            return rt;
        }
    }

}
