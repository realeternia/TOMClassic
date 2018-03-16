using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Quests;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms
{
    internal sealed partial class QuestForm : BasePanel
    {
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;

        public QuestForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            InitQuests();
        }

        public void InitQuests()
        {
            vRegion = new VirtualRegion(this);
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
            RefreshQuests();
        }

        private void RefreshQuests()
        {
            vRegion.ClearRegion();
            int index = 0;

            var sceneConfig = ConfigData.GetSceneConfig(UserProfile.InfoBasic.MapId);
            var regionId = sceneConfig.RegionId;

            foreach (var questData in ConfigData.QuestDict.Values)
            {
                if (questData.RegionId != regionId)
                    continue;
                Image img = null;
                Color borderColor = Color.Black;
                if (UserProfile.InfoQuest.IsQuestNotReceive(questData.Id))
                    img = HSIcons.GetIconsByEName("npc1");
                else if (UserProfile.InfoQuest.IsQuestFinish(questData.Id))
                    img = HSIcons.GetIconsByEName("npc4");
                else if (UserProfile.InfoQuest.IsQuestCanReward(questData.Id))
                    img = HSIcons.GetIconsByEName("npc3");
                else
                    img = HSIcons.GetIconsByEName("npc2");
                if(questData.TypeR == 0) //主线
                    borderColor = Color.Gold;
                var region = new ImageRegion(index++, 30 + questData.X * 24, 75 + questData.Y * 24, 20, 20, ImageRegionCellType.None, img);
                region.SetKeyValue(questData.Id);
                region.AddDecorator(new RegionBorderDecorator(borderColor));
                vRegion.AddRegion(region);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            var questConfig = ConfigData.GetQuestConfig(key);
            if (UserProfile.InfoQuest.IsQuestNotReceive(questConfig.Id))
                return;
            Image image = QuestBook.GetPreview(questConfig.Id);
            tooltip.Show(image, this, x, y);
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void QuestForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 任务 ", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            font = new Font("黑体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            var sceneConfig = ConfigData.GetSceneConfig(UserProfile.InfoBasic.MapId);
            e.Graphics.DrawString(string.Format("{0} ({1})", ConfigData.GetSceneRegionConfig(sceneConfig.RegionId).Name, sceneConfig.Name), font, Brushes.White, 33, 40);
            
            Pen darkPen = new Pen(Color.FromArgb(30,30,30));
            for (int i = 1; i <= 16; i++)
            {
                e.Graphics.DrawLine(darkPen, 40+i*24, 85, 40 + i * 24, 369);
                e.Graphics.DrawString((i).ToString(), font, Brushes.DimGray, 30 + i * 24, 67);
            }
            for (int i = 1; i <= 11; i++)
            {
                e.Graphics.DrawLine(darkPen, 40, 85 + i * 24, 448, 85 + i * 24);
                e.Graphics.DrawString(((char)('A'+i-1)).ToString(), font, Brushes.DimGray, 20, 77 + i * 24);
            }
            darkPen.Dispose();
            font.Dispose();
            e.Graphics.DrawRectangle(Pens.DimGray, 40, 85, 408, 284);
            vRegion.Draw(e.Graphics);
        }
    }
}