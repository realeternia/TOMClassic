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
            //int[] cardToGive = new[] { 53000019 };
            //foreach (var cardId in cardToGive)
            //{
            //    self.HandCards.AddCard(new ActiveCard(cardId, 1, 0));
            //}
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
            bool isLeft = self.IsLeft;
            var rival = self == BattleManager.Instance.PlayerManager.LeftPlayer ? 
                BattleManager.Instance.PlayerManager.RightPlayer : BattleManager.Instance.PlayerManager.LeftPlayer;
            
            self.CardsDesk.SetSelectId(MathTool.GetRandom(self.CardNumber) + 1);
            if (self.SelectCardId != 0)
            {
                ActiveCard card = self.CardsDesk.GetSelectCard();
                if (self.CheckUseCard(card, self, rival) != ErrorConfig.Indexer.OK)
                    return;

                int tar = -1;
                if (card.CardType == CardTypes.Weapon)
                {
                    for (int i = 0; i <BattleManager.Instance.MonsterQueue.Count; i++)
                    {
                        LiveMonster monster =BattleManager.Instance.MonsterQueue[i];
                        if (!monster.IsGhost && monster.IsLeft == isLeft && monster.Weapon == null && monster.Hp > monster.RealMaxHp / 2)
                        {
                            if (!monster.CanAddWeapon())//建筑无法使用武器
                                continue;

                            if (tar == -1 || monster.Avatar.Star >BattleManager.Instance.MonsterQueue[tar].Avatar.Star)
                                tar = i;
                        }
                    }
                    if (tar == -1)
                        return;
                }
                else if (card.CardType == CardTypes.Spell)
                {
                    SpellConfig spellConfig = ConfigData.GetSpellConfig(card.CardId);
                    if (BattleTargetManager.IsSpellUnitTarget(spellConfig.Target))
                    {
                        var targetStar = -1;
                        if (tar >= 0)
                            targetStar = BattleManager.Instance.MonsterQueue[tar].Avatar.Star;
                        for (int i = 0; i <BattleManager.Instance.MonsterQueue.Count; i++)
                        {
                            LiveMonster monster =BattleManager.Instance.MonsterQueue[i];
                            if(monster.IsGhost)
                                continue;
                            if ((monster.IsLeft != isLeft && spellConfig.Target[1] != 'F') || (monster.IsLeft == isLeft && spellConfig.Target[1] != 'E'))
                            {
                                if (tar == -1 || monster.Avatar.Star > targetStar)
                                {
                                    tar = i;
                                    targetStar = monster.Avatar.Star;
                                }
                            }
                        }
                        if (tar == -1)
                            return;
                    }
                }

                if (card.CardType == CardTypes.Monster)
                {
                    Point monPos = GetMonsterPoint(card.CardId, false);
                    self.UseMonster(card, monPos);
                }
                else if (card.CardType == CardTypes.Weapon)
                {
                    var lm =BattleManager.Instance.MonsterQueue[tar];
                    self.UseWeapon(lm, card);
                }
                else if (card.CardType == CardTypes.Spell)
                {
                    SpellConfig spellConfig = ConfigData.GetSpellConfig(card.CardId);
                    Point targetPos = Point.Empty;
                    LiveMonster targetMonster = null;
                    if (BattleTargetManager.IsSpellNullTarget(spellConfig.Target))
                    {
                        targetPos = new Point(isLeft ? MathTool.GetRandom(200, 300) : MathTool.GetRandom(600, 700), MathTool.GetRandom(size * 3 / 10, row * size - size * 3 / 10));
                    }
                    else if (BattleTargetManager.IsSpellUnitTarget(spellConfig.Target))
                    {
                        targetMonster = BattleManager.Instance.MonsterQueue[tar];
                        targetPos = targetMonster.CenterPosition;
                    }

                    self.DoSpell(targetMonster, card, targetPos);
                }
            }
        }

        private Point GetMonsterPoint(int mid, bool isLeft)
        {
            int size = BattleManager.Instance.MemMap.CardSize;
            var sideCell = BattleManager.Instance.MemMap.ColumnCount / 2;
            while (true)
            {
                int x = isLeft ? MathTool.GetRandom(0, sideCell) : MathTool.GetRandom(sideCell + 1, BattleManager.Instance.MemMap.ColumnCount - 1);
                int y = MathTool.GetRandom(0, BattleManager.Instance.MemMap.RowCount);
                x *= size;
                y *= size;
                if (BattleLocationManager.IsPlaceCanSummon(mid,x, y,false))
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