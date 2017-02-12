using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Quests;
using TaleofMonsters.DataType.Scenes;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms
{
    internal sealed partial class QuestForm : BasePanel
    {
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;
        private NLSelectPanel selectPanel;
        private ColorWordRegion colorWord;
        private List<int> items;

        public QuestForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");

            InitTasks();
        }

        public void InitTasks()
        {
            virtualRegion = new VirtualRegion(this);
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
            virtualRegion.AddRegion(new PictureRegion(1, 650, 84, 50, 50, PictureRegionCellType.SceneQuest, 0));
            virtualRegion.AddRegion(new PictureRegion(2, 650, 144, 50, 50, PictureRegionCellType.SceneQuest, 0));
            virtualRegion.AddRegion(new PictureRegion(3, 650, 204, 50, 50, PictureRegionCellType.SceneQuest, 0));
            
            selectPanel = new NLSelectPanel(10, 84, 154, 400, this);
            selectPanel.ItemHeight = 30;
            selectPanel.SelectIndexChanged += selectPanel_SelectedIndexChanged;
            selectPanel.DrawCell += new NLSelectPanel.SelectPanelCellDrawHandler(selectPanel_DrawCell);

            colorWord = new ColorWordRegion(190, 84, 440, "微软雅黑", 11, Color.White);
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);

            comboBoxType.SelectedIndex = 0;
            //RefreshInfo(1);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RefreshInfo(int index)
        {
            items = QuestBook.GetQuestIdByChapter(index);
            selectPanel.ClearContent();
            foreach (var itm in items)
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

            var questConfig = ConfigData.GetQuestConfig(items[selectPanel.SelectIndex]);
            colorWord.Text = questConfig.Descript;

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
            if (selectPanel.SelectIndex >= 0 && items[selectPanel.SelectIndex] == info)
                offX += 15;
            g.DrawString(questConfig.Name, font, Brushes.White, offX, 5 + yOff);
            font.Dispose();
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (id > 0)
            {
              //  if (UserProfile.InfoTask.GetTaskStateById(key) > 0)
                {
                  //  Image image = TaskBook.GetPreview(key);
                  //  tooltip.Show(image, this, x, y);
                }
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