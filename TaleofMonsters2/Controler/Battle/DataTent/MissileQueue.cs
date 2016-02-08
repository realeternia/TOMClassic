using System.Collections.Generic;
using System.Drawing;
using NarlonLib.Core;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemMonster;

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

    /// <summary>
    /// 锁定或者非锁定的投射物
    /// </summary>
    internal class Missile
    {
        private int effectImgId;
        private LiveMonster target;//目标
        private LiveMonster parent;//母体

        private float speed = 2;//像素速度

        public NLPointF Position { get; set; }

        public Missile( int effId, LiveMonster self, LiveMonster mon)
        {
            parent = self;
            target = mon;
            Position = new NLPointF(self.Position.X, self.Position.Y);
        }

        public void Next()
        {
            if (target == null || !target.IsAlive || parent == null || !parent.IsAlive)
            {
                IsFinished = true;
                return;
            }
            
            if (MathTool.GetDistance(target.Position, Position.ToPoint()) < 10)//todo 10是一个估算值
            {
                parent.HitTarget(target.Id);
                IsFinished = true;
                return;
            }

            var posDiff = new NLPointF(target.Position.X - Position.X, target.Position.Y - Position.Y);
            posDiff = posDiff.Normalize()*speed;
            Position = Position + posDiff;

            if (MathTool.GetDistance(target.Position, Position.ToPoint()) < 10)//todo 10是一个估算值
            {
                parent.HitTarget(target.Id);
                IsFinished = true;
            }
        }

        public bool IsFinished { get; set; }

        public void Draw(Graphics g)
        {
           // effect.Draw(g); todo 绘制
        }
    }
}
