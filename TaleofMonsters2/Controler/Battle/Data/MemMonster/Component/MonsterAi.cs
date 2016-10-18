using System;
using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemMissile;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Effects;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    internal class MonsterAi
    {
        private readonly LiveMonster monster;

        private int lastTarget; //上一个攻击目标

        public MonsterAi(LiveMonster mon)
        {
            monster = mon;
        }

        /// <summary>
        /// 获得一次行动机会
        /// </summary>
        public void CheckAction()
        {
            LiveMonster targetEnemy = null;
            if (lastTarget > 0)
            {//先找上次攻击的目标
                targetEnemy = BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(lastTarget);
                if (targetEnemy != null)
                {
                    if (targetEnemy.IsAlive && CanAttack(targetEnemy))
                    {
                        if (monster.AddAts())
                        {
                            CheckFight(targetEnemy);
                            return;
                        }
                    }
                }
            }

            targetEnemy = GetNearestEnemy(monster.IsLeft, monster.Position);//没有就找最近的目标
            if (targetEnemy != null)
            {
                if (monster.RealAtk > 0 && monster.CanAttack && CanAttack(targetEnemy))
                {
                    if (monster.AddAts())
                    {
                        CheckFight(targetEnemy);
                        lastTarget = targetEnemy.Id;
                    }
                }
                else if (monster.ReadMov>0 && monster.CanMove)//判定是否需要移动
                {
                    if (monster.AddAts())
                    {
                        var moveTarget = targetEnemy;
                        if (monster.RealAtk <= 0)
                            moveTarget = BattleManager.Instance.MonsterQueue.GetKingTower(!monster.IsLeft);
                        CheckMove(moveTarget);
                    }
                }
            }
        }

        private LiveMonster GetNearestEnemy(bool isLeft, Point mouse)
        {
            LiveMonster target = null;
            int dis = int.MaxValue;
            int rate = 0;

            foreach (var mon in BattleManager.Instance.MonsterQueue.Enumerator)
            {
                if (mon.IsGhost)
                    continue;

                if (isLeft != mon.Owner.IsLeft)
                {
                    var tpDis = MathTool.GetDistance(mon.Position, mouse);
                    if (tpDis / BattleManager.Instance.MemMap.CardSize * 10 > mon.RealRange && (isLeft && mon.Position.X < mouse.X || !isLeft && mon.Position.X > mouse.X)) //不管身后的敌人
                        continue;

                    var targetRate = GetMonsterRate(mon);
                    if (tpDis < dis || tpDis == dis && targetRate > rate)
                    {
                        dis = tpDis;
                        target = mon;
                        rate = targetRate;
                    }
                }
            }
            return target;
        }

        private void CheckFight(LiveMonster nearestEnemy)
        {
            if (monster.RealRange <= GameConstants.MaxMeleeAtkRange)
            {
                monster.HitTarget(nearestEnemy, true); //近战
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
            monster.MovRound = 0;
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

            if (aimPos.X >=0&& aimPos.Y >=0)
            {
                BattleLocationManager.SetToPosition(monster, aimPos);
            }

            if (monster.ReadMov > 10) //会返回一些ats
            {
                monster.AddActionRate((float) (monster.ReadMov - 10)/monster.ReadMov);
            }
            monster.MovRound++;
        }

        private bool CanAttack(LiveMonster target)
        {
            var dis = MathTool.GetDistance(target.Position, monster.Position);
            return dis <= monster.RealRange * BattleManager.Instance.MemMap.CardSize/10;//射程也是十倍的
        }

        private int GetMonsterRate(LiveMonster mon)
        {
            int score = 10000-mon.Hp;
            score += mon.RealRange*10;//优先打远程单位
            if (MonsterBook.HasTag(mon.CardId, "taunt"))
            {
                score += 10000;
            }
            else if (MonsterBook.HasTag(mon.CardId, "hide"))
            {
                score -= 5000;
            }
            return score;
        }
    }
}
