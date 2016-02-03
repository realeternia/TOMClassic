using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards;

namespace TaleofMonsters.Controler.Battle.Components
{
    internal sealed class CardSlot
    {
        public bool MouseOn { get; set; }

        public ActiveCard ACard { get; private set; }
        private Card Card { get; set; }

        public Point Location;
        public Size Size;
        public bool Enabled = true;
        public Color BgColor = Color.DimGray;

        public bool IsSelected { get; set; }

        public CardSlot()
        {

        }

        public void SetSlotCard(ActiveCard tcard)
        {
            ACard = tcard;
            Card = CardAssistant.GetCard(tcard.CardId);
            Card.SetData(ACard);
        }

        public void CardSlot_Paint( PaintEventArgs e)
        {
            if (Card != null)
            {
                CardMouseState state = !Enabled ? CardMouseState.Disable : (MouseOn ? CardMouseState.MouseOn : CardMouseState.Normal);
                Draw(e.Graphics, state);


#if DEBUG
          //      Font font = new Font("Arial", 7*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
           //     e.Graphics.DrawString(acard.Id.ToString(), font, Brushes.White, 0, 20);
          //      font.Dispose();
#endif
            }
        }

        private void Draw(Graphics g, CardMouseState mouse)
        {
            if (Card is SpecialCard)
            {
             //   Image img2 = Card.GetCardImage(120, 120);
             //   g.DrawImage(img2, new Rectangle(Location.X, 10, 120, 120), 0, 0, img2.Width, img2.Height, GraphicsUnit.Pixel);
                return;
            }

            int x = Location.X;
            int y = 0;
            Color bgColor = IsSelected ? Color.LightGreen : BgColor;
            SolidBrush sb = new SolidBrush(bgColor);
            g.FillRectangle(sb, new Rectangle(x, 0, Size.Width, Size.Height));
            sb.Dispose();
            if (mouse != CardMouseState.MouseOn)
                y = 10;
            CardAssistant.DrawBase(g, Card.CardId, x, y, Size.Width, 120);

            if (mouse == CardMouseState.Disable)
            {
                var sbrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                g.FillRectangle(sbrush, x, y, Size.Width, 120);
                sbrush.Dispose();
            }

            Font font = new Font("Arial", 7*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            for (int i = 0; i < Card.Star; i++)
            {
                g.DrawString("★", font, Brushes.Black, x + Size.Width - 16, y+2+i*10);
                g.DrawString("★", font, Brushes.Yellow, x + Size.Width - 15, y + 1 + i * 10);
            }
            font.Dispose();

            font = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            var cardName = string.Format("{0}Lv{1}", Card.Name, ACard.Level);
            var cardQual = Config.CardConfigManager.GetCardConfig(Card.CardId).Quality;
            var cardColor = Color.FromName(HSTypes.I2QualityColor(cardQual));
            var brush = new SolidBrush(cardColor);
            g.DrawString(cardName, font, Brushes.Black, x + 1, mouse != CardMouseState.MouseOn ? 117 : 122);
            g.DrawString(cardName, font,brush, x, mouse != CardMouseState.MouseOn ? 116 : 121);
            font.Dispose();
            brush.Dispose();

            int index = 0;
            foreach (var manaType in ACard.CostList)
            {
                var imgIcon = HSIcons.GetIconsByEName("mix" + (int)manaType);
                g.DrawImage(imgIcon, x + index * 10, y+1,16,16);
                index++;
            }
        }

    }
}
