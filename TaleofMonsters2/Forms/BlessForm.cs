using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
using NarlonLib.Core;
using NarlonLib.Tools;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain.Blesses;

namespace TaleofMonsters.Forms
{
    internal sealed partial class BlessForm : BasePanel
    {
        private BlessItem[] blessControls;
        private List<int> blesses;
        private string timeText;
        private int showType;

        public BlessForm()
        {
            InitializeComponent();

            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButton1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack1.PNG");
            this.bitmapButton2.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack3.PNG");
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            blessControls = new BlessItem[12];
            for (int i = 0; i < 12; i++)
            {
                blessControls[i] = new BlessItem(this, 9 + (i % 3) * 180, 60 + (i / 3) * 77, 180, 77);
                blessControls[i].Init(i);
            }
            ChangeType(1);
        }

        delegate void RefreshCallback();
        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);
            if ((tick % 6) == 0)
            {
                TimeSpan span = TimeTool.UnixTimeToDateTime(UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastBlessShopTime) + GameConstants.BlessShopDura) - DateTime.Now;
                if (span.TotalSeconds > 0)
                {
                    timeText = string.Format("更新剩余 {0}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
                    Invalidate(new Rectangle(445, 375, 150, 20));
                }
                else
                {
                    showType = 1;
                    BeginInvoke(new RefreshCallback(RefreshPage));
                }
            }
        }

        private void RefreshInfo()
        {
            for (int i = 0; i < 12; i++)
            {
                blessControls[i].RefreshData((i < blesses.Count) ? blesses[i] : 0);
            }
        }

        private void ChangeType(int type)
        {
            showType = type;
            if (type == 1)
                blesses = UserProfile.InfoWorld.GetBlessShopData();
            else
                blesses = BlessManager.GetNegtiveBless();

            RefreshInfo();
            Invalidate(new Rectangle(9, 35, 66, 30*5));
        }

        public void RefreshPage()
        {
            ChangeType(showType);
        }

        private void bitmapButton1_Click(object sender, EventArgs e)
        {
            ChangeType(int.Parse((sender as Control).Tag.ToString()));
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BlessViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 祝福 ", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            foreach (var ctl in blessControls)
                ctl.Draw(e.Graphics);

            font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(timeText, font, Brushes.YellowGreen, 445, 375);
            font.Dispose();
        }
    }

}