using System.Collections.Generic;
using TaleofMonsters.Controler.Battle.Data.MemMissile;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal class MissileQueue
    {
        List<Missile> queue = new List<Missile>();
        private bool isFast;

        public List<Missile> Enumerator
        {
            get { return queue; }
        }

        #region 模仿数组迭代
        public Missile this[int index]
        {
            get { return queue[index]; }
            set { queue[index] = value; }
        }

        public int Count
        {
            get { return queue.Count; }
        }

        public void Add(Missile effect)
        {
            if (isFast)
            {
                return;
            }

            queue.Add(effect);
        }

        public void Remove(Missile effect)
        {
            queue.Remove(effect);
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
                    if (i < queue.Count)
                    {
                        var effect = queue[i];
                        effect.Next();
                        if (effect.IsFinished)
                        {
                            queue.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }

}
