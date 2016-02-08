using System;
using System.Drawing;
using NarlonLib.Core;
using NarlonLib.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;

namespace TaleofMonsters.Controler.Battle.Data.MemMissile
{
    /// <summary>
    /// 锁定或者非锁定的投射物
    /// </summary>
    internal class Missile
    {
        private Image sourceImg;
        private Image effectImg;
        private LiveMonster target;//目标
        private LiveMonster parent;//母体

        private float speed = 2;//像素速度

        public NLPointF Position { get; set; }

        public Missile(string effName, LiveMonster self, LiveMonster mon)
        {
            parent = self;
            target = mon;
            Position = new NLPointF(self.Position.X, self.Position.Y);
            sourceImg = MissileBook.GetImage(effName);
         //   effectImg = sourceImg;
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
            posDiff = posDiff.Normalize() * speed;
            Position = Position + posDiff;
            var angle = Math.Atan(-posDiff.Y / posDiff.X) / Math.PI * 180;

            effectImg = DrawTool.Rotate(sourceImg, posDiff.X >= 0 ? (int)angle : (int)angle + 180);

            if (MathTool.GetDistance(target.Position, Position.ToPoint()) < 10)//todo 10是一个估算值
            {
                parent.HitTarget(target.Id);
                IsFinished = true;
            }
        }

        public bool IsFinished { get; set; }

        public void Draw(Graphics g)
        {
            if (effectImg != null)
            {
                int size = BattleManager.Instance.MemMap.CardSize;
                int ewid = effectImg.Width * size / 100;
                int eheg = effectImg.Height * size / 100;

                var x = Position.X - (float)ewid / 2 + (float)size / 2;//+size/2是为了平移到中心位置
                var y = Position.Y - (float)eheg / 2 + (float)size / 2;

                g.DrawImage(effectImg, x, y, ewid, eheg);
            }
        }

    }
}
