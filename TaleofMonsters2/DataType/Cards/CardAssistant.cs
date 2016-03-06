using System.Drawing;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Cards.Spells;
using TaleofMonsters.DataType.Cards.Weapons;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.Cards
{
    internal static class CardAssistant
    {
        public static void DrawBase(Graphics g, int cid, int x, int y, int width, int height)
        {
            int xDiff = width / 12;
            int yDiff = height / 12;
            Image img = GetCardImage(cid, width, height);
            var cardData = CardConfigManager.GetCardConfig(cid);
            int attr = cardData.Attr;//属性决定包边颜色
            g.FillRectangle(PaintTool.GetBrushByAttribute(attr), x + 1, y + 1, width - 2, height - 2);
            g.DrawImage(img, x + 2 + xDiff, y + 2 + xDiff, width - 4 - xDiff*2, height - 4-yDiff*2);

            string cardBorder = GetCardBorder(cardData);
            g.DrawImage(PicLoader.Read("Border", cardBorder), x + 2, y + 2, width - 4, height - 4);
           
        }

        public static string GetCardBorder(CardConfigData card)
        {
            string cardBorder = "";
            if (card.Type == CardTypes.Monster)
            {
                cardBorder = "border4.PNG";
            }
            else if (card.Type == CardTypes.Weapon)
            {
                cardBorder = "border6.PNG";
            }
            else
            {
                cardBorder = "border5.PNG";
            }

            if (card.Quality == CardQualityTypes.Legend)//传说卡
            {
                cardBorder = "border2.PNG";
            }
            return cardBorder;
        }

        public static Image GetCardImage(int cid, int width, int height)
        {
            switch (GetCardType(cid))
            {
                case CardTypes.Monster: return MonsterBook.GetMonsterImage(cid, width, height);
                case CardTypes.Weapon: return WeaponBook.GetWeaponImage(cid, width, height);
                case CardTypes.Spell: return SpellBook.GetSpellImage(cid, width, height);
            }
            return SpecialCards.NullCard.GetCardImage(width, height);
        }

        public static Card GetCard(int cid)
        {
            switch (GetCardType(cid))
            {
                case CardTypes.Monster: return new MonsterCard(new Monster(cid));
                case CardTypes.Weapon: return new WeaponCard(new Weapon(cid));
                case CardTypes.Spell: return new SpellCard(new Spell(cid));
            }
            return SpecialCards.NullCard;
        }

        internal static CardTypes GetCardType(int cid)
        {
            switch (cid / 1000000)
            {
                case 51: return CardTypes.Monster;
                case 52: return CardTypes.Weapon;
                case 53: return CardTypes.Spell;
            }
            return CardTypes.Null;
        }

        private static float[] rangePunish = new float[] { 1.3f, 1, 0.75f, 0.68f, 0.62f, 0.56f, 0.52f, 0.48f, 0.44f, 0.42f, 0.4f };
        public static float GetCardFactorOnRange(int range)
        {
            return rangePunish[range / 10];
        }

        private static float[] movPunish = new float[] { 1.2f, 1, 1, 0.92f, 0.86f, 0.82f, 0.79f, 0.77f, 0.75f, 0.74f, 0.73f };
        public static float GetCardFactorOnMove(int mov)
        {
            return movPunish[mov / 5];
        }
    }
}
