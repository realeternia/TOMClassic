using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal partial class CastleForm : BasePanel
    {
        public CastleForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            DoubleBuffered = true;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
        }

        private void CastleForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("城堡", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            var img = PicLoader.Read("System", "CastleBack.JPG");
            e.Graphics.DrawImage(img, 10, 35, Width-20, Height-45);
            img.Dispose();
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
