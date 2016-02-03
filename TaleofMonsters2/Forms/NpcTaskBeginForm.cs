using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.NPCs;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms
{
    internal sealed partial class NpcTaskBeginForm : BasePanel
    {
        private int taskId;
        private int npcId;
        private int tar;
        private ColorWordRegion colorWord;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;

        public int TaskId
        {
            set { taskId = value; }
        }

        public int NpcId
        {
            set { npcId = value; }
        }

        public NpcTaskBeginForm()
        {
            InitializeComponent();
            colorWord = new ColorWordRegion(22, 97, 268, "宋体", 10, Color.White);
            virtualRegion = new VirtualRegion(this);
            virtualRegion.AddRegion(new SubVirtualRegion(1, 22, 230, 258, 20, 1));
            virtualRegion.AddRegion(new SubVirtualRegion(2, 22, 250, 258, 20, 2));
            virtualRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClicked);
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            TaskConfig taskConfig = ConfigData.GetTaskConfig(taskId);
            colorWord.Text = taskConfig.Descript;
            if (taskConfig.StartNpc != 0)
            {
                virtualRegion.AddRegion(new PictureAnimRegion(11, 100, 150, 30, 30, 11, VirtualRegionCellType.Npc, taskConfig.StartNpc));
            }
            if (taskConfig.EndNpc != 0 && taskConfig.StartNpc != taskConfig.EndNpc)
            {
                virtualRegion.AddRegion(new PictureAnimRegion(12, 150, 150, 30, 30, 12, VirtualRegionCellType.Npc, taskConfig.EndNpc));
            }

            int pos = 0;
            if (taskConfig.Type == TaskTypes.Item)
            {
                for (int i = 0; i < taskConfig.NeedItemId.Length; i ++)
                {
                    virtualRegion.AddRegion(new PictureAnimRegion(20 + pos, 100 + pos * 50, 190, 30, 30, 20 + pos, VirtualRegionCellType.Item, taskConfig.NeedItemId[i]));
                    pos++;
                }
            }

            Invalidate();
        }

        private void virtualRegion_RegionEntered(int info, int x, int y, int key)
        {
            if (info >= 20)
            {
                Image image = HItemBook.GetPreview(key);
                tooltip.Show(image, this, x, y);  
            }
            tar = info;
            Invalidate();
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }        

        private void virtualRegion_RegionClicked(int info, MouseButtons button)
        {
            if (info == 1)
            {
                TaskConfig taskConfig = ConfigData.GetTaskConfig(taskId);
                if (taskConfig.Type == TaskTypes.Fight)
                {
                    PeopleBook.Fight(taskConfig.FightId, "house", taskConfig.FightLand, taskConfig.Level, null, OnFightWin);
                }
                else
                {
                    UserProfile.InfoTask.BeginTask(taskId);
                }
            }
            Close();
        }

        private void OnFightWin()
        {
            UserProfile.InfoTask.SetTaskStateById(taskId, 2);
        }

        private void TaskBeginWindow_Paint(object sender, PaintEventArgs e)
        {
            if (npcId > 0)
            {
                Image bgImage = PicLoader.Read("System", "TalkBack.PNG");
                e.Graphics.DrawImage(bgImage, 0, 0, bgImage.Width, bgImage.Height);
                bgImage.Dispose();
                e.Graphics.DrawImage(NPCBook.GetPersonImage(npcId), 24, 0, 70, 70);


                Font font = new Font("宋体", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                e.Graphics.DrawString(ConfigDatas.ConfigData.GetNpcConfig(npcId).Name, font, Brushes.Chocolate, 131, 50);
                font.Dispose();

                font = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                SolidBrush sb = new SolidBrush(Color.FromArgb(255, 160, 255, 180));
                e.Graphics.DrawString("任务目标", font, sb, 23, 78);
                e.Graphics.DrawString("相关人物", font, sb, 24, 152);
                e.Graphics.DrawString("要点提示", font, sb, 24, 192);
                sb.Dispose();

                if (tar != 0)
                    e.Graphics.FillRectangle(Brushes.DarkBlue, 22, tar*20 + 208, 258, 20);
                e.Graphics.DrawString("接受任务", font, Brushes.LimeGreen, 22, 231);
                e.Graphics.DrawString("取消", font, Brushes.DarkGray, 22, 251);
                font.Dispose();

                colorWord.Draw(e.Graphics);

                virtualRegion.Draw(e.Graphics);
            }
        }
    }
}