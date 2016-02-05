using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Tool;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster
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
                    monster.HitTarget(nearestEnemy.Id);
                }
                else
                {
                    var moveDis = monster.Mov*BattleManager.Instance.MemMap.CardSize;
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
                }
            }
        }

        private bool CanAttack(LiveMonster target)
        {
            var dis = MathTool.GetDistance(target.Position, monster.Position);
            return dis <= monster.Range * BattleManager.Instance.MemMap.CardSize;
        }
    }
}
