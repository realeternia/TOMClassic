using System.Collections.Generic;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Forms;
using TaleofMonsters.DataType.Tasks;
using System.Drawing;
using ConfigDatas;

namespace TaleofMonsters.DataType.Scenes.SceneObjects
{ 
    class SceneNPC : SceneObject
    {
        private List<int> taskAvails;
        private List<int> taskFinishs;

        public SceneNPC(int npcid)
        {
            Id = npcid;
            NpcConfig npcConfig = ConfigData.GetNpcConfig(npcid);
            Name = npcConfig.Name;
            X = npcConfig.X;
            Y = npcConfig.Y;
            Width = 60;
            Height = 60;
            Figue = npcConfig.Figue;

            taskAvails = TaskBook.GetAvailTask(Id);
            taskFinishs = TaskBook.GetFinishingTask(Id);
        }

        public override void CheckClick()
        {
            int npcFunc = ConfigData.GetNpcConfig(Id).Func;
            if (npcFunc == 0) //∆’Õ®NPC
            {
                NpcTalkForm npcTalkForm = new NpcTalkForm();
                npcTalkForm.NpcId = Id;
                npcTalkForm.SetTasks(taskAvails, taskFinishs);
                MainForm.Instance.DealPanel(npcTalkForm);
            }
            else if (npcFunc < 10) //≤…ºØNPC
            {
                NpcDigForm npcDigForm = new NpcDigForm();
                npcDigForm.NpcId = Id;
                MainForm.Instance.DealPanel(npcDigForm);
            }
        }

        public override void Draw(Graphics g, int target)
        {
            Image head = PicLoader.Read("NPC", string.Format("{0}.PNG", Figue));
            int ty = Y + 50;
            int ty2 = ty;
            if (target == Id)
            {
                g.DrawImage(head, X - 5, ty - 5, Width*5/4, Width * 5 / 4);
                ty2 -= 2;
            }
            else
            {
                g.DrawImage(head, X, ty, Width, Width);
            }

            head.Dispose();

            Font font = new Font("Œ¢»Ì—≈∫⁄", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(Name, font, Brushes.Black, X + 3, Y + Height + 30);
            g.DrawString(Name, font, Brushes.White, X, Y + Height + 27);
            font.Dispose();

            if (taskFinishs.Count > 0)
            {
                Image img = PicLoader.Read("System", "MarkTaskEnd.PNG");
                g.DrawImage(img, X + 15, ty2 - 35, 21, 30);
                img.Dispose();
            }
            else if (taskAvails.Count > 0)
            {
                Image img = PicLoader.Read("System", "MarkTaskBegin.PNG");
                g.DrawImage(img, X + 15, ty2 - 35, 24, 30);
                img.Dispose();
            }
        }
    }
}

