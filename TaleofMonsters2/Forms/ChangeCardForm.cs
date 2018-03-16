using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class ChangeCardForm : BasePanel
    {
        internal class MemChangeCardData
        {
            public int Id1;
            public int Type1;
            public int Id2;
            public int Type2;
            public bool Used;

            public bool IsEmpty()
            {
                return Id1 == 0 && Id2 == 0;
            }
        }

        private List<MemChangeCardData> changes;
        private ChangeCardItem[] changeControls;
        private ColorWordRegion colorWord;

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            changeControls =new ChangeCardItem[8];
            for (int i = 0; i < 8; i++)
            {
                changeControls[i] = new ChangeCardItem(this, 8 + (i % 2) * 192, 111 + (i / 2) * 55, 193, 56);
                changeControls[i].Init(i);
            }
            GetChangeCardData();
            RefreshInfo();
            OnFrame(0, 0);
        }

        private void RefreshInfo()
        {
            for (int i = 0; i < 8; i++)
                changeControls[i].RefreshData();
            bitmapButtonRefresh.Visible = changes.Count < 8;
        }

        public ChangeCardForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonRefresh.ImageNormal = PicLoader.Read("Button.Panel", "PlusButton.JPG");
            bitmapButtonRefresh.NoUseDrawNine = true;
            this.bitmapButtonFresh.ImageNormal = PicLoader.Read("Button.Panel", "FreshButton.JPG");
            bitmapButtonFresh.NoUseDrawNine = true;
            colorWord = new ColorWordRegion(12, 38, 384, new Font("微软雅黑", 11 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel), Color.White);
            colorWord.UpdateText("|交换公式随机出现，交换公式的|Lime|背景颜色||决定交换公式的最高品质。");
        }

        private void pictureBoxCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bitmapButtonRefresh_Click(object sender, EventArgs e)
        {
            var cost = GameResourceBook.OutSulfurRefresh(0.5f);
            if (MessageBoxEx2.Show(string.Format("是否花{0}硫磺增加一条交换公式?", cost)) == DialogResult.OK)
            {
                if (UserProfile.InfoBag.HasResource(GameResourceType.Sulfur, cost))
                {
                    UserProfile.InfoBag.SubResource(GameResourceType.Sulfur, cost);
                    AddChangeCardData();
                    RefreshInfo();
                }
                else
                {
                    MainTipManager.AddTip(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                }
            }
        }

        private void bitmapButtonFresh_Click(object sender, EventArgs e)
        {
            var cost = GameResourceBook.OutSulfurRefresh(1);
            if (MessageBoxEx2.Show(string.Format("是否花{0}硫磺刷新所有交换公式?", cost)) == DialogResult.OK)
            {
                if (UserProfile.InfoBag.HasResource(GameResourceType.Sulfur, cost))
                {
                    UserProfile.InfoBag.SubResource(GameResourceType.Sulfur, cost);
                    RefreshAllChangeCardData();
                    RefreshInfo();
                }
                else
                {
                    MainTipManager.AddTip(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                }
            }
        }

        private void ChangeCardWindow_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("交换", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            colorWord.Draw(e.Graphics);
            foreach (var ctl in changeControls)
                ctl.Draw(e.Graphics);
        }

        public List<MemChangeCardData> GetChangeCardData()
        {
            changes = new List<MemChangeCardData>();
            for (int i = 0; i < 5; i++)
                changes.Add(CreateMethod(i));
            return changes;
        }

        private void AddChangeCardData()
        {
            if (changes.Count < 8)
                changes.Add(CreateMethod(changes.Count));
        }

        public MemChangeCardData GetChangeCardData(int index)
        {
            if (changes.Count > index)
                return changes[index];
            return new MemChangeCardData();
        }

        public void RemoveChangeCardData(int index)
        {
            if (changes.Count > index)
                changes[index].Used = true;
        }

        private void RefreshAllChangeCardData()
        {
            int count = changes.Count;
            changes.Clear();
            for (int i = 0; i < count; i++)
                changes.Add(CreateMethod(i));
        }
        
        private MemChangeCardData CreateMethod(int index)
        {
            MemChangeCardData chg = new MemChangeCardData();
            int level = MathTool.GetRandom(Math.Max(index / 2, 1), index / 2 + 3);
            chg.Id1 = MonsterBook.GetRandStarMid(level);
            while (true)
            {
                chg.Id2 = MonsterBook.GetRandStarMid(level);
                if (chg.Id2 != chg.Id1)
                    break;
            }
            chg.Type1 = 1;
            chg.Type2 = 1;

            return chg;
        }
    }
}