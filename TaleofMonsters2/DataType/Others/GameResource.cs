using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.Others
{
    public class GameResource
    {
        [FieldIndex(Index = 1)]
        public int Gold;
        [FieldIndex(Index = 2)]
        public int Lumber;
        [FieldIndex(Index = 3)]
        public int Stone;
        [FieldIndex(Index = 4)]
        public int Mercury;//锻造使用，分解装备获得
        [FieldIndex(Index = 5)]
        public int Carbuncle;
        [FieldIndex(Index = 6)]
        public int Sulfur;
        [FieldIndex(Index = 7)]
        public int Gem;//合成卡牌用，分解卡牌获得

        public GameResource()
            : this(0, 0, 0, 0, 0, 0, 0)
        {
        }

        public GameResource(int gold, int lumber, int stone, int mercury, int carbuncle, int sulfur, int gem)
        {
            Gold = gold;
            Lumber = lumber;
            Stone = stone;
            Mercury = mercury;
            Carbuncle = carbuncle;
            Sulfur = sulfur;
            Gem = gem;
        }

        internal void Add(GameResourceType type, int value)
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

                int value = int.Parse(infos[1]);
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

        public int[] ToArray()
        {
            int[] rt = new int[7];
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
