using System.Drawing;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards.Monsters;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal interface IAIStrategy
    {
        void OnInit();
        void AIProc();
        void Discover(IMonster m, int[] cardId, int lv, DiscoverCardActionType type);
    }

    internal class AIStrategy : IAIStrategy
    {
        private Player self;

        public AIStrategy(Player player)
        {
            self = player;
        }

        public void OnInit()
        {
            var cds = self.CardsDesk.GetAllCard();
            for (int i = 0; i < cds.Length; i++)
            {
                var card = cds[i];
                if (CardConfigManager.GetCardConfig(card.CardId).Star > 3) //把3费以上卡都换掉
                    self.HandCards.RedrawCardAt(i + 1);
            }

#if DEBUG
            int[] cardToGive = new[] { 53000139 };
            foreach (var cardId in cardToGive)
            {
                self.HandCards.AddCard(new ActiveCard(cardId, 1));
            }
#endif
        }

        public void AIProc()
        {            
            if (self.CardNumber <= 0)
                return;

            if (MathTool.GetRandom(4) != 0)
                return;

            var rival = self.Rival as Player;
            int totalMpNeed = 0;
            for (int i = 0; i < self.CardNumber; i++)
            {
                self.CardsDesk.SetSelectId(i + 1); //逐个判断是否可以使用卡牌
                if (self.SelectCardId != 0)
                {
                    ActiveCard card = self.CardsDesk.GetSelectCard();
                    if (self.CheckUseCard(card, self, rival) != ErrorConfig.Indexer.OK)
                        continue;

                    if(TryUseCard(card)) //一回合只使用一张卡
                        break;

                    totalMpNeed += card.Mp;
                }
            }

            if (self.Mp > totalMpNeed*0.5)
            {//尝试使用英雄技能
                LevelExpConfig levelConfig = ConfigData.GetLevelExpConfig(self.Level);

                for (int i = 0; i < self.HeroSkillList.Count; i++)
                {
                    var skillId = self.HeroSkillList[i];
                    HeroPowerConfig heroSkillConfig = ConfigData.GetHeroPowerConfig(skillId);
                    ActiveCard card = new ActiveCard(heroSkillConfig.CardId, (byte)levelConfig.TowerLevel);
                    card.Mp = ConfigData.GetSpellConfig(heroSkillConfig.CardId).Cost;
                    if (self.CheckUseCard(card, self, rival) != ErrorConfig.Indexer.OK)
                        continue;

                    if (TryUseCard(card)) //一回合只使用一个英雄技能
                        break;
                }
            }
        }

        private bool TryUseCard(ActiveCard selectCard)
        {
            if (selectCard.CardType == CardTypes.Monster)
            {
                var canRush = MonsterBook.HasTag(selectCard.CardId, "rush");
                Point monPos = GetSummonPoint(false, canRush);
                self.UseMonster(selectCard, monPos);
                return true;
            }
            else if (selectCard.CardType == CardTypes.Weapon)
            {
                LiveMonster target = GetWeaponTarget();
                if (target != null)
                {
                    self.UseWeapon(target, selectCard);
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
                            if (self.CardNumber > 3) return false;
                            break;
                        case AiSpellCastTypes.CardMore:
                            if (self.CardNumber < 4) return false;
                            break;
                        case AiSpellCastTypes.CardRivalMore:
                            if (self.Rival.CardNumber < 2) return false;
                            break;
                    }
                    targetPos = BattleManager.Instance.MemMap.GetRandomPoint(self.IsLeft, true, false);
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
                if (!self.CanSpell(targetMonster, selectCard))
                    return false;
                if(targetMonster != null)
                    targetPos = targetMonster.CenterPosition;
                self.DoSpell(targetMonster, selectCard, targetPos);
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
                if ((pickMon.IsLeft != self.IsLeft && !getEnemy) || (pickMon.IsLeft == self.IsLeft && getEnemy))
                    continue;
                if (pickMon.HpRate < 0.4 && !getWeak) //快死的不要
                    continue;
                if (pickMon.HpRate > 0.4 && getWeak) 
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
                if ((pickMon.IsLeft != self.IsLeft && !getEnemy) || (pickMon.IsLeft == self.IsLeft && getEnemy))
                    continue;
                if (pickMon.HpRate > 0.8 && needHpLow)
                    continue;
                var count1 = pickMon.Map.GetRangeMonster(self.IsLeft, spellConfig.Target.Substring(1, 1),
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
                if ((pickMon.IsLeft != self.IsLeft && !getEnemy) || (pickMon.IsLeft == self.IsLeft && getEnemy))
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
                if (pickMon.IsGhost || pickMon.IsLeft != self.IsLeft)
                    continue;
                if (pickMon.Weapon != null) //重复装备太奢侈了
                    continue;
                if (pickMon.HpRate < 0.5)
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

        public void Discover(IMonster m, int[] cardId, int lv, DiscoverCardActionType type)
        {
            var targetCardId = cardId[MathTool.GetRandom(cardId.Length)]; //随机拿一张
            self.AddDiscoverCard(m, targetCardId, lv, type);
        }
    }
}