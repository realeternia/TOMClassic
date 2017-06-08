using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Achieves;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using ConfigDatas;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Forms
{
    internal sealed partial class AchieveViewForm : BasePanel
    {
        private VirtualRegion virtualRegion;

        private AchieveItem[] achieveControls;
        private int page;
        private int[] achieveIds;
        private ControlPlus.NLPageSelector nlPageSelector1;

        public AchieveViewForm()
        {
            InitializeComponent();
            virtualRegion = new VirtualRegion(this);
            for (int i = 0; i < 5; i++)
            {
                ButtonRegion region = new ButtonRegion(i + 1, 9, 35 + 30 * i, 74, 24, "CommonButton1.JPG", "CommonButton1On.JPG");
                region.AddDecorator(new RegionTextDecorator(18, 7, 10, Color.Black));
                virtualRegion.AddRegion(region);
            }
            virtualRegion.SetRegionDecorator(1, 0, "常 规");
            virtualRegion.SetRegionDecorator(2, 0, "资 源");
            virtualRegion.SetRegionDecorator(3, 0, "杀 敌");
            virtualRegion.SetRegionDecorator(4, 0, "召 唤");
            virtualRegion.SetRegionDecorator(5, 0, "战 斗");

            virtualRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);

            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.nlPageSelector1 = new ControlPlus.NLPageSelector(this, 463, 422, 150);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            achieveControls = new AchieveItem[15];
            for (int i = 0; i < 15; i++)
            {
                achieveControls[i] = new AchieveItem(this, 75 + (i % 3) * 180, 35 + (i / 3) * 77, 180, 77);
                achieveControls[i].Init(i);
            }
            ChangeType(1);
        }

        private void RefreshInfo()
        {
            for (int i = 0; i < 15; i++)
            {
                achieveControls[i].RefreshData((page * 15 + i < achieveIds.Length) ? achieveIds[page * 15 + i] : 0);
            }
        }

        private void ChangeType(int type)
        {
            page = 0;
            AchieveConfig[] achieves = AchieveBook.GetAchievesByType(type);
            achieveIds=new int[achieves.Length];
            for (int i = 0; i < achieves.Length; i++)
            {
                achieveIds[i] = achieves[i].Id;
            }
            Array.Sort(achieveIds, new CompareByAid());
            nlPageSelector1.TotalPage = (achieveIds.Length - 1) / 15 + 1;
            RefreshInfo();
            for (int i = 0; i < 5; i++)
            {
                virtualRegion.SetRegionState(i + 1, RegionState.Free);
            }
            virtualRegion.SetRegionState(type, RegionState.Blacken);
            Invalidate(new Rectangle(9, 35, 66, 30*5));
        }

        void virtualRegion_RegionClicked(int id, int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                ChangeType(id);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AchieveViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 成就 ", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            virtualRegion.Draw(e.Graphics);

            foreach (AchieveItem ctl in achieveControls)
            {
                ctl.Draw(e.Graphics);
            }
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            page = pg;
            RefreshInfo();
        }

        private class CompareByAid : IComparer<int>
        {
            #region IComparer<int> 成员

            public int Compare(int x, int y)
            {
                bool finishx = UserProfile.Profile.GetAchieveState(x) >= ConfigData.AchieveDict[x].Condition.Value;
                bool finishy = UserProfile.Profile.GetAchieveState(y) >= ConfigData.AchieveDict[y].Condition.Value;
                if (finishx && !finishy)
                {
                    return -1;
                }
                if (!finishx && finishy)
                {
                    return 1;
                }
                return x.CompareTo(y);
            }

            #endregion
        }
    }

}