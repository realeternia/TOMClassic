using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemMissile;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Effects;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    internal class MonsterAi
    {
        internal enum AiModes
        {
            Common,
            Tank,
        }
        private readonly LiveMonster self;

        private int lastTarget; //上一个攻击目标
        public AiModes AIMode { get; set; }

        public MonsterAi(LiveMonster mon)
        {
            self = mon;
            AIMode = AiModes.Common;
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
            if (self.IsPrepare)
            {
                if (self.AddAts())
                    self.IsPrepare = false;
                return;
            }

            LiveMonster targetEnemy = null;
            if (lastTarget > 0)
            {//先找上次攻击的目标
                targetEnemy = BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(lastTarget);
                if (targetEnemy != null)
                {
                    if (targetEnemy.IsAlive && CanAttack(targetEnemy))
                    {
                        if (self.AddAts())
                            CheckFight(targetEnemy);
                        return;
                    }
                }
            }

            if (AIMode == AiModes.Tank)
                targetEnemy = GetNearestEnemy(self.IsLeft, self.Position, true); 
            else
                targetEnemy = GetNearestEnemy(self.IsLeft, self.Position, false);//没有就找最近的目标
            if (targetEnemy != null)
            {
                if (self.RealAtk > 0 && self.CanAttack && CanAttack(targetEnemy))
                {
                    if (self.AddAts())
                    {
                        CheckFight(targetEnemy);
                        lastTarget = targetEnemy.Id;
                    }
                }
                else if (self.ReadMov > 0 && self.CanMove) //判定是否需要移动
                {
                    if (self.AddAts())
                    {
                        var moveTarget = targetEnemy;
                        if (self.RealAtk <= 0)
                            moveTarget = BattleManager.Instance.MonsterQueue.GetKingTower(!self.IsLeft);
                        CheckMove(moveTarget);
                    }
                }
            }
        }

        private LiveMonster GetNearestEnemy(bool isLeft, Point mouse, bool aimBuilding)
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

                if (aimBuilding && !pickMon.IsDefence)
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
            if (self.RealRange <= GameConstants.MaxMeleeAtkRange)
            {
                self.HitTarget(nearestEnemy, true); //近战
                BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(self.Arrow), nearestEnemy, false));
            }
            else
            {
                BasicMissileControler controler = new TraceMissileControler(self, nearestEnemy);
                Missile selectMissile = new Missile(self.Arrow, self.Position.X, self.Position.Y, controler, 0, 0);
                BattleManager.Instance.MissileQueue.Add(selectMissile);
            }

            if (self.RealSpd != 0) //会返回一些ats
                self.AddActionRate((self.RealSpd)*GameConstants.SpdToRate/100);
            self.MovRound = 0;
        }

        private void CheckMove(LiveMonster nearestEnemy)
        {
            var moveDis = BattleManager.Instance.MemMap.CardSize;
            Point aimPos; //决定想去的目标点
            bool goX;
            if (nearestEnemy.Position.X != self.Position.X)
            {
                var x = self.Position.X + (nearestEnemy.Position.X > self.Position.X ? moveDis : -moveDis);
                aimPos = new Point(x, self.Position.Y);
                goX = true;
            }
            else
            {
                var y = self.Position.Y + (nearestEnemy.Position.Y > self.Position.Y ? moveDis : -moveDis);
                aimPos = new Point(self.Position.X, y);
                goX = false;
            }

            if (!BattleManager.Instance.MemMap.IsPlaceCanMove(aimPos.X, aimPos.Y))
            {
                if (goX)//绕过不可行走区域
                    aimPos = MonsterPositionHelper.GetAvailPoint(self.Position, "side", self.IsLeft,1);
                else//往前走
                    aimPos= MonsterPositionHelper.GetAvailPoint(self.Position, "come", self.IsLeft,1);
            }

            if (aimPos.X != self.Position.X || aimPos.Y != self.Position.Y)
                BattleLocationManager.SetToPosition(self, aimPos);

            if (self.ReadMov > 10) //会返回一些ats
                self.AddActionRate((float) (self.ReadMov - 10)/self.ReadMov);
            self.MovRound++;
        }

        private bool CanAttack(LiveMonster target)
        {
            var dis = MathTool.GetDistance(target.Position, self.Position);
            return dis <= self.RealRange * BattleManager.Instance.MemMap.CardSize/10;//射程也是十倍的
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
