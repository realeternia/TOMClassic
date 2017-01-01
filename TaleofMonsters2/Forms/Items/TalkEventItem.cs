using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.MainItem.Scenes.SceneObjects.SceneQuests;

namespace TaleofMonsters.Forms.Items
{
    internal class TalkEventItem
    {
        private Rectangle pos;
        private SceneQuestEvent evt;
        private int rollItemX;
        private int rollItemSpeedX;
        private SceneQuestBlock result;

        public TalkEventItem(Rectangle r, SceneQuestEvent e)
        {
            pos = r;
            evt = e;
            IsRunning = true;
            rollItemX = MathTool.GetRandom(0, pos.Width);
            rollItemSpeedX = MathTool.GetRandom(20, 40);
        }

        public bool IsRunning { get; set; }

        public SceneQuestBlock GetResult()
        {
            return result;
        }

        public void OnFrame(int tick)
        {
            if (evt.Type == "roll")
            {
                rollItemX += rollItemSpeedX;
                if (rollItemX <0 )
                {
                    rollItemX = 0;
                    rollItemSpeedX *= -1;
                }
                if (rollItemX > pos.Width)
                {
                    rollItemX = pos.Width;
                    rollItemSpeedX *= -1;
                }

                if (MathTool.GetRandom(10) < 2)
                {
                    if (rollItemSpeedX > 0)
                    {
                        rollItemSpeedX = rollItemSpeedX - MathTool.GetRandom(1, 3);
                    }
                    else
                    {
                        rollItemSpeedX = rollItemSpeedX + MathTool.GetRandom(1, 3);
                    }
                    if (rollItemSpeedX == 0)
                    {
                        if (result == null)
                        {
                            IsRunning = false;
                            int frameSize = pos.Width / evt.ParamList.Count;
                            result = evt.ChooseTarget(rollItemX / frameSize);
                        }
                    }
                }
            }
            else
            {
                IsRunning = false;
            }
        }

        public void Draw(Graphics g)
        {
            g.DrawRectangle(Pens.White, pos);
            if (evt.Type == "roll")
            {
                int frameSize = pos.Width / evt.ParamList.Count;
                Font font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                for (int i = 0; i < evt.ParamList.Count; i++)
                {
                    g.FillRectangle(Brushes.White, pos.X+ i* frameSize, pos.Y+30, frameSize-2, 5);
                    g.DrawString(evt.ParamList[i], font, Brushes.White, pos.X + i * frameSize, pos.Y + 10);
                }
                g.FillEllipse(Brushes.Yellow, new Rectangle(pos.X + rollItemX, pos.Y + 40, 6, 6));
                font.Dispose();
            }
            
        }
    }
}
