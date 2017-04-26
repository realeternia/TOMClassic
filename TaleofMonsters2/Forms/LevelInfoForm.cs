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
        public int Level { get; set; }
        public int OldLevel { get; set; }

        private int[] infoDatas;
        private int nowIndex;

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            infoControls = new LevelInfoItem[3];
            for (int i = 0; i < 3; i++)
            {
                infoControls[i] = new LevelInfoItem(this, 8, 35 + i * 80, 400, 80);
                infoControls[i].Init(i);
            }
            infoDatas = LevelInfoBook.GetLevelInfosByLevel(OldLevel, Level);
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            for (int i = 0; i < 3; i++)
            {
                infoControls[i].RefreshData(i + nowIndex < infoDatas.Length ? infoDatas[i + nowIndex] : 0);
            }
            nowIndex += 3;
        }

        public LevelInfoForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
        }

        private void pictureBoxCancel_Click(object sender, EventArgs e)
        {
            if (nowIndex >= infoDatas.Length)
            {
                Close();
            }
            else
            {
                RefreshInfo();
            }
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