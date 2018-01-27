using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.Items.Core;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Control;
using TaleofMonsters.DataType.CardPieces;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms.MagicBook
{
    internal sealed partial class CardDropViewForm : BasePanel
    {
        private const int cardWidth = 67;
        private const int cardHeight = 90;
        private int xCount = 8;
        private int yCount = 5;
        private int cardCount;
        private int page;
        private int tar = -1;
        private List<int> cards;
        private CardDetail cardDetail;
        private NLSelectPanel selectPanel;

        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion vRegion;

        public CardDropViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonNext.ImageNormal = PicLoader.Read("Button.Panel", "NextButton.JPG");
            bitmapButtonNext.NoUseDrawNine = true;
            this.bitmapButtonPre.ImageNormal = PicLoader.Read("Button.Panel", "PreButton.JPG");
            bitmapButtonPre.NoUseDrawNine = true;

            vRegion = new VirtualRegion(this);

            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        private void InitVirtualRegion(int id, int x, int y)
        {
            var picRegion = new PictureRegion(id, x, y, 50, 50, PictureRegionCellType.Item, 22031201);
            picRegion.AddDecorator(new RegionTextDecorator(2, 35, 9, Color.White));
            vRegion.AddRegion(picRegion);
        }

        public override void Init(int width, int height)
        {
            #region 智能修正面板元素数量
            //823,575 x=8,y=5
            int borderX = 823 - cardWidth * xCount;
            int borderY = 575 - cardHeight * yCount;

            xCount = (width - borderX) * 8 / 10 / cardWidth; //0.8作为一个边缘因子
            yCount= (height - borderY) * 9 / 10 / cardHeight;
            cardCount = xCount*yCount;

            Width = xCount * cardWidth + borderX;
            Height = yCount * cardHeight + borderY;

            selectPanel = new NLSelectPanel(65, 35, xCount * cardWidth, yCount * cardHeight, this);
            selectPanel.ItemsPerRow = xCount;
            selectPanel.ItemHeight = cardHeight;
            selectPanel.DrawCell += SelectPanel_DrawCell;
            selectPanel.SelectIndexChanged += SelectPanel_SelectIndexChanged;

            selectPanel.UseCache = true;
            #endregion

            base.Init(width, height);
            cards = new List<int>();
            cardDetail = new CardDetail(this, cardWidth * xCount + 65, 35, cardHeight * yCount + 70);
            cardDetail.Invalidate = DetailInvalidate;
            int baseY = cardHeight * yCount + 45;
            InitVirtualRegion(1, 65 + 10, baseY + 3);
            InitVirtualRegion(2, 65 + 70, baseY + 3);
            InitVirtualRegion(3, 65 + 130, baseY + 3);
            InitVirtualRegion(4, 65 + 190, baseY + 3);
            InitVirtualRegion(5, 65 + 250, baseY + 3);
            InitVirtualRegion(6, 65 + 310, baseY + 3);
            InitVirtualRegion(7, 65 + 370, baseY + 3);
            InitVirtualRegion(8, 65 + 430, baseY + 3);
            InitVirtualRegion(9, 65 + 490, baseY + 3);
            InitVirtualRegion(10, 65 + 550, baseY + 3);

            ChangeCards();
        }

        private void ChangeCards()
        {
            page = 0;
            cards.Clear();
            tar = -1;
            cardDetail.SetInfo(-1);
            SelectCard(-1);
            foreach (var monsterConfig in ConfigData.MonsterDict.Values)
            {
                if (monsterConfig.IsSpecial > 0)
                    continue;
                cards.Add(monsterConfig.Id);
            }

            UpdateButtonState();
            InitItems();
            if (cards.Count > 0)
                cardDetail.SetInfo(cards[0]);
            Invalidate(new Rectangle(65, 35, cardWidth * xCount, cardHeight * yCount));
        }

        public override void OnFrame(int tick, float timePass)
        {
            cardDetail.OnFrame();
        }

        private void UpdateButtonState()
        {
            bitmapButtonPre.Enabled = page > 0;
            bitmapButtonNext.Enabled = (page + 1) * cardCount < cards.Count;
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
            SelectCard(-1);
            UpdateButtonState();
            InitItems();
            Invalidate(new Rectangle(65, 35, cardWidth*xCount, cardHeight*yCount));
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            page++;
            if (page > (cards.Count - 1) / cardCount)
            {
                page--;
                return;
            }
            tar = -1;
            cardDetail.SetInfo(-1);
            SelectCard(-1);
            UpdateButtonState();
            InitItems();
            Invalidate(new Rectangle(65, 35, cardWidth * xCount, cardHeight * yCount));
        }

        private void InitItems()
        {
            int pages = cards.Count / cardCount + 1;
            int cardLimit = (page < pages - 1) ? cardCount : (cards.Count % cardCount);
            int former = cardCount * page + 1;
            var datas = new List<int>();
            for (int i = former - 1; i < former + cardLimit - 1; i++)
                datas.Add(cards[i]);
            selectPanel.AddContent(datas);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SelectCard(int cardId)
        {
            for (int i = 0; i < 10; i++)
            {
                vRegion.SetRegionKey(i+1, 0);
                vRegion.SetRegionDecorator(i + 1, 0, "");//回复默认状态
            }

            var dropList = CardPieceBook.GetDropListByCardId(cardId);
            for (int i = 0; i < Math.Min(10, dropList.Count); i++)
            {
                vRegion.SetRegionKey(i + 1, dropList[i].ItemId);
                vRegion.SetRegionDecorator(i+1,0,string.Format("{0:0.0}%", (float)(dropList[i].Rate)/100));
            }
            cardDetail.Invalidate();
            Invalidate(new Rectangle(67, 37 + yCount * cardHeight, cardWidth * xCount - 4,71));
        }

        private void DetailInvalidate()
        {
            if (cardDetail != null)
                Invalidate(new Rectangle(cardDetail.X, cardDetail.Y, cardDetail.Width, cardDetail.Height));
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (key > 10)
            {
                Image image = HItemBook.GetPreview(key);
                tooltip.Show(image, this, x, y);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void SelectPanel_SelectIndexChanged()
        {
            tar = selectPanel.SelectIndex + page * cardCount;

            cardDetail.SetInfo(cards[tar]);
            SelectCard(cards[tar]);
        }

        private void SelectPanel_DrawCell(Graphics g, int info, int xOff, int yOff, bool inMouseOn, bool isTarget, bool onlyBorder)
        {
            if (!onlyBorder)
                CardAssistant.DrawBase(g, info, xOff, yOff, cardWidth, cardHeight);

            if (inMouseOn)
            {
                var brushes = new SolidBrush(Color.FromArgb(130, Color.Yellow));
                g.FillRectangle(brushes, xOff, yOff, cardWidth, cardHeight);
                brushes.Dispose();
            }

            if (!onlyBorder)
            {
                var cardConfigData = CardConfigManager.GetCardConfig(info);
                Font font = new Font("宋体", 5 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                g.DrawString(("★★★★★★★★★★").Substring(10 - cardConfigData.Star), font, Brushes.Yellow, xOff + 3, yOff + 3);
                font.Dispose();

                var quality = cardConfigData.Quality + 1;
                g.DrawImage(HSIcons.GetIconsByEName("gem" + (int)quality), xOff + cardWidth / 2 - 8, yOff + cardHeight - 20, 16, 16);

                var jobId = cardConfigData.JobId;
                if (jobId > 0)
                {
                    var jobConfig = ConfigData.GetJobConfig(jobId);
                    Brush brush = new SolidBrush(Color.FromName(jobConfig.Color));
                    g.FillRectangle(brush, xOff + cardWidth - 24, yOff + 4, 20, 20);
                    g.DrawImage(HSIcons.GetIconsByEName("job" + jobConfig.JobIndex), xOff + cardWidth - 24, yOff + 4, 20, 20);
                    brush.Dispose();
                }
            }
         
        }

        private void CardDropViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("卡牌掉落", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            cardDetail.Draw(e.Graphics);

            e.Graphics.FillRectangle(Brushes.DarkGray, 67, 37 + yCount * cardHeight, cardWidth * xCount - 4, 71);
            Image img = PicLoader.Read("System", "SearchBack.JPG");
            e.Graphics.DrawImage(img, 70, 40 + yCount * cardHeight, cardWidth * xCount - 10, 65);
            img.Dispose();

            vRegion.Draw(e.Graphics);
        }
    }
}