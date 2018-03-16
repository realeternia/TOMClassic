using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class LevelInfoForm : BasePanel
    {
        private LevelInfoItem[] infoControls;
        public int Level { get; set; }
        public int OldLevel { get; set; }

        private string title;

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            infoControls = new LevelInfoItem[3];
            for (int i = 0; i < 3; i++)
            {
                infoControls[i] = new LevelInfoItem(8, 35 + i * 80, 400, 80);
                infoControls[i].Init(i);
            }
            RefreshInfo();
            SoundManager.Play("System", "LevelUp.mp3");
        }

        private void RefreshInfo()
        {
            OldLevel++;
            var items = LevelInfoBook.GetLevelInfosByLevel(OldLevel);
            for (int j = 0; j < 3; j++)
                infoControls[j].RefreshData(0); //清空
            int i;
            for (i = 0; i < items.Length; i++)
                infoControls[i].RefreshData(items[i]);
            infoControls[i].RefreshData(100); //赠送卡包
            UserProfile.InfoBag.AddItem(HItemBook.GetItemId("kabao1"), 1);
            title = string.Format("恭喜升级到Lv{0}", OldLevel);
            Invalidate();
        }

        public LevelInfoForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
        }

        private void pictureBoxCancel_Click(object sender, EventArgs e)
        {
            if (OldLevel == Level)
                Close();
            else
                RefreshInfo();
        }

        private void LevelInfoForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(title, font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            foreach (var ctl in infoControls)
                ctl.Draw(e.Graphics);
        }
    }
}