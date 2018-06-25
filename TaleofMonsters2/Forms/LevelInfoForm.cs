using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
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
        internal class LevelInfoData
        {
            public int Id;
            public string Des;
        }

        private CellItemBox itemBox;
        public int Level { get; set; }
        public int OldLevel { get; set; }

        private string title;

        public LevelInfoForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");

            itemBox = new CellItemBox(8, 35, 400, 80 * 3);
        }


        public override void Init(int width, int height)
        {
            base.Init(width, height);

            for (int i = 0; i < 3; i++)
            {
                var item = new LevelInfoItem(this);
                itemBox.AddItem(item);
                item.Init(i);
            }
            RefreshInfo();
            SoundManager.Play("System", "LevelUp.mp3");
        }

        public override void RefreshInfo()
        {
            OldLevel++;
            var items = LevelInfoBook.GetLevelInfosByLevel(OldLevel);
            for (int j = 0; j < 3; j++)
                itemBox.Refresh(j, new LevelInfoData {Id = 0, Des = ""}); //清空
            int i;
            for (i = 0; i < items.Length; i++)
                itemBox.Refresh(i, new LevelInfoData { Id = items[i], Des = ConfigData.GetLevelInfoConfig(items[i]).Des});
            foreach (var jobConfig in ConfigData.JobDict.Values)
            {
                if (jobConfig.LevelNeed == OldLevel)
                {
                    itemBox.Refresh(i, new LevelInfoData { Id = 101, Des  = ConfigData.GetLevelInfoConfig(101).Des.Replace("Job", jobConfig.Name)}); //开启职业
                    i++;
                    break;
                }
            }
            itemBox.Refresh(i, new LevelInfoData { Id = 100, Des = ConfigData.GetLevelInfoConfig(100).Des}); //赠送卡包
            UserProfile.InfoBag.AddItem(HItemBook.GetItemId("kabao1"), 1);
            title = string.Format("恭喜升级到Lv{0}", OldLevel);
            Invalidate();
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
            itemBox.Draw(e.Graphics);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            itemBox.Dispose();
        }
    }
}