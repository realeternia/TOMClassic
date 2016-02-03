using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Core;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.User.Mem;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class ChangeCardForm : BasePanel
    {
        private List<MemChangeCardData> changes;
        private ChangeCardItem[] changeControls;
        private ColorWordRegion colorWord;
        private string timeText;

        internal override void Init(int width, int height)
        {
            base.Init(width, height);

            changeControls =new ChangeCardItem[8];
            for (int i = 0; i < 8; i++)
            {
                changeControls[i] = new ChangeCardItem(this, 8 + (i % 2) * 192, 111 + (i / 2) * 55, 193, 56);
                changeControls[i].Init(i);
            }
            RefreshInfo();
            OnFrame(0);
        }

        private void RefreshInfo()
        {
            changes = UserProfile.InfoWorld.GetChangeCardData();
            for (int i = 0; i < 8; i++)
            {
                changeControls[i].RefreshData();
            }
            bitmapButtonRefresh.Visible = changes.Count < 8;
        }

        public ChangeCardForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonRefresh.ImageNormal = PicLoader.Read("ButtonBitmap", "PlusButton.JPG");
            bitmapButtonRefresh.NoUseDrawNine = true;
            this.bitmapButtonFresh.ImageNormal = PicLoader.Read("ButtonBitmap", "FreshButton.JPG");
            bitmapButtonFresh.NoUseDrawNine = true;
            colorWord = new ColorWordRegion(12, 38, 384, "微软雅黑", 11, Color.White);
            colorWord.Bold = true;
            colorWord.Text = "|每|Red|24小时||随机更新5条交换公式，交换公式的|Lime|背景颜色||决定交换公式的最高品质。";
        }

        private void pictureBoxCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bitmapButtonRefresh_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx2.Show("是否花5钻石增加一条交换公式?") == DialogResult.OK)
            {
                if (UserProfile.InfoBag.PayDiamond(5))
                {
                    UserProfile.InfoWorld.AddChangeCardData();
                    RefreshInfo();
                }
            }
        }

        private void bitmapButtonFresh_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx2.Show("是否花10钻石刷新所有交换公式?") == DialogResult.OK)
            {
                if (UserProfile.InfoBag.PayDiamond(10))
                {
                    UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.LastCardChangeTime, 0);
                    UserProfile.InfoWorld.RefreshAllChangeCardData();
                    RefreshInfo();
                }
            }
        }

        delegate void RefreshInfoCallback();
        internal override void OnFrame(int tick)
        {
            if ((tick % 6) == 0)
            {
                TimeSpan span = TimeTool.UnixTimeToDateTime(UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastCardChangeTime) + SysConstants.ChangeCardDura) - DateTime.Now;
                if (span.TotalSeconds > 0)
                {
                    timeText = string.Format("更新剩余 {0}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
                    Invalidate(new Rectangle(12, 345, 150, 20));
                }
                else
                {
                    BeginInvoke(new RefreshInfoCallback(RefreshInfo));
                }
            }
        }

        private void ChangeCardWindow_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("交换", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(timeText, font, Brushes.YellowGreen, 12, 345);
            font.Dispose();

            colorWord.Draw(e.Graphics);
            foreach (ChangeCardItem ctl in changeControls)
            {
                ctl.Draw(e.Graphics);
            }
        }
    }
}