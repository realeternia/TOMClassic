using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
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
        private CellItemBox itemBox;
        private List<int> blesses;
        private string timeText;
        private int showType;

        public BlessForm()
        {
            InitializeComponent();

            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButton1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack1.PNG");
            this.bitmapButton2.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack3.PNG");

            itemBox = new CellItemBox(9, 60, 180*3, 77*4);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            for (int i = 0; i < 12; i++)
            {
                var item = new BlessItem(this);
                itemBox.AddItem(item);
                item.Init(i);
            }
            ChangeType(1);
        }

        delegate void RefreshCallback(int type);
        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);
            if ((tick % 6) == 0)
            {
                TimeSpan span = TimeTool.UnixTimeToDateTime(UserProfile.InfoRecord.GetStateById(MemPlayerStateTypes.LastBlessShopTime) + GameConstants.BlessShopDura) - DateTime.Now;
                if (span.TotalSeconds > 0)
                {
                    timeText = string.Format("更新剩余 {0}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
                    Invalidate(new Rectangle(445, 375, 150, 20));
                }
                else
                {
                    showType = 1;
                    BeginInvoke(new RefreshCallback(ChangeType));
                }
            }
        }

        public override void RefreshInfo()
        {
            if (showType == 1)
                blesses = UserProfile.InfoWorld.GetBlessShopData();
            else
                blesses = BlessManager.GetNegtiveBless();
            for (int i = 0; i < 12; i++)
                itemBox.Refresh(i, (i < blesses.Count) ? blesses[i] : 0);
            Invalidate(new Rectangle(9, 35, 66, 30 * 5));
        }

        private void ChangeType(int type)
        {
            showType = type;
            RefreshInfo();
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

            itemBox.Draw(e.Graphics);

            font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(timeText, font, Brushes.YellowGreen, 445, 375);
            font.Dispose();
        }
        public override void OnRemove()
        {
            base.OnRemove();

            itemBox.Dispose();
        }
    }

}