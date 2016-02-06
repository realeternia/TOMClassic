using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Config;

namespace TaleofMonsters.DataType.Cards.Weapons
{
    internal class Weapon
    {
        public WeaponConfig WeaponConfig;

        public int Id
        {
            get { return WeaponConfig.Id; }
        }

        public int Atk { get; set; }
        public int Def { get; set; }
        public int Dura { get; set; }

        public Weapon(int id)
        {
            WeaponConfig = ConfigData.GetWeaponConfig(id);
            Dura = (int)(WeaponConfig.Dura*1.67);
            UpgradeToLevel1();
        }

        public void AddAtk(int addon)
        {
            Atk += addon;
        }

        public void AddDef(int addon)
        {
            Def += addon;
        }

        public void AddStrengthLevel(int value)
        {
            int basedata = value*MathTool.GetSqrtMulti10(WeaponConfig.Star);
            if (WeaponConfig.Type == (int)CardTypeSub.Weapon || WeaponConfig.Type == (int)CardTypeSub.Scroll)
                Atk += 2*basedata/10;
            else if (WeaponConfig.Type == (int)CardTypeSub.Armor)
                Def += 1*basedata/10;
        }

        public void RemoveNegaPoint()
        {
            if (Atk < 0)
                Atk = 0;
            if (Def < 0)
                Def = 0;
        }

        public override string ToString()
        {
            string s = "";
            if (Atk != 0) s += string.Format("物攻+{0} ", Atk);
            if (Def != 0) s += string.Format("物防+{0} ", Def);
            if (WeaponConfig.SkillId != 0)
                s += string.Format("技能-{0}{1} ", ConfigData.GetSkillConfig(WeaponConfig.SkillId).Name, WeaponConfig.Percent == 100 ? "" : "(" + WeaponConfig.Percent + "%发动)");
            return s.Replace("+-", "-");
        }

        public CardProductMarkTypes GetSellMark()
        {
            CardProductMarkTypes mark = CardProductMarkTypes.Null;
            var cardData = CardConfigManager.GetCardConfig(WeaponConfig.Id);
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

        public void UpgradeToLevel1()
        {
            UpgradeToLevel(1);
        }

        public void UpgradeToLevel(int level)
        {
            int standardValue = (30 + WeaponConfig.Star * 10) * (level*8 + 92) / 100 * (100 + WeaponConfig.Modify) / 100;
            standardValue = (int)((float)standardValue * 4 / WeaponConfig.Dura * (1 + (WeaponConfig.Dura - 4) * 0.1));//耐久低的武器总值削减
            Atk = standardValue * (WeaponConfig.Atk) / 100;
            Def = standardValue * ( WeaponConfig.Def) / 100;
        }
    }
}