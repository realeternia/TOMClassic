using System;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms.VBuilds
{
    internal sealed partial class OreForm : BasePanel
    {
        private bool showImage;

        public OreForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButtonHelp.ImageNormal = PicLoader.Read("Button.Panel", "LearnButton.JPG");

            this.bitmapButtonC1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("tsk2");
            this.bitmapButtonC2.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC2.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("tsk2");
            this.bitmapButtonC3.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC3.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("tsk2");
            this.bitmapButtonC4.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC4.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("tsk2");
            this.bitmapButtonC5.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC5.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("tsk2");
            this.bitmapButtonC6.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC6.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("tsk2");
            this.bitmapButtonC7.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC7.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("tsk2");
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            showImage = true;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OreForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("矿场", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if (!showImage)
                return;

            Image back = PicLoader.Read("Build", "ore.JPG");
            e.Graphics.DrawImage(back, 15, 40,572,352);
            back.Dispose();

            font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Brush b = new SolidBrush(Color.FromArgb(200, Color.Black));
            for (int i = 0; i < 7; i++)
            {
                int baseX = 15 + 80 * i + 10;
                int baseY = 40 + 40;
          
                e.Graphics.FillRectangle(b, baseX, baseY, 70, 250);

                Image tile = PicLoader.Read("Build", string.Format("Ore.ore{0}.JPG", i));
                e.Graphics.DrawImage(tile, baseX, baseY + 30, tile.Width, tile.Height);
                tile.Dispose();
                
                e.Graphics.DrawRectangle(Pens.Black, baseX, baseY, 70, 250);

                string timeText = string.Format("采集{0}", HSTypes.I2Resource(i));
                e.Graphics.DrawString(timeText, font, Brushes.White, baseX + 5, baseY + 10);
            }

            e.Graphics.FillRectangle(b, 30, 350, 160, 20);
            var digTime = UserProfile.InfoCastle.OreDigEp/5;
            var digEp = UserProfile.InfoCastle.OreDigEp % 5;
            e.Graphics.DrawString(string.Format("挖掘次数{0} (能量累计{1}%)", digTime, digEp*20), font, Brushes.White, 30, 353);
            font.Dispose();
            b.Dispose();
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            int resId = int.Parse((sender as Control).Tag.ToString());
            var digTime = UserProfile.InfoCastle.OreDigEp / 5;
            if (digTime <= 0)
            {
                AddFlowCenter("挖掘次数不足", "Red");
                return;
            }

            var addon = GameResourceBook.InResBuildOre(resId, 3);
            UserProfile.InfoBag.AddResource((GameResourceType) resId, addon);
            AddFlowCenter(string.Format("{0}+{1}", HSTypes.I2Resource(resId), addon), "Lime");
            UserProfile.InfoCastle.OreDigEp -= 5;
            Invalidate();
        }

        private void bitmapButtonHelp_Click(object sender, EventArgs e)
        {
            MessageBoxEx.Show("通过完成事件和战斗可以累积能量");
        }
    }
}