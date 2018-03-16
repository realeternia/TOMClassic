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
    internal partial class UserForm : BasePanel
    {
        public UserForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            listView1.OwnerDraw = true;
            listView1.DrawItem += ListView1_DrawItem;
            AddText("获得卡牌", MemPlayerRecordTypes.CardGet, Color.White, "tsk7");
            AddText("经历事件数", MemPlayerRecordTypes.TotalEvent, Color.White);
            AddText("完成任务", MemPlayerRecordTypes.QuestFinish, Color.White);
            AddText("副本成就", MemPlayerRecordTypes.GismoGet, Color.White);
            AddText("道具取得", MemPlayerRecordTypes.ItemGet, Color.White, "tsk4");
            AddText("装备取得", MemPlayerRecordTypes.EquipGet, Color.White);
            AddText("死亡次数", MemPlayerRecordTypes.TotalDie, Color.Red, "oth11");
            AddText("获得祝福", MemPlayerRecordTypes.AddBless, Color.White);
            AddText("获得诅咒", MemPlayerRecordTypes.AddCurse, Color.Red);

            AddText("参加战斗", MemPlayerRecordTypes.FightAttend, Color.White, "abl1");
            AddText("总胜场次", MemPlayerRecordTypes.TotalWin, Color.White, "oth1");
            AddText("连胜场次", MemPlayerRecordTypes.ContinueWin, Color.White, "oth1");
            AddText("击杀怪物", MemPlayerRecordTypes.TotalKill, Color.White);
            AddText("召唤怪物", MemPlayerRecordTypes.TotalSummon, Color.White);
            AddText("使用道具", MemPlayerRecordTypes.TotalWeapon, Color.White);
            AddText("使用法术", MemPlayerRecordTypes.TotalSpell, Color.White);

            for (int i = 0; i <= 6; i++)
                AddText(string.Format("使用{0}属性牌", HSTypes.I2Attr(i)), MemPlayerRecordTypes.TotalUseAttr+i, Color.White, "atr"+i);
            for (int i = (int)CardTypeSub.Devil; i <= (int)CardTypeSub.Totem; i++)
                AddText(string.Format("使用{0}种族牌", ConfigData.GetMonsterRaceConfig(i).Name), MemPlayerRecordTypes.TotalUseRace + i, Color.White, "rac"+i);
            for (int i = 1; i <= 7; i++)
                AddText(string.Format("使用{0}星卡牌", i), MemPlayerRecordTypes.TotalUseLevel + i, Color.White, "sysstar");
            for (int i = 0; i <= 6; i++)
                AddText(string.Format("击杀{0}属性怪", HSTypes.I2Attr(i)), MemPlayerRecordTypes.TotalKillAttr + i, Color.Red, "atr" + i);
            for (int i = (int)CardTypeSub.Devil; i <= (int)CardTypeSub.Totem; i++)
                AddText(string.Format("击杀{0}种族怪", ConfigData.GetMonsterRaceConfig(i).Name), MemPlayerRecordTypes.TotalKillRace + i, Color.Red, "rac" + i);
            for (int i = 1; i <= 7; i++)
                AddText(string.Format("击杀{0}星卡怪", i), MemPlayerRecordTypes.TotalKillLevel + i, Color.Red, "sysstar");
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

        private void UserForm_Paint(object sender, PaintEventArgs e)
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

        private void AddText(string type, MemPlayerRecordTypes recordId, Color color, string icon="")
        {
            ListViewItem item = new ListViewItem();
            item.Text = string.Format("{0}-{1}", type,
                UserProfile.InfoRecord.GetRecordById((int) recordId).ToString());
            item.ForeColor = color;
            item.Tag = icon;
            listView1.Items.Add(item);
        }
    }
}
