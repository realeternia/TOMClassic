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

        public void ClearTarget()
        {
            lastTarget = 0;
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
                            CheckFight(targetEnemy);
                        return;
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
                else if (monster.ReadMov > 0 && monster.CanMove) //判定是否需要移动
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

            foreach (var pickMon in BattleManager.Instance.MonsterQueue.Enumerator)
            {
                if (pickMon.IsGhost)
                    continue;

                if (isLeft == pickMon.Owner.IsLeft)
                    continue;

                var tpDis = MathTool.GetDistance(pickMon.Position, mouse);
                if (tpDis / BattleManager.Instance.MemMap.CardSize * 10 > pickMon.RealRange 
                    && (isLeft && pickMon.Position.X < mouse.X || !isLeft && pickMon.Position.X > mouse.X)) //不管身后的敌人
                    continue;

                var targetRate = GetMonsterRate(pickMon, tpDis / BattleManager.Instance.MemMap.CardSize * 10);
                if (targetRate == 0)
                    continue;

                if (tpDis < dis || tpDis == dis && targetRate > rate)
                {
                    dis = tpDis;
                    target = pickMon;
                    rate = targetRate;
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
                Missile selectMissile = new Missile(monster.Arrow, monster.Position.X, monster.Position.Y, controler);
                BattleManager.Instance.MissileQueue.Add(selectMissile);
            }

            if (monster.RealSpd != 0) //会返回一些ats
                monster.AddActionRate((monster.RealSpd)*GameConstants.SpdToRate/100);
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

            if (!BattleManager.Instance.MemMap.IsPlaceCanMove(aimPos.X, aimPos.Y))
            {
                if (goX)//绕过不可行走区域
                    aimPos = MonsterPositionHelper.GetAvailPoint(monster.Position, "side", monster.IsLeft,1);
                else//往前走
                    aimPos= MonsterPositionHelper.GetAvailPoint(monster.Position, "come", monster.IsLeft,1);
            }

            if (aimPos.X != monster.Position.X || aimPos.Y != monster.Position.Y)
                BattleLocationManager.SetToPosition(monster, aimPos);

            if (monster.ReadMov > 10) //会返回一些ats
                monster.AddActionRate((float) (monster.ReadMov - 10)/monster.ReadMov);
            monster.MovRound++;
        }

        private bool CanAttack(LiveMonster target)
        {
            var dis = MathTool.GetDistance(target.Position, monster.Position);
            return dis <= monster.RealRange * BattleManager.Instance.MemMap.CardSize/10;//射程也是十倍的
        }

        private int GetMonsterRate(LiveMonster mon, int dis)
        {
            if (MonsterBook.HasTag(mon.CardId, "hide"))
            {
                if (dis > 10)
                    return 0;
            }

            int score = 10000-mon.Hp;
            score += mon.RealRange*10;//优先打远程单位
            if (MonsterBook.HasTag(mon.CardId, "taunt"))
                score += 100000;
            return score;
        }
    }
}
