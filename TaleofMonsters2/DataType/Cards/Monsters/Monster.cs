using System;
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
        public int Hp { get; set; }//成长属性

        public int Def { get; set; }
        public int Spd { get; set; }
        public int Mag { get; set; }
        public int Luk { get; set; }
        public int Hit { get; set; }
        public int Dhit { get; set; }
        public int Crt { get; set; }
        public int Mov { get; set; }
        public int Range { get; set; }

        public int Level { get; set; }

        public string Name { get; set; }

        public Monster(int id)
        {
            MonsterConfig = ConfigData.GetMonsterConfig(id);
            Def = MonsterConfig.Def;
            Spd = MonsterConfig.Spd;
            Mag = MonsterConfig.Mag;
            Luk = MonsterConfig.Luk;
            Hit = MonsterConfig.Hit;
            Dhit = MonsterConfig.Dhit;
            Crt = MonsterConfig.Crt;
            Mov = MonsterConfig.Mov;
            Range = MonsterConfig.Range;

            Name = MonsterConfig.Name;
            UpgradeToLevel1();
        }

        public int GetAttrByIndex(PlayerAttrs attr)
        {
            switch (attr)
            {
                case PlayerAttrs.Atk: return Atk;
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

        private static float[] rangeAtkPunish = new float[]{1,1,0.75f,0.68f,0.62f,0.56f,0.52f,0.48f,0.44f,0.42f,0.4f};
        private static float[] rangeHpPunish = new float[] { 1, 1, 0.8f, 0.75f, 0.7f, 0.65f, 0.57f, 0.54f, 0.52f, 0.5f, 0.48f };

        public void UpgradeToLevel(int level)
        {
            int standardValue = (30 + MonsterConfig.Star * 10) * (level*8 + 92) / 100 * (100 + MonsterConfig.Modify) / 100;
            Atk = standardValue * (100 + MonsterConfig.AtkP) / 100; //200
            Hp = standardValue * (100 + MonsterConfig.VitP) / 100 * 5; //200
            if (MonsterConfig.Range>10)
            {
                Atk = (int)(Atk * rangeAtkPunish[MonsterConfig.Range / 10]);
                Hp = (int)(Hp * rangeHpPunish[MonsterConfig.Range / 10]);
            }

            Level = level;
        }
    }
}
