using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.Forms
{
    internal sealed partial class LevelInfoForm : BasePanel
    {
        private LevelInfoItem[] infoControls;
        private int level;

        public int Level
        {
            set { level = value; }
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);

            infoControls = new LevelInfoItem[3];
            for (int i = 0; i < 3; i++)
            {
                infoControls[i] = new LevelInfoItem(this, 8, 35 + i * 80, 400, 80);
                infoControls[i].Init(i);
            }
            refreshInfo();
        }

        private void refreshInfo()
        {
            int[] ids = LevelInfoBook.GetLevelInfosByLevel(level);
            for (int i = 0; i < 3; i++)
            {
                infoControls[i].RefreshData(i < ids.Length ? ids[i] : 0);
            }
        }

        public LevelInfoForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
        }

        private void pictureBoxCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LevelInfoForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("新功能", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            foreach (LevelInfoItem ctl in infoControls)
            {
                ctl.Draw(e.Graphics);
            }
        }
    }
}