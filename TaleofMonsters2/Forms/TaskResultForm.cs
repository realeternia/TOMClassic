using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Equips;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms
{
    internal sealed partial class TaskResultForm : BasePanel
    {
        private bool show;
        private int taskId;
       // private List<int> items;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;
        private List<int> itemTypeList = new List<int>();//存下道具的类型

        public TaskResultForm()
        {
            InitializeComponent();
            this.bitmapButtonClose2.ImageNormal = PicLoader.Read("ButtonBitmap", "CancelButton.JPG");
            bitmapButtonClose2.NoUseDrawNine = true;
            virtualRegion = new VirtualRegion(this);
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public void SetInfo(int tid)
        {
            taskId = tid;
            SoundManager.Play("System", "QuestCompleted.wav");

            TaskConfig taskConfig = ConfigData.GetTaskConfig(taskId);
            var itemIndex = itemTypeList.Count + 1;
            if (taskConfig.Card != 0 && CardConfigManager.GetCardConfig(taskConfig.Card).Id > 0)
            {
                virtualRegion.AddRegion(new PictureAnimRegion(itemIndex, 15+80*itemIndex, 200, 60, 60, PictureRegionCellType.Card, taskConfig.Card));
                itemTypeList.Add(3);
                itemIndex++;
            }
            for (int i = 0; i < taskConfig.Item.Count; i++)
            {
                var type = taskConfig.Item[i].Value == 1 ? PictureRegionCellType.Item : PictureRegionCellType.Equip;
                virtualRegion.AddRegion(new PictureAnimRegion(itemIndex, 15 + 80 * itemIndex, 200, 60, 60, type, taskConfig.Item[i].Id));
                itemTypeList.Add(taskConfig.Item[i].Value);
                itemIndex++;
            }

            show = true;
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (id > itemTypeList.Count)
            {
                return;
            }

            int type = itemTypeList[id - 1];
            Image image = null;
            if (type == 1)
            {
                image = HItemBook.GetPreview(key);
            }
            else if (type == 2)
            {
                Equip equip = new Equip(key);
                image = equip.GetPreview();
            }
            else
            {
                image = CardAssistant.GetCard(key).GetPreview(CardPreviewType.Normal, new int[] { });
            }
            tooltip.Show(image, this, x, y);
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
       //     TaskBook.Award(taskId);
       //     UserProfile.InfoTask.EndTask(taskId);
            Close();
        }

        private void TaskResultForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("任务完成", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            Image back = PicLoader.Read("System", "TaskResultBack.JPG");
            e.Graphics.DrawImage(back, 15, 35, 504, 284);
            back.Dispose();

            virtualRegion.Draw(e.Graphics);

            if (show)
            {
                font = new Font("微软雅黑", 15*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                Font font2 = new Font("宋体", 12*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                TaskConfig taskConfig = ConfigData.GetTaskConfig(taskId);
                e.Graphics.DrawString(taskConfig.Name, font, Brushes.White, 101, 51);
                GameResource ges = GameResource.Parse(taskConfig.Resource);
            //    ges.Add(GameResourceType.Gold, TaskBook.GetMoneyReal(taskId));
                int[] res = ges.ToArray();
                e.Graphics.DrawString(string.Format("{0}", ges.Gold), font2, Brushes.White, 155, 103);
                e.Graphics.DrawString(string.Format("{0}", 111), font2, Brushes.White, 155, 123);
                for (int i = 0; i < 6; i++)
                {
                    e.Graphics.DrawString(res[i + 1].ToString(), font2, Brushes.Pink, 135 + 68 * i, 159);
                }
                font.Dispose();
                font2.Dispose();
            }
        }

    }
}