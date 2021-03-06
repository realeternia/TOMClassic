using System;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Core.Config;

namespace TaleofMonsters.Datas.Cards.Spells
{
    internal class Spell : ISpell
    {
        public SpellConfig SpellConfig;
        public int Id { get { return SpellConfig.Id; } }

        public int Level { get; set; }

        public int Damage { get; set; }
        public int Cure { get; set; }
        public double Time { get; set; }
        public double Help { get; set; }
        public double Rate { get; set; }//100表示100%
        public int Atk { get; set; }

        public int Attr {
            get { return SpellConfig.Attr; }
        }

        public double Addon { get; set; }//法术强化造成的效果

        public Spell(int sid)
        {
            SpellConfig = ConfigData.GetSpellConfig(sid);
            UpgradeToLevel1();
        }

        public string Target
        {
            get { return SpellConfig.Target.Substring(1,1); }
        }

        public string Shape
        {
            get { return SpellConfig.Target.Substring(2, 1); }
        }

        public int Range
        {
            get { return SpellConfig.Range; }
        }

        public string Descript
        {
            get
            {
                if (SpellConfig.GetDescript == null)
                    return "";
                return string.Format(SpellConfig.GetDescript, Damage, Cure, Time, Help, Rate, Atk);
            }
        }

        public CardProductMarkTypes GetSellMark()
        {
            CardProductMarkTypes mark = CardProductMarkTypes.Null;
            var cardData = CardConfigManager.GetCardConfig(SpellConfig.Id);
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

            var standardValue = CardAssistant.GetCardModify(SpellConfig.Star, level, (QualityTypes)SpellConfig.Quality, SpellConfig.Modify);
            Damage = standardValue * (SpellConfig.Damage) / 100 * 5;
            Cure = standardValue * (SpellConfig.Cure) / 100 * 5;
            Atk = standardValue * (SpellConfig.Atk) / 100;//和monster的攻击一样
            if (Addon > 0 || Addon < 0)
            {
                Damage = (int)(Damage * (1 + Addon));
                Cure = (int)(Cure * (1 + Addon));
            }

            double standardValue2 = Math.Sqrt(((double)level + 3) / 4) * (100 + SpellConfig.Modify) / 100;
            Time = SpellConfig.Time*standardValue2;
            Help = SpellConfig.Help * standardValue2;

            double standardValue3 = Math.Sqrt(((double)level + 7) / 8) * (100 + SpellConfig.Modify) / 100;
            Rate = SpellConfig.Rate * standardValue3;
            if (Rate > 100)
                Rate = 100;
        }
    }
}
