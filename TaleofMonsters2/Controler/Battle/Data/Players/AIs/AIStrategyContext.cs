using System.Drawing;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards.Monsters;

namespace TaleofMonsters.Controler.Battle.Data.Players.AIs
{
    internal class AIStrategyContext
    {
        public Player Self { get; private set; }
        private IAIState nowState;

        public AIStrategyContext(Player player)
        {
            Self = player;
        }

        public void ChangeState(AIStates s)
        {
            if (nowState != null)
                nowState.OnExit();
            switch (s)
            {
                case AIStates.Wander: nowState = new AIStateWander(this); break;
                case AIStates.Attack: nowState = new AIStateAttack(this); break;
                case AIStates.Defend: nowState = new AIStateDefend(this); break;
            }
            if (nowState != null)
                nowState.OnEnter();
        }

        public void OnInit()
        {
            var cds = Self.CardsDesk.GetAllCard();
            for (int i = 0; i < cds.Length; i++)
            {
                var card = cds[i];
                if (CardConfigManager.GetCardConfig(card.CardId).Star > 3) //把3费以上卡都换掉
                    Self.HandCards.RedrawCardAt(i + 1);
            }
            ChangeState(AIStates.Wander);
#if DEBUG
            int[] cardToGive = new[] { 53000139 };
            foreach (var cardId in cardToGive)
            {
                Self.HandCards.AddCard(new ActiveCard(cardId, 1));
            }
#endif
        }

        public void OnTimePast()
        {
            nowState.OnTimePast(0.2f);
        }

        public void OnTowerHited(double towerHpRate)
        {
            nowState.OnTowerHited(towerHpRate);
        }

        /// <summary>
        /// 尝试使用所有的手牌
        /// </summary>
        public void TryAllHandCards(bool monsterOnly, float threat, ref int totalMpNeed)
        {
            for (int i = 0; i < Self.CardNumber; i++)
            {
                Self.CardsDesk.SetSelectId(i + 1); //逐个判断是否可以使用卡牌
                if (Self.SelectCardId == 0)
                    continue;

                ActiveCard card = Self.CardsDesk.GetSelectCard();
                if(monsterOnly && card.CardType != CardTypes.Monster)
                    continue;

                if (Self.CheckUseCard(card, Self, Self.Rival as Player) != ErrorConfig.Indexer.OK)
                    continue;

                if (TryUseCard(card, threat)) //一回合只使用一张卡
                    break;

                totalMpNeed += card.Mp;
            }
        }

        /// <summary>
        /// 尝试使用英雄技能
        /// </summary>
        public void TryHeroPower(float threat)
        {
            LevelExpConfig levelConfig = ConfigData.GetLevelExpConfig(Self.Level);

            for (int i = 0; i < Self.HeroSkillList.Count; i++)
            {
                var skillId = Self.HeroSkillList[i];
                HeroPowerConfig heroSkillConfig = ConfigData.GetHeroPowerConfig(skillId);
                ActiveCard card = new ActiveCard(heroSkillConfig.CardId, (byte)levelConfig.TowerLevel);
                card.Mp = ConfigData.GetSpellConfig(heroSkillConfig.CardId).Cost;
                if (Self.CheckUseCard(card, Self, Self.Rival as Player) != ErrorConfig.Indexer.OK)
                    continue;

                if (TryUseCard(card, threat)) //一回合只使用一个英雄技能
                    break;
            }
        }

        private bool TryUseCard(ActiveCard selectCard, float threat)
        {
            if (selectCard.CardType == CardTypes.Monster)
            {
                var canRush = MonsterBook.HasTag(selectCard.CardId, "rush");
                Point monPos = GetSummonPoint(false, canRush);
                Self.UseMonster(selectCard, monPos);
                return true;
            }
            else if (selectCard.CardType == CardTypes.Weapon)
            {
                LiveMonster target = GetWeaponTarget();
                if (target != null)
                {
                    Self.UseWeapon(target, selectCard, target.Position);
                    return true;
                }
            }
            else if (selectCard.CardType == CardTypes.Spell)
            {
                SpellConfig spellConfig = ConfigData.GetSpellConfig(selectCard.CardId);
                Point targetPos = Point.Empty;
                LiveMonster targetMonster = null;
                var aiGuideType = (AiSpellCastTypes) spellConfig.AIGuide;
                if (aiGuideType == AiSpellCastTypes.CellBlank)
                {
                    targetPos = GetSummonPoint(false, true);
                }
                else if (aiGuideType >= AiSpellCastTypes.AtWill && aiGuideType < AiSpellCastTypes.EnemySingle)
                {
                    switch (aiGuideType)
                    {
                        case AiSpellCastTypes.CardLess:
                            if (Self.CardNumber > 3) return false;
                            break;
                        case AiSpellCastTypes.CardMore:
                            if (Self.CardNumber < 4) return false;
                            break;
                        case AiSpellCastTypes.CardRivalMore:
                            if (Self.Rival.CardNumber < 2) return false;
                            break;
                        case AiSpellCastTypes.MonsterAdv:
                            if (threat > -100) return false;
                            break;
                        case AiSpellCastTypes.MonsterDisadv:
                            if (threat < 100) return false;
                            break;
                    }
                    targetPos = BattleManager.Instance.MemMap.GetRandomPoint(Self.IsLeft, true, false);
                }
                else
                {
                    switch (aiGuideType)
                    {
                        case AiSpellCastTypes.EnemySingle:
                            targetMonster = GetSpellUnitTarget(true, false);
                            break;
                        case AiSpellCastTypes.EnemySingleWeak:
                            targetMonster = GetSpellUnitTarget(true, true);
                            break;
                        case AiSpellCastTypes.EnemyTwo:
                            targetMonster = GetSpellUnitTargetCount(selectCard.CardId, 2, true, false);
                            break;
                        case AiSpellCastTypes.EnemyTomb:
                            targetMonster = GetSpellUnitTargetTomb(true);
                            break;
                        case AiSpellCastTypes.FriendSingle:
                            targetMonster = GetSpellUnitTarget(false, false);
                            break;
                        case AiSpellCastTypes.FriendWeak:
                            targetMonster = GetSpellUnitTarget(false, true);
                            break;
                        case AiSpellCastTypes.FriendTwo:
                            targetMonster = GetSpellUnitTargetCount(selectCard.CardId, 2, false, false);
                            break;
                        case AiSpellCastTypes.FriendTwoCure:
                            targetMonster = GetSpellUnitTargetCount(selectCard.CardId, 2, false, true);
                            break;
                        case AiSpellCastTypes.FriendTomb:
                            targetMonster = GetSpellUnitTargetTomb(false);
                            break;
                    }
                    if (targetMonster == null)//剩下的都是单位目标了，取到空就说明没有合适目标
                        return false;
                }
                if (!Self.CanSpell(targetMonster, selectCard))
                    return false;
                if(targetMonster != null)
                    targetPos = targetMonster.CenterPosition;
                Self.DoSpell(targetMonster, selectCard, targetPos);
                return true;
            }
            return false;
        }

        private LiveMonster GetSpellUnitTarget(bool getEnemy, bool getWeak)
        {
            int tar = -1;
            for (int i = 0; i < BattleManager.Instance.MonsterQueue.Count; i++)
            {
                LiveMonster pickMon = BattleManager.Instance.MonsterQueue[i];
                if (pickMon.IsGhost || pickMon is TowerMonster) //不打塔
                    continue;
                if ((pickMon.IsLeft != Self.IsLeft && !getEnemy) || (pickMon.IsLeft == Self.IsLeft && getEnemy))
                    continue;
                if (pickMon.HpRate < 40 && !getWeak) //快死的不要
                    continue;
                if (pickMon.HpRate > 40 && getWeak) 
                    continue;
                if (tar == -1 || pickMon.Avatar.Star > BattleManager.Instance.MonsterQueue[tar].Avatar.Star)
                    tar = i;
            }
            if (tar == -1)
                return null;
            return BattleManager.Instance.MonsterQueue[tar];
        }

        /// <summary>
        /// 找到>=count个目标
        /// </summary>
        private LiveMonster GetSpellUnitTargetCount(int spellId, int count, bool getEnemy, bool needHpLow)
        {
            var spellConfig = ConfigData.GetSpellConfig(spellId);
            for (int i = 0; i < BattleManager.Instance.MonsterQueue.Count; i++)
            {
                LiveMonster pickMon = BattleManager.Instance.MonsterQueue[i];
                if (pickMon is TowerMonster)
                    continue;
                if ((pickMon.IsLeft != Self.IsLeft && !getEnemy) || (pickMon.IsLeft == Self.IsLeft && getEnemy))
                    continue;
                if (pickMon.HpRate > 80 && needHpLow)
                    continue;
                var count1 = pickMon.Map.GetRangeMonster(Self.IsLeft, spellConfig.Target.Substring(1, 1),
                    spellConfig.Target.Substring(2, 1), spellConfig.Range, pickMon.CenterPosition).Count;
                if (count1 >= count)
                    return BattleManager.Instance.MonsterQueue[i];
            }
            return null;
        }

        private LiveMonster GetSpellUnitTargetTomb(bool getEnemy)
        {
            int tar = -1;
            for (int i = 0; i < BattleManager.Instance.MonsterQueue.Count; i++)
            {
                LiveMonster pickMon = BattleManager.Instance.MonsterQueue[i];
                if (!pickMon.IsGhost)
                    continue;
                if ((pickMon.IsLeft != Self.IsLeft && !getEnemy) || (pickMon.IsLeft == Self.IsLeft && getEnemy))
                    continue;
                if (tar == -1 || pickMon.Avatar.Star > BattleManager.Instance.MonsterQueue[tar].Avatar.Star)
                    tar = i;
            }
            if (tar == -1)
                return null;
            return BattleManager.Instance.MonsterQueue[tar];
        }

        private LiveMonster GetWeaponTarget()
        {
            int tar = -1;
            for (int i = 0; i < BattleManager.Instance.MonsterQueue.Count; i++)
            {
                LiveMonster pickMon = BattleManager.Instance.MonsterQueue[i];
                if (pickMon.IsGhost || pickMon.IsLeft != Self.IsLeft)
                    continue;
                if (pickMon.Weapon != null) //重复装备太奢侈了
                    continue;
                if (pickMon.HpRate < 50)
                    continue;
                if (!pickMon.CanAddWeapon()) //建筑无法使用武器
                    continue;

                if (tar == -1 || pickMon.Avatar.Star > BattleManager.Instance.MonsterQueue[tar].Avatar.Star)
                    tar = i;
            }
            if (tar == -1)
                return null;

            return BattleManager.Instance.MonsterQueue[tar];
        }

        private Point GetSummonPoint(bool isLeft, bool canRush)
        {
            int size = BattleManager.Instance.MemMap.CardSize;
            var sideCell = BattleManager.Instance.MemMap.ColumnCount / 2;
            while (true)
            {
                int x = isLeft ? MathTool.GetRandom(0, sideCell) : MathTool.GetRandom(sideCell + 1, BattleManager.Instance.MemMap.ColumnCount - 1);
                int y = MathTool.GetRandom(0, BattleManager.Instance.MemMap.RowCount);
                x *= size;
                y *= size;
                if (BattleLocationManager.IsPlaceCanSummon(x, y, false, canRush))
                    return new Point(x, y);
            }
        }

        /// <summary>
        /// 计算当前的威胁值，-100认为有优势，100认为有劣势
        /// </summary>
        public float GetThreat(bool isLeft)
        {
            var widthStage = BattleManager.Instance.MemMap.StageWidth;
            float threat = 0;
            for (int i = 0; i < BattleManager.Instance.MonsterQueue.Count; i++)
            {
                LiveMonster pickMon = BattleManager.Instance.MonsterQueue[i];
                if (pickMon.IsGhost || pickMon.IsDefence)
                    continue;

                var pickThreat = (float) (pickMon.Atk* (1 + (pickMon.RealDef + pickMon.RealSpd + pickMon.RealHit +
                    pickMon.RealDHit + pickMon.RealCrt)* 0.05));
                var cellMoved = pickMon.IsLeft ? pickMon.Position.X : widthStage - pickMon.Position.X;
                if (isLeft == pickMon.IsLeft)
                    pickThreat = -pickThreat * (1 + 2 * (widthStage - (float)cellMoved) / widthStage); //1-3倍距离产生的压力
                else
                    pickThreat = pickThreat * (1 + 2 * (float)cellMoved / widthStage); //1-3倍距离产生的压力
                pickThreat = pickThreat*(float)pickMon.HpRate/100;
                threat += pickThreat;
            }
            return threat;
        }

        public AIStates GetState()
        {
            if(nowState == null)
                return AIStates.None;
            return nowState.State;
        }

        public void Discover(IMonster m, int[] cardId, int lv, DiscoverCardActionType type)
        {
            var targetCardId = cardId[MathTool.GetRandom(cardId.Length)]; //随机拿一张
            Self.AddDiscoverCard(m, targetCardId, lv, type);
        }
    }
}