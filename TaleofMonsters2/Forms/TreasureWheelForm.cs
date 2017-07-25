using System;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;

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
            for (int i = 0; i < points.Length; i++)
            {
                var itemId = HItemBook.GetItemId(ConfigDatas.ConfigData.GetTreasureWheelConfig(i + 1).Item);
                virtualRegion.AddRegion(new PictureAnimRegion(i, points[i].X, points[i].Y, 40, 40, PictureRegionCellType.Item, itemId)); 
            }
           
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public override void OnFrame(int tick, float timePass)
        {
            if (fuel < fuelAim && (fuel<40 || (tick%(fuel/8+1)==0)))
            {
                fuel++;
                panelBack.Invalidate();

                if (fuel == fuelAim)
                {
                    var itemId = HItemBook.GetItemId(ConfigDatas.ConfigData.GetTreasureWheelConfig((fuel % points.Length) + 1).Item);
                    UserProfile.InfoBag.AddItem(itemId, 1);
                    fuelAim = 0;
                }
            }
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            if (fuelAim>0)
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
