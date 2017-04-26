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
        private bool show;
        private int tar = -1;
        private List<int> cards;
        private Bitmap tempImage;
        private bool isDirty = true;
        private CardDetail cardDetail;

        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;

        public CardDropViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonNext.ImageNormal = PicLoader.Read("ButtonBitmap", "NextButton.JPG");
            bitmapButtonNext.NoUseDrawNine = true;
            this.bitmapButtonPre.ImageNormal = PicLoader.Read("ButtonBitmap", "PreButton.JPG");
            bitmapButtonPre.NoUseDrawNine = true;
            tempImage = new Bitmap(cardWidth * xCount, cardHeight*yCount);

            virtualRegion = new VirtualRegion(this);

            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        private void InitVirtualRegion(int id, int x, int y)
        {
            var picRegion = new PictureRegion(id, x, y, 50, 50, PictureRegionCellType.Item, 22031201);
            picRegion.AddDecorator(new RegionTextDecorator(2, 35, 9, Color.White));
            virtualRegion.AddRegion(picRegion);
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
            #endregion

            base.Init(width, height);
            show = true;
            cards = new List<int>();
            cardDetail = new CardDetail(this, cardWidth * xCount + 65, 35, cardHeight * yCount + 70);

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

            if (cards.Count > 0)
                cardDetail.SetInfo(cards[0]);
            isDirty = true;
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
            isDirty = true;
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
            isDirty = true;
            Invalidate(new Rectangle(65, 35, cardWidth * xCount, cardHeight * yCount));
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CardViewForm_MouseMove(object sender, MouseEventArgs e)
        {
            int truex = e.X - 65;
            int truey = e.Y - 35;
            if (truex > 0 && truex < xCount * cardWidth && truey > 0 && truey < yCount * cardHeight)
            {
                int temp = truex / cardWidth + truey / cardHeight * xCount + cardCount * page;
                if (temp != tar)
                {
                    if (temp < cards.Count)
                    {
                        tar = temp;
                    }
                    else
                    {
                        tar = -1;
                    }
                    Invalidate(new Rectangle(65, 35, cardWidth * xCount, cardHeight * yCount));
                }
            }
            else
            {
                if (tar != -1)
                {
                    tar = -1;
                    Invalidate(new Rectangle(65, 35, cardWidth * xCount, cardHeight * yCount));
                }
            }
        }

        private void CardViewForm_Click(object sender, EventArgs e)
        {
            if (tar != -1)
            {
                cardDetail.SetInfo(cards[tar]);
                SelectCard(cards[tar]);
                Invalidate(new Rectangle(65 + cardWidth * xCount, 35, 200, 525));
            }
        }

        private void SelectCard(int cardId)
        {
            for (int i = 0; i < 10; i++)
            {
                virtualRegion.SetRegionKey(i+1, 0);
                virtualRegion.SetRegionDecorator(i + 1, 0, "");//回复默认状态
            }

            var dropList = CardPieceBook.GetDropListByCardId(cardId);
            for (int i = 0; i < Math.Min(10, dropList.Count); i++)
            {
                virtualRegion.SetRegionKey(i + 1, dropList[i].ItemId);
                virtualRegion.SetRegionDecorator(i+1,0,string.Format("{0:0.0}%", (float)(dropList[i].Rate)/100));
            }

            Invalidate(new Rectangle(67, 37 + yCount * cardHeight, cardWidth * xCount - 4,71));
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

        private void DrawOnCardView(Graphics g, int cid, int x, int y, bool isSelected)
        {
            CardAssistant.DrawBase(g, cid, x, y, cardWidth, cardHeight);
            if (isSelected)
            {
                var brushes = new SolidBrush(Color.FromArgb(130, Color.Yellow));
                g.FillRectangle(brushes, x, y, cardWidth, cardHeight);
                brushes.Dispose();
            }

            var cardConfigData = CardConfigManager.GetCardConfig(cid);
            Font font = new Font("宋体", 5*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(("★★★★★★★★★★").Substring(10 - cardConfigData.Star), font, Brushes.Yellow, x+3, y+3);
            font.Dispose();

            var quality = cardConfigData.Quality + 1;
            g.DrawImage(HSIcons.GetIconsByEName("gem" + quality), x + cardWidth / 2 - 8, y + cardHeight - 20, 16, 16);

            var jobId = cardConfigData.JobId;
            if (jobId > 0)
            {
                var jobConfig = ConfigData.GetJobConfig(jobId);
                Brush brush = new SolidBrush(Color.FromName(jobConfig.Color));
                g.FillRectangle(brush, x + cardWidth - 24, y + 4, 20, 20);
                g.DrawImage(HSIcons.GetIconsByEName("job" + jobConfig.JobIndex), x + cardWidth - 24, y + 4, 20, 20);
                brush.Dispose();
            }
        }

        private void CardViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("卡牌掉落", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            cardDetail.Draw(e.Graphics);

            if (show)
            {
                int pages = cards.Count / cardCount + 1;
                int cardLimit = (page < pages - 1) ? cardCount : (cards.Count % cardCount);
                int former = cardCount * page + 1;
                if (isDirty)
                {
                    tempImage.Dispose();
                    tempImage = new Bitmap(cardWidth * xCount, cardHeight * yCount);
                    Graphics g = Graphics.FromImage(tempImage);
                    for (int i = former - 1; i < former + cardLimit - 1; i++)
                    {
                        int ri = i % (xCount * yCount);
                        int x = (ri % xCount) * cardWidth;
                        int y = (ri / xCount) * cardHeight;
                        DrawOnCardView(g, cards[i], x, y, false);
                    }
                    g.Dispose();
                    isDirty = false;
                }
                e.Graphics.DrawImage(tempImage, 65, 35);
                if (tar != -1 && tar < cards.Count)
                {
                    int ri = tar % (xCount * yCount);
                    int x = (ri % xCount) * cardWidth + 65;
                    int y = (ri/xCount)*cardHeight + 35;
                    DrawOnCardView(e.Graphics, cards[tar], x, y, true);
                }

                e.Graphics.FillRectangle(Brushes.DarkGray, 67, 37 + yCount * cardHeight, cardWidth * xCount - 4, 71);
                Image img = PicLoader.Read("System", "SearchBack.JPG");
                e.Graphics.DrawImage(img, 70, 40 + yCount * cardHeight, cardWidth * xCount - 10, 65);
                img.Dispose();

                virtualRegion.Draw(e.Graphics);
            }
        }
    }
}