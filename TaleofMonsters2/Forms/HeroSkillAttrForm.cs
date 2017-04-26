using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.DataType.HeroSkills;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class HeroSkillAttrForm : BasePanel
    {
        private HeroSkillAttrItem[] skillControls;
        private int page;
        private int[] skills;
        private ControlPlus.NLPageSelector nlPageSelector1;

        public HeroSkillAttrForm()
        {
            InitializeComponent();            
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            this.nlPageSelector1 = new ControlPlus.NLPageSelector(this, 398, 282, 150);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            skills = HeroSkillAttrBook.GetAvailSkills(UserProfile.InfoBasic.Level);
            page = 0;
            skillControls = new HeroSkillAttrItem[9];
            for (int i = 0; i < 9; i++)
            {
                skillControls[i] = new HeroSkillAttrItem(this, 10 + (i % 3) * 180, 35 + (i / 3) * 82, 180, 82);
                skillControls[i].Init(i);
            }
            nlPageSelector1.TotalPage = (skills.Length-1)/9 + 1;
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            for (int i = 0; i < 9; i++)
            {
                skillControls[i].RefreshData((page*9 + i < skills.Length) ? skills[page*9 + i] : 0);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void HeroSkillAttrForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 奇术 ", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            foreach (HeroSkillAttrItem ctl in skillControls)
            {
                ctl.Draw(e.Graphics);
            }

            font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            string str = string.Format("我的阅历:  {0} ", UserProfile.InfoBasic.AttrPoint);
            int strWid = (int)e.Graphics.MeasureString(str, font).Width;
            e.Graphics.DrawString(str, font, Brushes.White, 20, 290);
            font.Dispose();
            e.Graphics.DrawImage(HSIcons.GetIconsByEName("oth6"), 20 + strWid, 289, 14, 14);
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            page = pg;
            RefreshInfo();
        }
    }
}