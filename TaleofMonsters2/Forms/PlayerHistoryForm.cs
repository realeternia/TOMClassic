using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal partial class PlayerHistoryForm : BasePanel
    {
        public PlayerHistoryForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            if (ParentPanel != null)
                Location = new Point(ParentPanel.Location.X + ParentPanel.Width, ParentPanel.Location.Y);

            listView1.OwnerDraw = true;
            listView1.DrawItem += ListView1_DrawItem;
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.CardGet).Cname, RecordInfoConfig.Indexer.CardGet, Color.White, "tsk7");
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.TotalEvent).Cname, RecordInfoConfig.Indexer.TotalEvent, Color.White);
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.QuestFinish).Cname, RecordInfoConfig.Indexer.QuestFinish, Color.White);
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.GismoGet).Cname, RecordInfoConfig.Indexer.GismoGet, Color.White);
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.ItemGet).Cname, RecordInfoConfig.Indexer.ItemGet, Color.White, "tsk4");
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.EquipGet).Cname, RecordInfoConfig.Indexer.EquipGet, Color.White);
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.TotalDie).Cname, RecordInfoConfig.Indexer.TotalDie, Color.Red, "oth11");
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.AddBless).Cname, RecordInfoConfig.Indexer.AddBless, Color.White);
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.AddCurse).Cname, RecordInfoConfig.Indexer.AddCurse, Color.Red);

            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.FightAttend).Cname, RecordInfoConfig.Indexer.FightAttend, Color.White, "abl1");
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.TotalWin).Cname, RecordInfoConfig.Indexer.TotalWin, Color.White, "oth1");
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.ContinueWin).Cname, RecordInfoConfig.Indexer.ContinueWin, Color.White, "oth1");
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.TotalKill).Cname, RecordInfoConfig.Indexer.TotalKill, Color.White);
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.TotalSummon).Cname, RecordInfoConfig.Indexer.TotalSummon, Color.White);
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.TotalWeapon).Cname, RecordInfoConfig.Indexer.TotalWeapon, Color.White);
            AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.TotalSpell).Cname, RecordInfoConfig.Indexer.TotalSpell, Color.White);

            for (int i = 0; i <= 6; i++)
                AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.TotalUseAttr + i).Cname, RecordInfoConfig.Indexer.TotalUseAttr+i, Color.White, "atr"+i);
            for (int i = (int)CardTypeSub.Devil; i <= (int)CardTypeSub.Totem; i++)
                AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.TotalUseRace + i).Cname, RecordInfoConfig.Indexer.TotalUseRace + i, Color.White, "rac"+i);
            for (int i = 0; i <= 6; i++)
                AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.TotalKillAttr + i).Cname, RecordInfoConfig.Indexer.TotalKillAttr + i, Color.Red, "atr" + i);
            for (int i = (int)CardTypeSub.Devil; i <= (int)CardTypeSub.Totem; i++)
                AddText(ConfigData.GetRecordInfoConfig(RecordInfoConfig.Indexer.TotalKillRace + i).Cname, RecordInfoConfig.Indexer.TotalKillRace + i, Color.Red, "rac" + i);
        }

        private void ListView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (e.ItemIndex%2 == 1)
            {
                var brushB = new SolidBrush(Color.FromArgb(20, 20, 20));
                e.Graphics.FillRectangle(brushB, e.Item.GetBounds(ItemBoundsPortion.Entire));
                brushB.Dispose();
            }

            var iconInfo = (string) e.Item.Tag;
            if (iconInfo != "")
                e.Graphics.DrawImage(HSIcons.GetIconsByEName(iconInfo), e.Item.Position.X, e.Item.Position.Y + 3, 16, 16);

            var items = e.Item.Text.Split('-');
            var brush = new SolidBrush(e.Item.ForeColor);
            e.Graphics.DrawString(items[0], listView1.Font, brush, e.Item.Position.X + (iconInfo == "" ? 0 : 20), e.Item.Position.Y+3);
            brush.Dispose();
            e.Graphics.DrawString(items[1], listView1.Font, Brushes.White, e.Item.Position.X+180, e.Item.Position.Y + 3);
            //  e.DrawText();
        }

        private void PlayerHistoryForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("你的记录", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddText(string type, int recordId, Color color, string icon="")
        {
            ListViewItem item = new ListViewItem();
            item.Text = string.Format("{0}-{1}", type,
                UserProfile.InfoRecord.GetRecordById(recordId).ToString());
            item.ForeColor = color;
            item.Tag = icon;
            listView1.Items.Add(item);
        }
    }
}
