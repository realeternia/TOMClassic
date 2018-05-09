using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Decks;
using TaleofMonsters.Datas.Peoples;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms.MagicBook
{
    internal sealed partial class PeopleDeckViewForm : BasePanel
    {
        private const int cardWidth = 60;
        private const int cardHeight = 60;
        private const int xCount = 8;
        private const int yCount = 6;
        private const int cardCount = xCount * yCount;
        private int totalCount;
        private int page;
        private int tar = -1;
        private List<int> people;
        private CardDetail cardDetail;

        private NLSelectPanel selectPanel;

        public PeopleDeckViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonNext.ImageNormal = PicLoader.Read("Button.Panel", "NextButton.JPG");
            bitmapButtonNext.NoUseDrawNine = true;
            this.bitmapButtonPre.ImageNormal = PicLoader.Read("Button.Panel", "PreButton.JPG");
            bitmapButtonPre.NoUseDrawNine = true;
            cardDetail = new CardDetail(this, cardWidth * xCount + 65, 35, cardHeight * 7 + 105);
            nlClickLabel1.Location = new Point(70, cardHeight * yCount + 65);
            nlClickLabel1.Size = new Size(cardWidth * xCount - 10, 70+(7-yCount)*cardHeight);

            selectPanel = new NLSelectPanel(65, 35, xCount * cardWidth, yCount * cardHeight, this);
            selectPanel.ItemsPerRow = xCount;
            selectPanel.ItemHeight = cardHeight;
            selectPanel.DrawCell += SelectPanel_DrawCell;
            selectPanel.SelectIndexChanged += SelectPanel_SelectIndexChanged;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            page = 0;
            people = new List<int>();
            foreach (PeopleConfig peopleConfig in ConfigData.PeopleDict.Values)
            {
                if (PeopleBook.IsPeople(peopleConfig.Id) && peopleConfig.Emethod != "")
                    people.Add(peopleConfig.Id);
            }
            totalCount = people.Count;
            UpdateButtonState();
            InitItems();
        }

        public override void OnFrame(int tick, float timePass)
        {
            cardDetail.OnFrame();
        }

        private void UpdateButtonState()
        {
            bitmapButtonPre.Enabled = page > 0;
            bitmapButtonNext.Enabled = (page + 1) * cardCount < totalCount;
        }

        private void buttonPre_Click(object sender, EventArgs e)
        {
            page--;
            if (page < 0)
            {
                page++;
                return;
            }
            tar = -1;
            cardDetail.SetInfo(-1);
            UpdateButtonState();
            InitItems();
            Invalidate(new Rectangle(65, 35, cardWidth * xCount+200, 630));
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            page++;
            if (page > (totalCount - 1) / cardCount)
            {
                page--;
                return;
            }
            tar = -1;
            cardDetail.SetInfo(-1);
            UpdateButtonState();
            InitItems();
            Invalidate(new Rectangle(65, 35, cardWidth * xCount+200, 630));
        }

        private void InitItems()
        {
            int pages = totalCount / cardCount + 1;
            int cardLimit = (page < pages - 1) ? cardCount : (totalCount % cardCount);
            int former = cardCount * page + 1;
            var datas = new List<int>();
            for (int i = former - 1; i < former + cardLimit - 1; i++)
                datas.Add(people[i]);
            selectPanel.AddContent(datas);
        }

        private void nlClickLabel1_SelectionChange(Object value)
        {
            cardDetail.SetInfo(value as DeckCard);
            Invalidate(new Rectangle(65, 35, cardWidth * xCount + 200, 630));
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SelectPanel_SelectIndexChanged()
        {
            tar = selectPanel.SelectIndex + page * cardCount;

            if (tar != -1)
            {
                PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(people[tar]);
                cardDetail.SetInfo(-1);

                DeckCard[] cards = DeckBook.GetDeckByName(peopleConfig.Emethod, peopleConfig.Level);
                nlClickLabel1.ClearLabel();
                foreach (DeckCard card in cards)
                {
                    CardConfigData pickCard = CardConfigManager.GetCardConfig(card.CardId);
                    if (pickCard.Id == 0)
                        continue;

                    var cardConfig = CardConfigManager.GetCardConfig(card.CardId);
                    var colorName = HSTypes.I2QualityColor((int)cardConfig.Quality);
                    if (colorName == "White")
                        colorName = "DarkGray";
                    nlClickLabel1.AddLabel(cardConfig.Name, card, Color.FromName(colorName));
                }

                nlClickLabel1.Invalidate();
            }
        }

        private void SelectPanel_DrawCell(Graphics g, int info, int xOff, int yOff, bool inMouseOn, bool isTarget, bool onlyBorder)
        {
            if (!onlyBorder)
            {
                var peopleConfig = ConfigData.GetPeopleConfig(info);
                var brush = new SolidBrush(Color.FromName(HSTypes.I2QualityColorD(peopleConfig.Quality)));
                g.FillRectangle(brush, xOff + 3, yOff + 3, cardWidth - 6, cardHeight - 6);
                brush.Dispose();

                g.DrawImage(PeopleBook.GetPersonImage(info), xOff, yOff, cardWidth, cardHeight);
            }

            if (inMouseOn || isTarget)
            {
                Color borderColor = isTarget ? Color.Lime : Color.Yellow;
                SolidBrush yellowbrush = new SolidBrush(Color.FromArgb(80, borderColor));
                g.FillRectangle(yellowbrush, xOff, yOff, cardWidth, cardHeight);
                yellowbrush.Dispose();

                Pen yellowpen = new Pen(borderColor, 2);
                g.DrawRectangle(yellowpen, xOff+1, yOff+1, cardWidth - 2, cardHeight - 2);
                yellowpen.Dispose();
            }
        }

        private void PeopleDeckViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 对战卡组 ", font, Brushes.White, 320, 8);
            font.Dispose();

            Font fontblack = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(245, 244, 242)), 65, cardHeight * yCount + 40, cardWidth * xCount, 100 + (7 - yCount) * cardHeight);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(190, 175, 160)), 65, cardHeight * yCount+40, cardWidth * xCount, 20);
            e.Graphics.DrawString("所有卡组", fontblack, Brushes.White, 65, cardHeight * yCount + 42);
            fontblack.Dispose();

            cardDetail.Draw(e.Graphics);
        }
    }
}