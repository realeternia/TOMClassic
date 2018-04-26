using System;
using System.Collections.Generic;
using NarlonLib.Math;
using NarlonLib.Tools;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Decks;

namespace TaleofMonsters.Controler.Battle.Data.MemCard
{
    /// <summary>
    /// 牌库的牌
    /// </summary>
    internal class CardOffBundle
    {
        private int index;
        private List<ActiveCard> waitList; //等待中
        private List<int> graveList; //坟场

        public int LeftCount
        {
            get { return Math.Max(0, waitList.Count - index); }
        }

        private CardOffBundle()
        {
            index = 0;
            waitList = new List<ActiveCard>();
            graveList = new List<int>();
        }

        public CardOffBundle(DeckCard[] itsCards)
        {
            waitList = new List<ActiveCard>();
            for (int i = 0; i < itsCards.Length; i++)
                waitList.Add(new ActiveCard(itsCards[i].CardId, itsCards[i].Level));
            ArraysUtils.RandomShuffle(waitList);
            graveList = new List<int>();
            index = 0;
        }

        public CardOffBundle GetCopy()
        {
            CardOffBundle cloneDeck = new CardOffBundle();
            foreach (var checkCard in waitList)
                cloneDeck.waitList.Add(new ActiveCard(checkCard.CardId, checkCard.Level));
            return cloneDeck;
        }

        public ActiveCard GetNextCard()
        {
            if (waitList.Count == 0)
                return ActiveCard.NoneCard;

            int rt = index;
            if (rt >= waitList.Count)
                return ActiveCard.NoneCard;
            index++;

            if (CardConfigManager.GetCardConfig(waitList[rt].CardId).Id == 0)
            {//卡牌配置可能已经过期，用下一个卡
                NarlonLib.Log.NLog.Warn("GetNextCard card is outofdate id={0}", waitList[rt].CardId);
                return GetNextCard();
            }

            return waitList[rt];
        }

        public ActiveCard ReplaceCard(ActiveCard card)
        {
            if (LeftCount <= 0)
                return ActiveCard.NoneCard;

            var targetIndex = index + MathTool.GetRandom(LeftCount);
            var target = waitList[targetIndex];
            waitList[targetIndex] = card;
            return target;
        }

        public void AddGrave(ActiveCard card)
        {
            graveList.Add(card.CardId);
        }

        public int GetRandomMonsterFromGrave()
        {
            var graveMonsters = new List<int>();
            foreach (var cardId in graveList)
            {
                if(CardConfigManager.GetCardConfig(cardId).Type == CardTypes.Monster)
                    graveMonsters.Add(cardId);
            }
            if(graveMonsters.Count == 0)
                return 0;

            var targetMon = graveMonsters[MathTool.GetRandom(graveMonsters.Count)];
            graveList.Remove(targetMon);
            return targetMon;
        }

        public void CardLevelUp(int n, int type)
        {
            foreach (var pickCard in waitList)
            {
                if (type != 0 && ConfigIdManager.GetCardType(pickCard.CardId) != (CardTypes)type)
                    continue;

                pickCard.ChangeLevel(n);
            }
        }
    }
}
