using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class EquipComposeForm : BasePanel
    {
        private EquipComposeItem[] equipControls;
        private int page;
        private List<int> equipIdList;
        private ControlPlus.NLPageSelector nlPageSelector1;

        public EquipComposeForm()
        {
            InitializeComponent();            
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButton1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack3.PNG");
            this.bitmapButton2.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack1.PNG");
            this.bitmapButton3.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack1.PNG");
            this.bitmapButton4.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack1.PNG");
            this.bitmapButton5.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack1.PNG");

            this.nlPageSelector1 = new ControlPlus.NLPageSelector(this, 398, 310, 150);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            equipControls = new EquipComposeItem[9];
            for (int i = 0; i < 9; i++)
            {
                equipControls[i] = new EquipComposeItem(this, 10 + (i % 3) * 180, 63 + (i / 3) * 82, 180, 82);
                equipControls[i].Init(i);
            }
            InitEquips(1);
        }

        private void InitEquips(int pos)
        {
            equipIdList = new List<int>();
            foreach (var eid in UserProfile.Profile.InfoEquip.EquipComposeAvail)
            {
                var equipConfig = ConfigData.GetEquipConfig(eid);
                if (equipConfig.Id > 0 && equipConfig.Position == pos)
                    equipIdList.Add(equipConfig.Id);
            }
            page = 0;
            nlPageSelector1.TotalPage = (equipIdList.Count - 1) / 9 + 1;
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            for (int i = 0; i < 9; i++)
            {
                equipControls[i].RefreshData((page*9 + i < equipIdList.Count) ? equipIdList[page*9 + i] : 0);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void EquipComposeForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 建造 ", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            foreach (var ctl in equipControls)
                ctl.Draw(e.Graphics);
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            page = pg;
            RefreshInfo();
        }

        private void bitmapButton1_Click(object sender, EventArgs e)
        {
            InitEquips(int.Parse((sender as Control).Tag.ToString()));
        }
    }
}