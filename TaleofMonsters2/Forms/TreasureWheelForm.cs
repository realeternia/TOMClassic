using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms
{
    internal partial class TreasureWheelForm : BasePanel
    {
        private Image backImage;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;
        private Point[] points;
        private int fuel;
        private int fuelAim;

        private List<IntPair> treasureList = new List<IntPair>();

        public TreasureWheelForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("wep4");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;
            backImage = PicLoader.Read("System", "WheelBack.JPG");

            points= new Point[18];
#region 初始化位置
            points[0] = new Point(15, 10);
            points[1] = new Point(65, 10);
            points[2] = new Point(115, 10);
            points[3] = new Point(165, 10);
            points[4] = new Point(215, 10);
            points[5] = new Point(265, 10);
            points[6] = new Point(265, 55);
            points[7] = new Point(265, 100);
            points[8] = new Point(265, 145);
            points[9] = new Point(265, 190);
            points[10] = new Point(215, 190);
            points[11] = new Point(165, 190);
            points[12] = new Point(115, 190);
            points[13] = new Point(65, 190);
            points[14] = new Point(15, 190);
            points[15] = new Point(15, 145);
            points[16] = new Point(15, 100);
            points[17] = new Point(15, 55);
#endregion

            virtualRegion = new VirtualRegion(panelBack);
            var wheelConfig = ConfigDatas.ConfigData.GetTreasureWheelConfig(1); //todo 1暂时写死
#region 读取轮盘配置
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item1), Value = wheelConfig.Count1 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item2), Value = wheelConfig.Count2 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item3), Value = wheelConfig.Count3 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item4), Value = wheelConfig.Count4 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item5), Value = wheelConfig.Count5 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item6), Value = wheelConfig.Count6 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item7), Value = wheelConfig.Count7 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item8), Value = wheelConfig.Count8 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item9), Value = wheelConfig.Count9 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item10), Value = wheelConfig.Count10 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item11), Value = wheelConfig.Count11 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item12), Value = wheelConfig.Count12 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item13), Value = wheelConfig.Count13 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item14), Value = wheelConfig.Count14 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item15), Value = wheelConfig.Count15 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item16), Value = wheelConfig.Count16 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item17), Value = wheelConfig.Count17 });
            treasureList.Add(new IntPair() { Type = HItemBook.GetItemId(wheelConfig.Item18), Value = wheelConfig.Count18 });
#endregion

            for (int i = 0; i < treasureList.Count; i++)
            {
                var targetItem = treasureList[i];
                var region = new PictureAnimRegion(i, points[i].X, points[i].Y, 40, 40, PictureRegionCellType.Item, targetItem.Type);
                region.AddDecorator(new RegionTextDecorator(5, 24, 10));
                virtualRegion.AddRegion(region);
                virtualRegion.SetRegionDecorator(i, 0, targetItem.Value.ToString());
            }
           
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public override void OnFrame(int tick, float timePass)
        {
            if (fuel < fuelAim && (fuel < 40 || (tick%(fuel/8 + 1) == 0)))
            {
                fuel++;
                panelBack.Invalidate();

                if (fuel == fuelAim)
                {
                    var targetItem = treasureList[fuel % points.Length];
                    UserProfile.InfoBag.AddItem(targetItem.Type, targetItem.Value);
                    fuelAim = 0;
                }
            }
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            if (fuelAim > 0)
            {
                return;
            }

            if (MessageBoxEx2.Show("是否花10钻石开始转转盘?") == DialogResult.OK)
            {
                if (UserProfile.InfoBag.PayDiamond(10))
                {
                    fuel = 0;
                    fuelAim = NarlonLib.Math.MathTool.GetRandom(48, 66);
                }
            }
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (key > 0)
            {
                Image image = HItemBook.GetPreview(key);
                tooltip.Show(image, this, x+panelBack.Location.X, y+panelBack.Location.Y);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void panelIcons_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(backImage, 0, 0, panelBack.Width, panelBack.Height);

            virtualRegion.Draw(e.Graphics);

            int tar = fuel%points.Length;
            Pen pen = new Pen(Brushes.Yellow, 3);
            e.Graphics.DrawRectangle(pen, points[tar].X, points[tar].Y, 40, 40);
            pen.Dispose();
        }

        private void ExpBottleForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("幸运转盘", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
