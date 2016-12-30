using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.NPCs;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.DataType.Tasks;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using ConfigDatas;

namespace TaleofMonsters.Forms
{
    internal sealed partial class NpcTalkForm : BasePanel
    {
        private int tar = -1;
        private int npcId;
        private string[] say;
        private string[] answers;
        private List<TalkWord> talkList;
        private ColorWordRegion colorWord;

        private List<int> taskAvails;
        private List<int> taskFinishs;

        public int NpcId
        {
            set { npcId = value; }
        }

        public void SetTasks(List<int> avails, List<int> finishs)
        {
            taskAvails = avails;
            taskFinishs = finishs;
        }

        public NpcTalkForm()
        {
            InitializeComponent();
            colorWord = new ColorWordRegion(22, 77, 268, "宋体", 10, Color.White);
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            NpcConfig npcConfig = ConfigData.GetNpcConfig(npcId);
            say = npcConfig.Say.Split(':');
            answers = npcConfig.Answer.Split(':');
            ChangeTo(0);
        }

        private void ChangeTo(int id)
        {
            colorWord.Text = say[id * 2 + 1];
            string[] answer = answers[id*2 + 1].Split('|');
            talkList = new List<TalkWord>();
            if (id == 0)
            {
                foreach (int tid in taskAvails)
                {
                    TalkWord temptalk = new TalkWord(TaskBook.GetReceiveWord(tid), -1);
                    if (TaskBook.CanReceive(tid))
                    {
                        talkList.Add(temptalk);
                    }
                }
                foreach (int tid in taskFinishs)
                {
                    if (TaskBook.CanFinish(tid))
                    {
                        string[] strs = TaskBook.GetFinishWord(tid);
                        foreach (string str in strs)
                        {
                            TalkWord temptalk = new TalkWord(str, -1);
                            talkList.Add(temptalk);
                        }
                    }
                }
            }
            for (int i = 0; i < answer.Length/2; i++)
            {
                TalkWord temptalk = new TalkWord(answer[i*2 + 1], int.Parse(answer[i*2]));
                if (temptalk.IsAvail())
                {
                    talkList.Add(temptalk);
                }
            }
            Invalidate();
        }

        private void NpcTalkForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (tar != -1)
            {
                int id = talkList[tar].Target; //对话换页
                if (id != -1)
                {
                    ChangeTo(id);
                    return;
                }

                talkList[tar].CheckParameter();

                string actionType;
                int actionData;
                talkList[tar].CheckAction(out actionType, out actionData);

                if (actionType != "") //如果需要动作
                {
                    switch (actionType)
                    {
                        case "Shp":
                            NpcShopForm sw = new NpcShopForm();
                            sw.NpcId = npcId;
                            sw.ShopId = actionData;
                            MainForm.Instance.DealPanel(sw);
                            break;
                        case "Maz":
                            MazeWindow mw = new MazeWindow();
                            mw.NpcId = npcId;
                            mw.MazeId = actionData;
                            MainForm.Instance.DealPanel(mw);
                            break;
                        case "Tbe":
                            NpcTaskBeginForm bw = new NpcTaskBeginForm();
                            bw.NpcId = npcId;
                            bw.TaskId = actionData;
                            MainForm.Instance.DealPanel(bw);
                            break;
                        case "Ted":
                            TaskResultForm trf = new TaskResultForm();
                            trf.SetInfo(actionData);
                            MainForm.Instance.DealPanel(trf);
                            break;
                        case "Mon":
                            PeopleBook.Fight(actionData, "house", -1, ConfigData.GetNpcConfig(npcId).Lv, null, null);
                            break;
                        case "Job":
                             MainForm.Instance.DealPanel(new SelectJobForm());
                            break;
                    }
                }
                //else if (taskid != 0)
                //{
                //    UserProfile.Profile.EndTask(taskid);
                //}
                Close();
            }
        }

        private void TalkWindow_Paint(object sender, PaintEventArgs e)
        {
            if (npcId > 0)
            {
                Image bgImage = PicLoader.Read("System", "TalkBack.PNG");
                e.Graphics.DrawImage(bgImage, 0, 0, bgImage.Width, bgImage.Height);
                bgImage.Dispose();
                e.Graphics.DrawImage(NPCBook.GetPersonImage(npcId), 24, 0, 70, 70);

                Font font = new Font("宋体", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                e.Graphics.DrawString(ConfigData.GetNpcConfig(npcId).Name, font, Brushes.Chocolate, 131, 50);
                font.Dispose();

                int id = 0;
                foreach (TalkWord word in talkList)
                {
                    word.Draw(e.Graphics, 22, id*20 + 262 - talkList.Count*20, id == tar);
                    id++;
                }

                colorWord.Draw(e.Graphics);
            }
        }

        private void NpcTalkForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > 25 && e.X < 353 && e.Y > 262 - talkList.Count * 20 && e.Y<262)
            {
                int val = (e.Y - 262 + talkList.Count*20)/20;
                if (val != tar)
                {
                    tar = val;
                    Invalidate();
                    return;
                }
            }
            else
            {
                tar = -1;
            }
        }
    }
}