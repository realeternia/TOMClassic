using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Control;
using NarlonLib.Drawing;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Quests;
using TaleofMonsters.DataType.Scenes;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms
{
    internal sealed partial class QuestForm : BasePanel
    {
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;
        private NLSelectPanel selectPanel;
        private ColorWordRegion colorWord;
        private List<int> questIds;

        public QuestForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");

            InitTasks();
        }

        public void InitTasks()
        {
            virtualRegion = new VirtualRegion(this);
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
            for (int i = 0; i < 3; i++)
            {
                var region = new PictureRegion(i + 1, 650, 84 + 60*i, 50, 50, PictureRegionCellType.SceneQuest, 0);
                region.AddDecorator(new RegionBorderDecorator(Color.Lime));
                virtualRegion.AddRegion(region); 
            }
          
            selectPanel = new NLSelectPanel(10, 84, 154, 400, this);
            selectPanel.ItemHeight = 30;
            selectPanel.SelectIndexChanged += selectPanel_SelectedIndexChanged;
            selectPanel.DrawCell += new NLSelectPanel.SelectPanelCellDrawHandler(selectPanel_DrawCell);

            colorWord = new ColorWordRegion(190, 84, 440, "微软雅黑", 11, Color.White);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            comboBoxType.SelectedIndex = 0;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RefreshInfo(int index)
        {
            questIds = QuestBook.GetQuestIdByChapter(index);
            selectPanel.ClearContent();
            foreach (var itm in questIds)
            {
                selectPanel.AddContent(itm);
            }
            selectPanel.SelectIndex = 0;

            Invalidate(selectPanel.Rectangle);
           // UpdateMethod();
        }

        private void selectPanel_SelectedIndexChanged()
        {
            if (selectPanel.SelectIndex < 0)
            {
                return;
            }

            var questConfig = ConfigData.GetQuestConfig(questIds[selectPanel.SelectIndex]);
            if (!UserProfile.InfoQuest.IsQuestFinish(questConfig.Id))
            {
                return;
            }
            
            colorWord.UpdateText(questConfig.Descript);

            virtualRegion.SetRegionKey(1,0);
            virtualRegion.SetRegionKey(2, 0);
            virtualRegion.SetRegionKey(3, 0);

            if (!string.IsNullOrEmpty(questConfig.Quest1))
                virtualRegion.SetRegionKey(1, SceneBook.GetSceneQuestByName(questConfig.Quest1));
            if (!string.IsNullOrEmpty(questConfig.Quest2))
                virtualRegion.SetRegionKey(2, SceneBook.GetSceneQuestByName(questConfig.Quest2));
            if (!string.IsNullOrEmpty(questConfig.Quest3))
                virtualRegion.SetRegionKey(3, SceneBook.GetSceneQuestByName(questConfig.Quest3));
            
            Invalidate();
        }

        private void selectPanel_DrawCell(Graphics g, int info, int xOff, int yOff)
        {
            QuestConfig questConfig = ConfigData.GetQuestConfig(info);
         //   g.DrawImage(SceneQuestBook.GetEquipImage(info), 5 + xOff, 5 + yOff, 40, 40);
            Font font = new Font("微软雅黑", 11.25F * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            int offX = 40 + xOff;
            if (selectPanel.SelectIndex >= 0 && questIds[selectPanel.SelectIndex] == info)
                offX += 15;
            if (UserProfile.InfoQuest.IsQuestFinish(questConfig.Id))
            {
                g.DrawString(questConfig.Name, font, Brushes.White, offX, 5 + yOff);
            }
            else
            {
                g.DrawString("???", font, Brushes.DarkGray, offX, 5 + yOff);
            }
            
            font.Dispose();
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (selectPanel.SelectIndex < 0 || selectPanel.SelectIndex >= questIds.Count)
            {
                return;
            }

            var questConfig = ConfigData.GetQuestConfig(questIds[selectPanel.SelectIndex]);
            if (!UserProfile.InfoQuest.IsQuestFinish(questConfig.Id))
            {
                return;
            }

            string sceneQuestId = "";
            string subContext = "";
            if (id ==1)
            {
                sceneQuestId = questConfig.Quest1;
                subContext = questConfig.SubDescript1;
            }
            if (id == 2)
            {
                sceneQuestId = questConfig.Quest2;
                subContext = questConfig.SubDescript2;
            }
            if (id == 3)
            {
                sceneQuestId = questConfig.Quest3;
                subContext = questConfig.SubDescript3;
            }
            if (!string.IsNullOrEmpty(sceneQuestId))
            {
                var name = ConfigData.GetSceneQuestConfig(SceneBook.GetSceneQuestByName(sceneQuestId)).Name;
                Image image = DrawTool.GetImageByString(name, subContext, 150, Color.White);
                tooltip.Show(image, this, x, y);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void TaskForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 传记 ", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            colorWord.Draw(e.Graphics);

            virtualRegion.Draw(e.Graphics);
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshInfo(comboBoxType.SelectedIndex + 1);
        }

    }
}