using System.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Cards.Spells;
using TaleofMonsters.Datas.Cards.Weapons;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Datas.Cards
{
    internal static class CardAssistant
    {
        public static void DrawBase(Graphics g, int cid, int x, int y, int width, int height)
        {
            int xDiff = width / 15;
            int yDiff = height / 15;
            Image img = GetCardImage(cid, width, height);
            var cardData = CardConfigManager.GetCardConfig(cid);
            int attr = cardData.Attr;//属性决定包边颜色
            g.FillRectangle(PaintTool.GetBrushByAttribute(attr), x + 1, y + 1, width - 2, height - 2);
            g.DrawImage(img, x + 2 + xDiff, y + 2 + yDiff, width - 4 - xDiff*2, height - 4-yDiff*2);

            string cardBorder = GetCardBorder(cardData);
            var imgBack = PicLoader.Read("Border", cardBorder);
            g.DrawImage(imgBack, x + 2, y + 2, width - 4, height - 4);
            imgBack.Dispose();
        }

        public static string GetCardBorder(CardConfigData card)
        {
            string cardBorder = "";
            if (card.Type == CardTypes.Monster)
                cardBorder = "border4.PNG";
            else if (card.Type == CardTypes.Weapon)
                cardBorder = "border5.PNG";
            else
                cardBorder = "border6.PNG";

            if (card.Quality == QualityTypes.Legend)//传说卡
                cardBorder = "border2.PNG";
            return cardBorder;
        }

        public static Image GetCardImage(int cid, int width, int height)
        {
            switch (ConfigIdManager.GetCardType(cid))
            {
                case CardTypes.Monster: return MonsterBook.GetMonsterImage(cid, width, height);
                case CardTypes.Weapon: return WeaponBook.GetWeaponImage(cid, width, height);
                case CardTypes.Spell: return SpellBook.GetSpellImage(cid, width, height);
            }
            return SpecialCards.NullCard.GetCardImage(width, height);
        }

        public static Card GetCard(int cid)
        {
            switch (ConfigIdManager.GetCardType(cid))
            {
                case CardTypes.Monster: return new MonsterCard(new Monster(cid));
                case CardTypes.Weapon: return new WeaponCard(new Weapon(cid));
                case CardTypes.Spell: return new SpellCard(new Spell(cid));
            }
            return SpecialCards.NullCard;
        }

        private static float[] rangePunish = new float[] { 1.3f, 1, 0.75f, 0.68f, 0.62f, 0.56f, 0.52f, 0.48f, 0.44f, 0.42f, 0.4f, 0.38f, 0.36f };
        public static float GetCardFactorOnRange(int range)
        {
            return rangePunish[range / 10];
        }

        private static float[] movPunish = new float[] { 1.2f, 1, 1, 0.92f, 0.86f, 0.82f, 0.79f, 0.77f, 0.75f, 0.74f, 0.73f, 0.72f, 0.71f };
        public static float GetCardFactorOnMove(int mov)
        {
            return movPunish[mov / 5];
        }

        public static int GetCardModify(int star, int level, QualityTypes quality, int modify)
        {
            float standardValue = (int)(30 * (1 + (star - 1) * GameConstants.CardStrengthStar) * (1 + (level - 1) * GameConstants.CardStrengthLevel));
            var baseRate = 1 + (float)modify/200;
            if (quality == QualityTypes.Legend)
                standardValue*=(baseRate + 0.4f);
            else if (quality == QualityTypes.Epic)
                standardValue *= (baseRate + 0.25f);
            else if (quality == QualityTypes.Excel)
                standardValue *= (baseRate + 0.15f);
            else if (quality == QualityTypes.Good)
                standardValue *= (baseRate + 0.05f);
            else
                standardValue *= baseRate;
            return (int) standardValue;
        }
    }
}
