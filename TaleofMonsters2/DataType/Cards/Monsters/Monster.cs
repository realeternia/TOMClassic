using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Config;

namespace TaleofMonsters.DataType.Cards.Monsters
{
    internal class Monster
    {
        public MonsterConfig MonsterConfig;

        public int Id
        {
            get { return MonsterConfig.Id; }
        }

        public int Atk { get; set; }
        public int Def { get; set; }
        public int Spd { get; set; }
        public int Mag { get; set; }
        public int Luk { get; set; }
        public int Hp { get; set; }

        public int Level { get; set; }

        public string Name { get; set; }

        public Monster(int id)
        {
            MonsterConfig = ConfigData.GetMonsterConfig(id);
            Name = MonsterConfig.Name;
            UpgradeToLevel1();
        }

        public int GetAttrByIndex(PlayerAttrs attr)
        {
            switch (attr)
            {
                case PlayerAttrs.Atk: return Atk;
                case PlayerAttrs.Def: return Def;
                case PlayerAttrs.Mag: return Mag;
                case PlayerAttrs.Luk: return Luk;
                case PlayerAttrs.Spd: return Spd;
                case PlayerAttrs.Hp: return Hp;
            }
            return 0;
        }

        public CardProductMarkTypes GetSellMark()
        {
            CardProductMarkTypes mark = CardProductMarkTypes.Null;
            var cardData = CardConfigManager.GetCardConfig(MonsterConfig.Id);
            if (cardData.Quality == CardQualityTypes.Legend)
            {
                mark = CardProductMarkTypes.Only;
            }
            else if (cardData.Quality < CardQualityTypes.Excel && MathTool.GetRandom(10) > 7)
            {
                mark = CardProductMarkTypes.Sale;
            }
            else
            {
                int roll = MathTool.GetRandom(10);
                if (roll == 0)
                    mark = CardProductMarkTypes.Hot;
                else if (roll == 1)
                    mark = CardProductMarkTypes.Gold;
            }
            return mark;
        }

        private void UpgradeToLevel1()
        {
            UpgradeToLevel(1);
        }

        public void UpgradeToLevel(int level)
        {
            int standardValue = (30 + MonsterConfig.Star * 10) * (level*8 + 92) / 100 * (100 + MonsterConfig.Modify) / 100;
            Atk = standardValue*(100 + MonsterConfig.AtkP)/100; //200
            Def = standardValue * (100 + MonsterConfig.DefP) / 100; //200
            Mag = standardValue * (100 + MonsterConfig.MagP) / 100; //200
            Spd = standardValue * (100 + MonsterConfig.SpdP) / 100; //100
            Luk = standardValue * (100 + MonsterConfig.LukP) / 100;//无用属性
            Hp = standardValue * (100 + MonsterConfig.VitP) / 100 * 5; //200

            Level = level;
        }
    }
}
