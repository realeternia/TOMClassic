using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Core.Config;

namespace TaleofMonsters.Datas.Cards.Weapons
{
    internal class Weapon
    {
        public WeaponConfig WeaponConfig;

        public int Id { get { return WeaponConfig.Id; } }
        public int Level { get; set; }

        public int Atk { get; set; }
        public int PArmor { get; set; }
        public int MArmor { get; set; }

        public int Dura { get; set; }
        public int Range { get; set; }
        public int Mov { get; set; }

        public int Spd { get; set; }
        public int Def { get; set; }
        public int Mag { get; set; }
        public int Luk { get; set; }
        public int Hit { get; set; }
        public int Dhit { get; set; }
        public int Crt { get; set; }

        public Weapon(int id)
        {
            WeaponConfig = ConfigData.GetWeaponConfig(id);
            if ((CardTypeSub) WeaponConfig.Type != CardTypeSub.Relic)
                Dura = (int) (WeaponConfig.Dura*1.67);
            else
                Dura = WeaponConfig.Dura;
            Range = WeaponConfig.Range; 
            Def = WeaponConfig.Def;
            Spd = WeaponConfig.Spd;
            Mag = WeaponConfig.Mag;
            Luk = WeaponConfig.Luk;
            Hit = WeaponConfig.Hit;
            Dhit = WeaponConfig.Dhit;
            Crt = WeaponConfig.Crt;
            Mov = WeaponConfig.Mov;

            UpgradeToLevel1();
        }

        public CardProductMarkTypes GetSellMark()
        {
            CardProductMarkTypes mark = CardProductMarkTypes.Null;
            var cardData = CardConfigManager.GetCardConfig(WeaponConfig.Id);
            if (cardData.Quality == QualityTypes.Legend)
            {
                mark = CardProductMarkTypes.Only;
            }
            else if (cardData.Quality < QualityTypes.Excel && MathTool.GetRandom(10) > 7)
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

        public void UpgradeToLevel1()
        {
            UpgradeToLevel(1);
        }

        public void UpgradeToLevel(int level)
        {
            Level = level;

            var standardValue = CardAssistant.GetCardModify(WeaponConfig.Star, level, (QualityTypes)WeaponConfig.Quality, WeaponConfig.Modify);
            standardValue = (int)((float)standardValue * 4 / WeaponConfig.Dura * (1 + (WeaponConfig.Dura - 4) * 0.1));//耐久低的武器总值削减
            Atk = standardValue * (WeaponConfig.AtkP) / 100;
            PArmor = standardValue * (WeaponConfig.PArmor) / 100*5;
            MArmor = standardValue * (WeaponConfig.MArmor) / 100 * 5;

            if (Range > 10)
                Atk = (int)(Atk * CardAssistant.GetCardFactorOnRange(Range));
            if (Mov > 10)
                Atk = (int)(Atk * CardAssistant.GetCardFactorOnMove(Mov));
        }
    }
}