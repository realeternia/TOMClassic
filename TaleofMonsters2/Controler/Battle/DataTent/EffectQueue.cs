using System.Collections.Generic;
using TaleofMonsters.Controler.Battle.Data.MemEffect;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal class EffectQueue
    {
        private List<BaseEffect> queue = new List<BaseEffect>();
        private bool isFast;

        public List<BaseEffect> Enumerator
        {
            get { return queue; }
        }

        #region 模仿数组迭代
        public BaseEffect this[int index]
        {
            get { return queue[index]; }
            set { queue[index] = value; }
        }

        public int Count
        {
            get { return queue.Count; }
        }

        public void Add(BaseEffect effect)
        {
            if (isFast)
            {
                return;
            }

            queue.Add(effect);
        }

        public void Remove(BaseEffect effect)
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
                        if (effect.IsFinished == RunState.Finished)
                            queue.RemoveAt(i);
                    }
                }
            }
        }
    }
}
