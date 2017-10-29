using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Scenes;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms
{
    internal partial class DungeonForm : BasePanel
    {
        private Image backImage;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion vRegion;
        private List<int> gismoList;

        public int DungeonId { get; set; }

        public DungeonForm()
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

            gismoList = DungeonBook.GetGismoListByDungeon(DungeonId);

            vRegion = new VirtualRegion(this);

            int xOff = 13;
            int yOff = 107;
            Color black = Color.FromArgb(180, Color.Black);
            for (int i = 0; i < gismoList.Count; i++)
            {
                var targetItem = gismoList[i];
                var region = new PictureAnimRegion(i, points[i].X+ xOff, points[i].Y + yOff, 40, 40, PictureRegionCellType.Item, targetItem.Type);
                region.AddDecorator(new RegionTextDecorator(5, 24, 10));
                vRegion.AddRegion(region);
                vRegion.SetRegionDecorator(i, 0, targetItem.Value.ToString());

                if (UserProfile.InfoBag.GetItemCount(gismoList[i]) <= 0)
                    region.AddDecorator(new RegionCoverDecorator(black));
            }

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
            foreach (var item in gismoList)
            {
                if (UserProfile.InfoBag.GetItemCount(item) <= 0)
                {
                    AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughItems), "Red");
                    return;
                }
            }

            foreach (var item in gismoList)
                UserProfile.InfoBag.DeleteItem(item, 1);

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
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
