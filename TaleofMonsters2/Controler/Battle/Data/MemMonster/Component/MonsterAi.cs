using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.DataTent;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Effects;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    internal class MonsterAi
    {
        private LiveMonster monster;

        public MonsterAi(LiveMonster mon)
        {
            monster = mon;
        }

        /// <summary>
        /// 获得一次行动机会
        /// </summary>
        public void CheckAction()
        {
            var nearestEnemy = BattleManager.Instance.MemMap.GetNearestMonster(monster.IsLeft, "E", monster.Position);
            if (nearestEnemy != null)
            {
                if (CanAttack(nearestEnemy))
                {
                    if (monster.Range <= GameConstants.MaxMeleeAtkRange)
                    {
                        monster.HitTarget(nearestEnemy.Id);//近战
                    }
                    else
                    {
                        var effect = EffectBook.GetEffect(monster.Arrow);
                        Missile mi = new Missile(new FlyEffect(effect, monster.Position, nearestEnemy.Position, 99, true));
                        BattleManager.Instance.MissileQueue.Add(mi);
                    }

                    if (monster.RealSpd > 10)//会返回一些ats
                    {
                        monster.AddActionRate((float)(monster.RealSpd - 10) / monster.RealSpd);
                    }
                }
                else
                {
                    var moveDis = BattleManager.Instance.MemMap.CardSize;
                    if (nearestEnemy.Position.X != monster.Position.X)
                    {
                        var x = monster.Position.X + (nearestEnemy.Position.X > monster.Position.X ? moveDis : -moveDis);
                        BattleLocationManager.SetToPosition(monster, new Point(x, monster.Position.Y));
                    }
                    else
                    {
                        var y = monster.Position.Y + (nearestEnemy.Position.Y > monster.Position.Y ? moveDis : -moveDis);
                        BattleLocationManager.SetToPosition(monster, new Point(monster.Position.X, y));
                    }

                    if (monster.Mov>10)//会返回一些ats
                    {
                        monster.AddActionRate((float)(monster.Mov-10)/monster.Mov);
                    }
                }
            }
        }

        private bool CanAttack(LiveMonster target)
        {
            var dis = MathTool.GetDistance(target.Position, monster.Position);
            return dis <= monster.Range * BattleManager.Instance.MemMap.CardSize/10;//射程也是十倍的
        }
    }
}
