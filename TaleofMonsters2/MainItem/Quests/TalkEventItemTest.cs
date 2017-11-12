using System;
using System.Collections.Generic;
using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.DataType.User;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemTest : TalkEventItem
    {
        private int attrVal;
        private int markNeed;

        private List<int> rollItemX;
        private List<int> rollItemSpeedX;

        private List<bool> hasStop;

        private int[] markArray = {1, 2, 1, 0, 1, 2};
        private const int FrameOff = 10; //第一个柱子相叫最左边的偏移

        public TalkEventItemTest(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            var type = int.Parse(evt.ParamList[0]);
            bool canConvert = type == 1; //是否允许转换成幸运检测
            var testType = type == 1 ? config.TestType1 : config.TestType2;
            var biasData = type == 1 ? config.TestBias1 : config.TestBias2;

            if (UserProfile.InfoDungeon.DungeonId > 0)
            {
                attrVal = Math.Max(1, UserProfile.InfoDungeon.GetAttrByStr(testType));
                markNeed = UserProfile.InfoDungeon.GetRequireAttrByStr(testType, biasData);
            }
            else
            {
                attrVal = 3;
                markNeed = 3 + biasData;
            }

            rollItemX = new List<int>();
            rollItemSpeedX = new List<int>();
            hasStop = new List<bool>();
            for (int i = 0; i < attrVal; i++)
            {
                rollItemX.Add(MathTool.GetRandom(0, pos.Width));
                rollItemSpeedX.Add(MathTool.GetRandom(20, 40));
                hasStop.Add(false);
            }
        }

        public override void OnFrame(int tick)
        {
            for (int i = 0; i < attrVal; i++)
            {
                if (hasStop[i])
                    continue;

                rollItemX[i] += rollItemSpeedX[i];
                if (rollItemX[i] < 0)
                {
                    rollItemX[i] = 0;
                    rollItemSpeedX[i] *= -1;
                }
                if (rollItemX[i] > pos.Width - FrameOff * 2)
                {
                    rollItemX[i] = pos.Width - FrameOff * 2;
                    rollItemSpeedX[i] *= -1;
                }

                if (MathTool.GetRandom(10) < 2)
                {
                    if (rollItemSpeedX[i] > 0)
                    {
                        rollItemSpeedX[i] = rollItemSpeedX[i] - MathTool.GetRandom(1, 3);
                    }
                    else
                    {
                        rollItemSpeedX[i] = rollItemSpeedX[i] + MathTool.GetRandom(1, 3);
                    }
                    if (Math.Abs(rollItemSpeedX[i]) <= 1)
                    {
                        hasStop[i] = true;
                    }
                }
            }

            if (hasStop.IndexOf(false) < 0)
            {
                OnStop();
            }
        }

        private void OnStop()
        {
            if (result == null)
            {
                RunningState = TalkEventState.Finish;

                var mark = GetNowMark();
                if (mark >= markNeed)
                    result = evt.ChooseTarget(1);
                else
                    result = evt.ChooseTarget(0);
            }
        }

        private int GetNowMark()
        {
            int markGet = 0;
            int frameSize = (pos.Width - FrameOff*2)/markArray.Length;
            for (int i = 0; i < rollItemX.Count; i++)
            {
                markGet += markArray[Math.Min(markArray.Length-1, rollItemX[i]/frameSize)];
            }
            return markGet;
        }

        public override void Draw(Graphics g)
        {
            // g.DrawRectangle(Pens.White, pos);

            Font font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(string.Format("请等待 需求：{0}，当前：{1}", markNeed, GetNowMark()), font, Brushes.White, pos.X + 3, pos.Y + 3);
            font.Dispose();

            g.DrawLine(Pens.Wheat, pos.X + 3, pos.Y + 3 + 20, pos.X + 3 + 400, pos.Y + 3 + 20);

            int frameSize = (pos.Width - FrameOff * 2) / markArray.Length;
            font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            for (int i = 0; i < markArray.Length; i++)
            {
                Brush b;
                switch (markArray[i])
                {
                    case 1: b = Brushes.Yellow; break;
                    case 2: b = Brushes.Lime; break;
                    case 0: b = Brushes.Red; break;
                    default: b = Brushes.Wheat; break;
                }
                g.FillRectangle(b, pos.X + i * frameSize + FrameOff, pos.Y + 25 + 30, frameSize - 2, 5);
                g.DrawString(markArray[i] + "分", font, b, pos.X + i * frameSize + FrameOff + frameSize / 2 - 20, pos.Y + 25 + 10);
            }
            for (int i = 0; i < attrVal; i++)
            {
                g.FillEllipse(Brushes.Yellow, new Rectangle(pos.X + rollItemX[i] + FrameOff - 6, pos.Y + 25 + 40 + i * 15, 12, 12));
                g.FillEllipse(Brushes.OrangeRed, new Rectangle(pos.X + rollItemX[i] + FrameOff - 1, pos.Y + 25 + 40 + 5 + i*15, 3, 3));
            }
            font.Dispose();
        }
    }
}