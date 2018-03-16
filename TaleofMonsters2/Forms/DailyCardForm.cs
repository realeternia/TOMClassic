using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms
{
    internal partial class DailyCardForm : BasePanel
    {
        private Image backImage;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;
        private Point[] points;
        private int[] items;

        private int cardId;

        private List<IntPair> treasureList = new List<IntPair>();

        public DailyCardForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("tsk7");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;
            DoubleBuffered = true;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            points = new Point[4];
            items = new int[4];
            var dts = UserProfile.InfoWorld.GetDailyCardData();
            
            items[0] = dts[0];
            items[1] = dts[1];
            items[2] = dts[2];
            items[3] = dts[3];
            cardId = dts[4];

            #region 初始化位置
            points[0] = new Point(65, 30);
            points[1] = new Point(115, 30);
            points[2] = new Point(165, 30);
            points[3] = new Point(215, 30);
            #endregion

            vRegion = new VirtualRegion(this);
            #region 读取轮盘配置
            treasureList.Add(new IntPair() { Type = items[0], Value = 1 });
            treasureList.Add(new IntPair() { Type = items[1], Value = 1 });
            treasureList.Add(new IntPair() { Type = items[2], Value = 1 });
            treasureList.Add(new IntPair() { Type = items[3], Value = 1 });
            #endregion

            int xOff = 13;
            int yOff = 107;
            Color black = Color.FromArgb(180, Color.Black);
            for (int i = 0; i < treasureList.Count; i++)
            {
                var targetItem = treasureList[i];
                var region = new PictureAnimRegion(i, points[i].X+ xOff, points[i].Y + yOff, 40, 40, PictureRegionCellType.Item, targetItem.Type);
                region.AddDecorator(new RegionTextDecorator(5, 24, 10));
                vRegion.AddRegion(region);
                vRegion.SetRegionDecorator(i, 0, targetItem.Value.ToString());

                if (UserProfile.InfoBag.GetItemCount(items[i]) <= 0)
                    region.AddDecorator(new RegionCoverDecorator(black));
            }

            vRegion.AddRegion(new PictureAnimRegion(10, 100 + xOff, 100 + yOff, 120, 120, PictureRegionCellType.Card, cardId));

            backImage = PicLoader.Read("System", "DailyBack.JPG");
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            foreach (var item in items)
            {
                if (UserProfile.InfoBag.GetItemCount(item) <= 0)
                {
                    AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughItems), "Red");
                    return;
                }
            }

            foreach (var item in items)
                UserProfile.InfoBag.DeleteItem(item, 1);

            UserProfile.InfoCard.AddCard(cardId);

            AddFlowCenter("合成成功", "Lime");
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (id == 10)
            {
                Image image = CardAssistant.GetCard(key).GetPreview(CardPreviewType.Normal, new uint[] { });
                tooltip.Show(image, this, x, y, key);
            }
            else if (key > 0)
            {
                Image image = HItemBook.GetPreview(key);
                tooltip.Show(image, this, x, y);
            }

        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void DailyCardForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("每日合成", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            int xOff = 13;
            int yOff = 107;
            e.Graphics.DrawImage(backImage, xOff, yOff, 324, 244);

            vRegion.Draw(e.Graphics);

            //if (WheelId > 0)
            //{
            //    var wheelConfig = ConfigDatas.ConfigData.GetTreasureWheelConfig(WheelId);
            //    e.Graphics.DrawImage(HSIcons.GetIconsByEName("res1"), xOff+ 63, yOff+ 60, 20, 20);
            //    Font font2 = new Font("宋体", 11 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            //    e.Graphics.DrawString(wheelConfig.GoldCost.ToString(), font2, Brushes.Black, xOff + 85 + 1, yOff + 62 + 1);
            //    e.Graphics.DrawString(wheelConfig.GoldCost.ToString(), font2, Brushes.White, xOff + 85, yOff + 62);
            //    font2.Dispose();
            //}
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
