using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Rpc;

namespace TaleofMonsters.Forms
{
    internal partial class RankForm : BasePanel
    {
        public RankForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            listView1.OwnerDraw = true;
            listView1.DrawItem += ListView1_DrawItem;

            TalePlayer.C2SSender.GetRank();
        }

        protected override void BasePanelMessageWork(int token)
        {
            RefreshData();
        }

        protected void RefreshData()
        {
            if (NetDataCache.RankList != null)
            {
                int index = 1;
                foreach (var rankData in NetDataCache.RankList)
                    AddText(index++, rankData.Name, rankData.Job, rankData.Level, rankData.Exp);
            }
        }

        private void ListView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (e.ItemIndex%2 == 1)
            {
                var brushB = new SolidBrush(Color.FromArgb(20, 20, 20));
                e.Graphics.FillRectangle(brushB, e.Item.GetBounds(ItemBoundsPortion.Entire));
                brushB.Dispose();
            }

            var items = e.Item.Text.Split('-');
            e.Graphics.DrawString(items[0], listView1.Font, Brushes.White, e.Item.Position.X+20, e.Item.Position.Y + 3);
            e.Graphics.DrawString(items[1], listView1.Font, Brushes.White, e.Item.Position.X + 50, e.Item.Position.Y + 3);
            e.Graphics.DrawString(ConfigData.GetJobConfig(int.Parse(items[2])).Name, listView1.Font, Brushes.White, e.Item.Position.X + 140, e.Item.Position.Y + 3);
            e.Graphics.DrawString(string.Format("Lv{0}({1})",items[3], items[4]), listView1.Font, Brushes.White, e.Item.Position.X + 190, e.Item.Position.Y + 3);
        }

        private void RankForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("排行榜", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddText(int index, string name, int job, int level, int exp)
        {
            ListViewItem item = new ListViewItem();
            item.Text = string.Format("{0}-{1}-{2}-{3}-{4}", index, name, job, level, exp);
            listView1.Items.Add(item);
        }
    }
}
