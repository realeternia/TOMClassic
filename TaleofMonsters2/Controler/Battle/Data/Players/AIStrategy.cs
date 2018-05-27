using System.Drawing;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal interface IAIStrategy
    {
        void OnInit();
        void AIProc();
        void Discover(IMonster m, int[] cardId, int lv, DiscoverCardActionType type);
    }

    internal class AIStrategyTrivial : IAIStrategy
    {
        public void OnInit()
        {
        }

        public void AIProc()
        {
        }

        public void Discover(IMonster m, int[] cardId, int lv, DiscoverCardActionType type)
        {
        }
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

            int row = BattleManager.Instance.MemMap.RowCount;
            int size = BattleManager.Instance.MemMap.CardSize;
            var rival = self == BattleManager.Instance.PlayerManager.LeftPlayer ? 
                BattleManager.Instance.PlayerManager.RightPlayer : BattleManager.Instance.PlayerManager.LeftPlayer;

            for (int i = 0; i < self.CardNumber; i++)
            {
                self.CardsDesk.SetSelectId(i + 1); //逐个判断是否可以使用卡牌
                if (self.SelectCardId != 0)
                {
                    ActiveCard card = self.CardsDesk.GetSelectCard();
                    if (self.CheckUseCard(card, self, rival) != ErrorConfig.Indexer.OK)
                        continue;

                    if(TryUseCard(card, size, row)) //一回合只使用一张卡
                        break;
                }
            }
       
        }

        private bool TryUseCard(ActiveCard card, int size, int row)
        {
            if (card.CardType == CardTypes.Monster)
            {
                Point monPos = GetSummonPoint(card.CardId, false);
                self.UseMonster(card, monPos);
                return true;
            }
            else if (card.CardType == CardTypes.Weapon)
            {
                LiveMonster target = GetWeaponTarget();
                if (target != null)
                {
                    self.UseWeapon(target, card);
                    return true;
                }
            }
            else if (card.CardType == CardTypes.Spell)
            {
                SpellConfig spellConfig = ConfigData.GetSpellConfig(card.CardId);
                Point targetPos = Point.Empty;
                LiveMonster targetMonster = null;
                var aiGuideType = (AiSpellCastTypes) spellConfig.AIGuide;
                if (aiGuideType == AiSpellCastTypes.Enemy)
                {
                    targetMonster = GetSpellUnitTarget(true);
                    if (targetMonster == null)
                        return false;
                    targetPos = targetMonster.CenterPosition;
                }
                else if (aiGuideType == AiSpellCastTypes.Friend)
                {
                    targetMonster = GetSpellUnitTarget(false);
                    if (targetMonster == null)
                        return false;
                    targetPos = targetMonster.CenterPosition;
                }
                else if (aiGuideType == AiSpellCastTypes.AtWill)
                {
                    targetPos = new Point(self.IsLeft ? MathTool.GetRandom(200, 300) : MathTool.GetRandom(600, 700),
                        MathTool.GetRandom(size * 3 / 10, row * size - size * 3 / 10));
                }

                self.DoSpell(targetMonster, card, targetPos);
                return true;
            }
            return false;
        }

        private LiveMonster GetSpellUnitTarget(bool getEnemy)
        {
            var targetStar = -1;
            int tar = -1;
            if (tar >= 0)
                targetStar = BattleManager.Instance.MonsterQueue[tar].Avatar.Star;
            for (int i = 0; i < BattleManager.Instance.MonsterQueue.Count; i++)
            {
                LiveMonster pickMon = BattleManager.Instance.MonsterQueue[i];
                if (pickMon.IsGhost)
                    continue;
                if ((pickMon.IsLeft != self.IsLeft && getEnemy) ||
                    (pickMon.IsLeft == self.IsLeft && !getEnemy))
                {
                    if (tar == -1 || pickMon.Avatar.Star > targetStar)
                    {
                        tar = i;
                        targetStar = pickMon.Avatar.Star;
                    }
                }
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
                if (!pickMon.IsGhost && pickMon.IsLeft == self.IsLeft && pickMon.Weapon == null)
                {
                    if (pickMon.HpRate < 0.5)
                        continue;
                    if (!pickMon.CanAddWeapon()) //建筑无法使用武器
                        continue;

                    if (tar == -1 || pickMon.Avatar.Star > BattleManager.Instance.MonsterQueue[tar].Avatar.Star)
                        tar = i;
                }
            }
            if (tar == -1)
                return null;

            return BattleManager.Instance.MonsterQueue[tar];
        }

        private Point GetSummonPoint(int mid, bool isLeft)
        {
            int size = BattleManager.Instance.MemMap.CardSize;
            var sideCell = BattleManager.Instance.MemMap.ColumnCount / 2;
            while (true)
            {
                int x = isLeft ? MathTool.GetRandom(0, sideCell) : MathTool.GetRandom(sideCell + 1, BattleManager.Instance.MemMap.ColumnCount - 1);
                int y = MathTool.GetRandom(0, BattleManager.Instance.MemMap.RowCount);
                x *= size;
                y *= size;
                if (BattleLocationManager.IsPlaceCanSummon(mid, x, y, false))
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