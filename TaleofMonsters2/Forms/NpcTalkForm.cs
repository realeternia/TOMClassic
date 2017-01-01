using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.DataType.NPCs;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.MainItem.Quests;
using TaleofMonsters.MainItem.Quests.SceneQuests;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.Forms
{
    internal sealed partial class NpcTalkForm : BasePanel
    {
        private int tar = -1;
        private SceneQuestBlock interactBlock;

        private ColorWordRegion colorWord;//问题区域
        public int EventId { get; set; }
        private List<string> answerList; //回答区
        private TalkEventItem evtItem; //事件交互区

        public NpcTalkForm()
        {
            InitializeComponent();
            colorWord = new ColorWordRegion(140, 38, Width-150, "宋体", 14, Color.White);
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            interactBlock = SceneManager.GetQuestData("test");
            answerList = new List<string>();
            SetupQuestItem();
        }

        internal override void OnFrame(int tick)
        {
            base.OnFrame(tick);

            if (evtItem != null && evtItem.RunningState == TalkEventItem.TalkEventState.Running)
            {
                evtItem.OnFrame(tick);
                if (evtItem.RunningState == TalkEventItem.TalkEventState.Finish)
                {
                    interactBlock = evtItem.GetResult();
                    if (interactBlock is SceneQuestSay)
                    {
                        SetupQuestItem();
                    }
                    else if (interactBlock == null)
                    {
                        answerList.Clear();
                        answerList.Add("结束");
                    }
                }
                Invalidate();
            }
        }

        private void NpcTalkForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (tar != -1)
            {
                if (interactBlock == null)//一般是最后一选了
                {
                    Close();
                    return;
                }

                interactBlock = interactBlock.Children[tar]; //对话换页
                evtItem = null;
                if (interactBlock.Children.Count == 1 && interactBlock.Children[0] is SceneQuestSay)
                {
                    interactBlock = interactBlock.Children[0];
                    SetupQuestItem();
                }
                else if (interactBlock.Children.Count == 1 && interactBlock.Children[0] is SceneQuestEvent)
                {
                    evtItem = TalkEventItem.CreateEventItem(new Rectangle(10, Height - 10 - 5 * 20 - 160, Width - 20, 160), interactBlock.Children[0] as SceneQuestEvent);
                }
                this.Invalidate();
            }
        }

        private void SetupQuestItem()
        {
            colorWord.Text = interactBlock.Script;
            answerList.Clear();
            foreach (var sceneQuestBlock in interactBlock.Children)
            {
                answerList.Add(sceneQuestBlock.Script);
            }
        }

        private void TalkWindow_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            e.Graphics.DrawImage(NPCBook.GetPersonImage(41000001), 15, 35, 120, 140);

            if (evtItem != null)
            {
                evtItem.Draw(e.Graphics);
            }

            colorWord.Draw(e.Graphics);

            if (answerList != null && (evtItem == null || evtItem.RunningState != TalkEventItem.TalkEventState.Running ))
            {
                Font font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                int id = 0;
                foreach (var word in answerList)
                {
                    if (id == tar)
                    {
                        e.Graphics.FillRectangle(Brushes.DarkBlue, 10, id * 20 + Height - 10 - answerList.Count * 20, Width - 20, 20);
                    }
                    e.Graphics.DrawString(word, font, Brushes.Wheat, 22, id * 20 + Height - 10 - answerList.Count * 20 + 2);

                    id++;
                }
                font.Dispose();
            }
        }

        private void NpcTalkForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (evtItem == null || evtItem.RunningState != TalkEventItem.TalkEventState.Running) //evtItem运行期间无法选择
            {
                if (e.Y > Height - 10 - answerList.Count * 20 && e.Y < Height - 10)
                {
                    int val = (e.Y - (Height - 10) + answerList.Count * 20) / 20;
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
}