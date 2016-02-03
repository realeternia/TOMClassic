using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using ConfigDatas;

namespace TaleofMonsters.Forms
{
    internal sealed partial class LevelUpForm : BasePanel
    {
        private int[] point;
        private int[] skillcommon;
        private int stindex;
        private int sindex;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;

        public LevelUpForm()
        {
            InitializeComponent();
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("ButtonBitmap", "LevelButton.JPG");
            virtualRegion = new VirtualRegion(this);
            virtualRegion.AddRegion(new SubVirtualRegion(1, 5, 5, 40, 40, 1));
            virtualRegion.AddRegion(new SubVirtualRegion(2, 5, 55, 40, 40, 2));
            virtualRegion.AddRegion(new SubVirtualRegion(3, 155, 5, 40, 40, 3));
            virtualRegion.AddRegion(new SubVirtualRegion(4, 155, 55, 40, 40, 4));
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);

            CheckLevelInfo();
        }

        private void CheckLevelInfo()
        {
            SoundManager.Play("System", "LevelUp.wav");

            int nowlevel = UserProfile.InfoBasic.Level + 1;
            Text = string.Format("恭喜你提升到{0}级", nowlevel);

            point = new int[8];
            for (int i = 0; i < 8; i++)
            {
                point[i] = 0;
            }

            RandomMaker maker = new RandomMaker();
            skillcommon = maker.Process(3);

            sindex = -1;
            stindex = 0;
        }

        private void LevelUpForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(Text, font, Brushes.White, Width / 2 - 70, 8);
            font.Dispose();

            Font font2 = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            for (int i = 0; i < 8; i++)
            {
                e.Graphics.DrawImage(HSIcons.GetIconsByEName(string.Format("hatt{0}", i + 1)), 29 + (i % 2) * 150, 66 + (i / 2) * 24, 24, 24);
                e.Graphics.DrawString(string.Format(ConfigData.GetEquipAddonConfig(i + 1).Format, point[i]), font2, Brushes.White, 29 + (i % 2) * 150 + 32, 72 + (i / 2) * 24);
            }
            font2.Dispose();
        }

        private void buttonLevelUp_Click(object sender, System.EventArgs e)
        {
            if (UserProfile.InfoBasic.CheckNewLevel())
            {
                //for (int i = 0; i < 8; i++)
                //{
                //    UserProfile.InfoBasic.AddSkillValueById(i + 1, point[i]);
                //}
                if (stindex >= 0 && stindex < 3)
                {
                //    UserProfile.InfoSkill.AddSkillCommon(skillcommon[stindex]);
                }
            }

            if (UserProfile.InfoBasic.Exp >= ExpTree.GetNextRequired(UserProfile.InfoBasic.Level))
            {
                CheckLevelInfo();
                Invalidate();
                panelSkill.Invalidate();
            }
            else
            {
                MainItem.SystemMenuManager.ResetIconState();
                MainForm.Instance.RefreshPanel();
                Close();

                MainItem.PanelManager.ShowLevelInfo(UserProfile.InfoBasic.Level);
            }
        }

        private void panelSkill_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.DarkBlue, 150 * (stindex / 2), 50 * (stindex % 2), 150, 50);

            Font font2 = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            //if (skillcommon[0] > 0)
            //{
            //    e.Graphics.DrawImage(HeroSkillBook.GetHeroSkillCommonImage(skillcommon[0]), 5, 5, 40, 40);
            //    e.Graphics.DrawString(ConfigData.GetHeroSkillCommonConfig(skillcommon[0]).Name, font2, Brushes.White, 59, 17);
            //}
            //if (skillcommon[1] > 0)
            //{
            //    e.Graphics.DrawImage(HeroSkillBook.GetHeroSkillCommonImage(skillcommon[1]), 5, 55, 40, 40);
            //    e.Graphics.DrawString(ConfigData.GetHeroSkillCommonConfig(skillcommon[1]).Name, font2, Brushes.White, 59, 67);
            //}
            //if (skillcommon[2] > 0)
            //{
            //    e.Graphics.DrawImage(HeroSkillBook.GetHeroSkillCommonImage(skillcommon[2]), 155, 5, 40, 40);
            //    e.Graphics.DrawString(ConfigData.GetHeroSkillCommonConfig(skillcommon[2]).Name, font2, Brushes.White, 209, 17);
            //}
         //   e.Graphics.DrawImage(HeroSkillBook.GetHeroSkillCommonDirectImage("skill99"), 155, 55, 40, 40);
            e.Graphics.DrawString("灵气+1", font2, Brushes.White, 209, 67);

            font2.Dispose();
            if (sindex >= 0)
            {
                Pen pen = new Pen(Color.Goldenrod, 3);
                e.Graphics.DrawRectangle(pen, 150 * (sindex / 2), 50 * (sindex % 2), 147, 47);
                pen.Dispose();
            }
        }

        private void panelSkill_MouseMove(object sender, MouseEventArgs e)
        {
            sindex = (e.X/150)*2 + e.Y/50;
            if (sindex < 3 && sindex >= 0 && skillcommon[sindex] == 0)
            {
                sindex = -1;
            }
            panelSkill.Invalidate();
        }

        private void panelSkill_MouseLeave(object sender, System.EventArgs e)
        {
            sindex = -1;
            panelSkill.Invalidate();
        }

        private void panelSkill_MouseClick(object sender, MouseEventArgs e)
        {
            if (sindex != -1 && sindex != stindex)
            {
                stindex = sindex;
                panelSkill.Invalidate();
            }
        }

        private void virtualRegion_RegionEntered(int info, int x, int y, int key)
        {
            int id = info;
            if (id < 4 && skillcommon[id - 1] != 0)
            {
                int sid = skillcommon[id - 1];
               // Image image = DrawTool.GetImageByString(ConfigData.GetHeroSkillCommonConfig(sid).Des, 200);
             //   tooltip.Show(image, this, panelSkill.Location.X + x, panelSkill.Location.Y + y);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }
    }
}