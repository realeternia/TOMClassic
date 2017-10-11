using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.MainItem;

namespace TaleofMonsters.Forms
{
    internal sealed partial class ChangeResForm : BasePanel
    {
        internal class MemChangeResData
        {
            public int Id1;
            public uint Count1;
            public int Id2;
            public uint Count2;
            public bool Used;

            public bool IsEmpty()
            {
                return Id1 == 0 && Id2 == 0;
            }
        }

        private List<MemChangeResData> changes;
        private ChangeResItem[] changeControls;
        private ColorWordRegion colorWord;

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            changeControls =new ChangeResItem[8];
            for (int i = 0; i < 8; i++)
            {
                changeControls[i] = new ChangeResItem(this, 8 + (i % 2) * 192, 111 + (i / 2) * 55, 193, 56);
                changeControls[i].Init(i);
            }
            GetChangeResData();
            RefreshInfo();
            OnFrame(0, 0);
        }

        private void RefreshInfo()
        {
            for (int i = 0; i < 8; i++)
            {
                changeControls[i].RefreshData();
            }
            bitmapButtonRefresh.Visible = changes.Count < 8;
        }

        public ChangeResForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonRefresh.ImageNormal = PicLoader.Read("Button.Panel", "PlusButton.JPG");
            bitmapButtonRefresh.NoUseDrawNine = true;
            this.bitmapButtonFresh.ImageNormal = PicLoader.Read("Button.Panel", "FreshButton.JPG");
            bitmapButtonFresh.NoUseDrawNine = true;
            colorWord = new ColorWordRegion(12, 38, 384, "微软雅黑", 11, Color.White);
            colorWord.Bold = true;
            colorWord.UpdateText("|交易公式随机出现，交易公式的|Lime|背景颜色||决定交易公式的品质。品质越高交换性价比越高。");
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
                    RefreshAllChangeResData();
                    RefreshInfo();
                }
                else
                {
                    MainTipManager.AddTip(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                }
            }
        }

        private void ChangeResWindow_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("期货", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            colorWord.Draw(e.Graphics);
            foreach (var ctl in changeControls)
            {
                ctl.Draw(e.Graphics);
            }
        }

        public List<MemChangeResData> GetChangeResData()
        {
            changes = new List<MemChangeResData>();
            for (int i = 0; i < 5; i++)
            {
                changes.Add(CreateMethod(i));
            }
            return changes;
        }

        private void AddChangeCardData()
        {
            if (changes.Count < 8)
            {
                changes.Add(CreateMethod(changes.Count));
            }
        }

        public MemChangeResData GetChangeResData(int index)
        {
            if (changes.Count > index)
            {
                return changes[index];
            }
            return new MemChangeResData();
        }

        public void RemoveChangeResData(int index)
        {
            if (changes.Count > index)
            {
                changes[index].Used = true;
            }
        }

        private void RefreshAllChangeResData()
        {
            int count = changes.Count;
            changes.Clear();
            for (int i = 0; i < count; i++)
            {
                changes.Add(CreateMethod(i));
            }
        }
        
        private MemChangeResData CreateMethod(int index)
        {
            MemChangeResData chg = new MemChangeResData();
            int floor = index / 2;
            float cutOff = 1;
            if (floor == 0)
                cutOff = (float)(MathTool.GetRandom(3) + 9) / 10;
            else if (floor == 1)
                cutOff = (float)(MathTool.GetRandom(2) + 9) / 10;
            else if (floor == 2)
                cutOff = (float)(MathTool.GetRandom(4) + 7) / 10;
            else if (floor == 3)
                cutOff = (float)(MathTool.GetRandom(6) + 5) / 10;

            chg.Id1 = MathTool.GetRandom(6) + 1;//1是木材
            while (true)
            {
                chg.Id2 = MathTool.GetRandom(6) + 1;//1是木材
                if(chg.Id1 != chg.Id2)    
                    break;
            }

            uint[] counts = new uint[] {10, 10, 10, 30, 30, 50, 50, 100, 200};
            chg.Count1 = counts[MathTool.GetRandom(counts.Length)];
            chg.Count2 = (uint)(chg.Count1/cutOff);

            if (chg.Id1 <= (int) GameResourceType.Stone && chg.Id2 > (int) GameResourceType.Stone) //汇率问题
                chg.Count2 /= 3;
            if (chg.Id1 > (int)GameResourceType.Stone && chg.Id2 <= (int)GameResourceType.Stone) //汇率问题
                chg.Count2 *= 3;

            return chg;
        }
    }
}