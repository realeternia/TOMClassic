using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemMissile;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Effects;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    internal class MonsterAi
    {
        private readonly LiveMonster monster;

        public MonsterAi(LiveMonster mon)
        {
            monster = mon;
        }

        /// <summary>
        /// 获得一次行动机会
        /// </summary>
        public void CheckAction()
        {
            var nearestEnemy = BattleManager.Instance.MemMap.GetNearestEnemy(monster.IsLeft, monster.Position);
            if (nearestEnemy != null)
            {
                if (monster.CanAttack && CanAttack(nearestEnemy))
                {
                    if (monster.AddAts())
                    {
                        CheckFight(nearestEnemy);
                    }
                }
                else if (monster.ReadMov>0 && monster.CanMove)//判定是否需要移动
                {
                    if (monster.AddAts())
                    {
                        CheckMove(nearestEnemy);
                    }
                }
            }
        }

        private void CheckFight(LiveMonster nearestEnemy)
        {
            if (monster.RealRange <= GameConstants.MaxMeleeAtkRange)
            {
                monster.HitTarget(nearestEnemy); //近战
                BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(monster.Arrow), nearestEnemy, false));
            }
            else
            {
                BasicMissileControler controler = new TraceMissileControler(monster, nearestEnemy);
                Missile mi = new Missile(monster.Arrow, monster.Position.X, monster.Position.Y, controler);
                BattleManager.Instance.MissileQueue.Add(mi);
            }

            if (monster.RealSpd != 0) //会返回一些ats
            {
                monster.AddActionRate((float) (monster.RealSpd)*GameConstants.SpdToRate/100);
            }
        }

        private void CheckMove(LiveMonster nearestEnemy)
        {
            var moveDis = BattleManager.Instance.MemMap.CardSize;
            Point aimPos; //决定想去的目标点
            bool goX;
            if (nearestEnemy.Position.X != monster.Position.X)
            {
                var x = monster.Position.X + (nearestEnemy.Position.X > monster.Position.X ? moveDis : -moveDis);
                aimPos = new Point(x, monster.Position.Y);
                goX = true;
            }
            else
            {
                var y = monster.Position.Y + (nearestEnemy.Position.Y > monster.Position.Y ? moveDis : -moveDis);
                aimPos = new Point(monster.Position.X, y);
                goX = false;
            }

            if (!BattleLocationManager.IsPlaceCanMove(aimPos.X, aimPos.Y))
            {
                if (goX)//绕过不可行走区域
                    aimPos = BattleLocationManager.GetMonsterNearPoint(monster.Position, "side", !monster.IsLeft);
                else//往前走
                    aimPos= BattleLocationManager.GetMonsterNearPoint(monster.Position, "come", !monster.IsLeft);
            }
            else
            {
                var targetMonster = BattleLocationManager.GetPlaceMonster(aimPos.X, aimPos.Y);//找到目标点的怪
                if (targetMonster != null && targetMonster.IsAlive) //如果前方是一个单位，就绕行
                {
                    if (goX)
                        aimPos = BattleLocationManager.GetMonsterNearPoint(monster.Position, "side", !monster.IsLeft);
                    else
                        aimPos = BattleLocationManager.GetMonsterNearPoint(monster.Position, "come", !monster.IsLeft);
                }
            }

            if (aimPos.X >=0&& aimPos.Y >=0)
            {
                BattleLocationManager.SetToPosition(monster, aimPos);
            }

            if (monster.ReadMov > 10) //会返回一些ats
            {
                monster.AddActionRate((float) (monster.ReadMov - 10)/monster.ReadMov);
            }
        }

        private bool CanAttack(LiveMonster target)
        {
            var dis = MathTool.GetDistance(target.Position, monster.Position);
            return dis <= monster.RealRange * BattleManager.Instance.MemMap.CardSize/10;//射程也是十倍的
        }
    }
}
