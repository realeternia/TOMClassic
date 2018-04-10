using System.Drawing;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;

namespace TaleofMonsters.Datas.Effects.Facts
{
    /// <summary>
    /// 绑定怪物单位的特效，一般在有战斗中使用
    /// </summary>
    internal class MonsterBindEffect : BaseEffect
    {
        private LiveMonster mon;
        private Point position;

        public MonsterBindEffect(Effect effect, LiveMonster mon, bool isMute)
            :base(effect, isMute)
        {
            this.mon = mon;
        }

        public MonsterBindEffect(Effect effect, Point p, bool isMute)
            : base(effect, isMute)
        {
            mon = null;
            position = p;
        }

        public override bool Next()
        {
            if (base.Next())
            {
                frameId++;
                if (Repeat && !mon.IsAlive)
                {
                    IsFinished = IsFinished == RunState.Run ? RunState.Finished : RunState.Zombie;
                    frameId = effect.Frames.Length - 1;
                }
                else if (frameId >= effect.Frames.Length)
                {
                    if (Repeat && mon.IsAlive)
                    {
                        frameId = 0;
                    }
                    else
                    {
                        IsFinished = IsFinished == RunState.Run ? RunState.Finished : RunState.Zombie;
                        frameId = effect.Frames.Length - 1;
                    }
                }
            }

            return true;
        }

        public override void Draw(Graphics g)
        {
            if (frameId >= 0 && frameId < effect.Frames.Length)
            {
                int size = BattleManager.Instance.MemMap.CardSize;
                int x = ((mon == null) ? position.X - size / 2 : mon.Position.X);
                int y = ((mon == null) ? position.Y - size / 2 : mon.Position.Y);
                effect.Frames[frameId].Draw(g, x, y, size, size);
            }
        }
    }
}
