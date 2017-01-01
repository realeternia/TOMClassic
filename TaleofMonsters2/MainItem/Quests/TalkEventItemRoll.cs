using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemRoll : TalkEventItem
    {
        private int rollItemX;
        private int rollItemSpeedX;

        public TalkEventItemRoll(Rectangle r, SceneQuestEvent e)
            : base(r, e)
        {
            rollItemX = MathTool.GetRandom(0, pos.Width);
            rollItemSpeedX = MathTool.GetRandom(20, 40);
        }

        public override void OnFrame(int tick)
        {
            rollItemX += rollItemSpeedX;
            if (rollItemX < 0)
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
                        RunningState = TalkEventState.Finish;
                        int frameSize = pos.Width / evt.ParamList.Count;
                        result = evt.ChooseTarget(rollItemX / frameSize);
                    }
                }
            }
        }
        public override void Draw(Graphics g)
        {
            g.DrawRectangle(Pens.White, pos);
            int frameSize = pos.Width / evt.ParamList.Count;
            Font font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            for (int i = 0; i < evt.ParamList.Count; i++)
            {
                g.FillRectangle(Brushes.White, pos.X + i * frameSize, pos.Y + 30, frameSize - 2, 5);
                g.DrawString(evt.ParamList[i], font, Brushes.White, pos.X + i * frameSize, pos.Y + 10);
            }
            g.FillEllipse(Brushes.Yellow, new Rectangle(pos.X + rollItemX, pos.Y + 40, 6, 6));
            font.Dispose();
        }
    }
}