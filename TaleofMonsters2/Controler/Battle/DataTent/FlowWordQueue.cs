using System;
using System.Collections.Generic;
using System.Drawing;
using TaleofMonsters.Controler.Battle.Data.MemFlow;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal class FlowWordQueue
    {
        private List<FlowWord> queue = new List<FlowWord>();
        private bool isFast;

        #region 模仿数组迭代
        public FlowWord this[int index]
        {
            get { return queue[index]; }
            set { queue[index] = value; }
        }

        public int Count
        {
            get { return queue.Count; }
        }

        public void Add(FlowWord flowWord)
        {
            if (isFast)
            {
                return;
            }

            if (flowWord.NoOverlap)
            {
                foreach (FlowWord word in queue)
                {
                    if (word.NoOverlap)
                    {
                        if (Math.Abs(word.Position.X - flowWord.Position.X) < 50 && Math.Abs(word.Position.Y - flowWord.Position.Y) < 20)
                        {
                            flowWord.Position = new Point(flowWord.Position.X, word.Position.Y + 20);
                        }
                    }
                }
            }

            queue.Add(flowWord);
        }

        public void Remove(FlowWord flowWord)
        {
            queue.Remove(flowWord);
        }

        public void RemoveAt(int index)
        {
            queue.RemoveAt(index);
        }

        public void Clear()
        {
            queue.Clear();
        }
        #endregion

        public void SetFast()
        {
            isFast = true;
        }

        public void Next()
        {
            lock (queue)
            {
                for (int i = queue.Count - 1; i >= 0; i--)
                {
                    if (queue[i].IsFinished)
                        queue.RemoveAt(i);
                }
            }
        }
    }
}
