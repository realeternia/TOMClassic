using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.MainItem.Scenes;
using TaleofMonsters.MainItem.Scenes.SceneObjects.SceneQuests;

namespace TaleofMonsters.Forms
{
    internal sealed partial class NpcTalkForm : BasePanel
    {
        private int tar = -1;
        private ColorWordRegion colorWord;
        public int EventId { get; set; }
        private SceneQuestBlock questItem;

        public NpcTalkForm()
        {
            InitializeComponent();
            colorWord = new ColorWordRegion(22, 77, 268, "宋体", 10, Color.White);
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            questItem = SceneManager.GetQuestData("test");
        }

        private void NpcTalkForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (tar != -1)
            {
                questItem = questItem.Children[tar]; //对话换页
                if (questItem.Children.Count == 0)
                {
                    Close();
                }
                this.Invalidate();
            }
        }

        private void TalkWindow_Paint(object sender, PaintEventArgs e)
        {
            if (questItem != null)
            {
                Image bgImage = PicLoader.Read("System", "TalkBack.PNG");
                e.Graphics.DrawImage(bgImage, 0, 0, bgImage.Width, bgImage.Height);
                bgImage.Dispose();
              //  e.Graphics.DrawImage(NPCBook.GetPersonImage(npcId), 24, 0, 70, 70);

                //Font font = new Font("宋体", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                //e.Graphics.DrawString(ConfigData.GetNpcConfig(npcId).Name, font, Brushes.Chocolate, 131, 50);
                //font.Dispose();

                 Font font = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                int id = 0;
                foreach (var word in questItem.Children)
                {
                    if (id == tar)
                    {
                        e.Graphics.FillRectangle(Brushes.DarkBlue, 22, id * 20 + 262 - questItem.Children.Count * 20, 258, 20);
                    }
                    e.Graphics.DrawString(word.Script, font, Brushes.Wheat, 22, id * 20 + 262 - questItem.Children.Count * 20);
                
                    id++;
                }
                font.Dispose();
                colorWord.Draw(e.Graphics);
            }
        }

        private void NpcTalkForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > 25 && e.X < 353 && e.Y > 262 - questItem.Children.Count * 20 && e.Y < 262)
            {
                int val = (e.Y - 262 + questItem.Children.Count * 20) / 20;
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