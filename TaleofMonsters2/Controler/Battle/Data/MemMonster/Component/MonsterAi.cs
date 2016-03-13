using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemMissile;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;

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
                if (monster.CanAttack && CanAttack(nearestEnemy))
                {
                    if (monster.AddAts())
                    {
                        if (monster.RealRange <= GameConstants.MaxMeleeAtkRange)
                        {
                            monster.HitTarget(nearestEnemy.Id);//近战
                        }
                        else
                        {
                            Missile mi = new Missile(monster.Arrow, monster, nearestEnemy);//todo
                            BattleManager.Instance.MissileQueue.Add(mi);
                        }

                        if (monster.RealSpd != 0)//会返回一些ats
                        {
                            monster.AddActionRate((float)(monster.RealSpd) * GameConstants.SpdToRate / 100);
                        }
                    }
                }
                else if (monster.ReadMov>0)//判定是否需要移到
                {
                    if (monster.AddAts())
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

                        if (monster.ReadMov > 10)//会返回一些ats
                        {
                            monster.AddActionRate((float)(monster.ReadMov - 10) / monster.ReadMov);
                        }
                    }
                }
            }
        }

        private bool CanAttack(LiveMonster target)
        {
            var dis = MathTool.GetDistance(target.Position, monster.Position);
            return dis <= monster.RealRange * BattleManager.Instance.MemMap.CardSize/10;//射程也是十倍的
        }
    }
}
