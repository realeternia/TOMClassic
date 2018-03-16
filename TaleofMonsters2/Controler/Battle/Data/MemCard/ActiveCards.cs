using System;
using System.Collections.Generic;
using NarlonLib.Math;
using NarlonLib.Tools;
using TaleofMonsters.Core.Config;
using TaleofMonsters.DataType.Decks;

namespace TaleofMonsters.Controler.Battle.Data.MemCard
{
    internal class ActiveCards
    {
        private int index;
        private List<ActiveCard> cards;
        internal static ActiveCard NoneCard = new ActiveCard();

        public int LeftCount
        {
            get { return Math.Max(0, cards.Count - index); }
        }

        private ActiveCards()
        {
            index = 0;
        }

        public ActiveCards(DeckCard[] itsCards)
        {
            cards = new List<ActiveCard>();
            for (int i = 0; i < itsCards.Length; i++)
                cards.Add(new ActiveCard(itsCards[i]));
            ArraysUtils.RandomShuffle(cards);
            index = 0;
        }

        public ActiveCards GetCopy()
        {
            ActiveCards cloneDeck = new ActiveCards();
            cloneDeck.cards = new List<ActiveCard>();
            foreach (var checkCard in cards)
                cloneDeck.cards.Add(new ActiveCard(checkCard.CardId, checkCard.Level, 0));
            return cloneDeck;
        }

        public ActiveCard GetNextCard()
        {
            if (cards.Count == 0)
                return NoneCard;

            int rt = index;
            if (rt >= cards.Count)
                return NoneCard;
            index++;

            if (CardConfigManager.GetCardConfig(cards[rt].CardId).Id == 0)
            {//卡牌配置可能已经过期，用下一个卡
                NarlonLib.Log.NLog.Warn("GetNextCard card is outofdate id={0}", cards[rt].CardId);
                return GetNextCard();
            }

            return cards[rt];
        }

        public ActiveCard ReplaceCard(ActiveCard card)
        {
            if (LeftCount <= 0)
                return NoneCard;

            var targetIndex = index + MathTool.GetRandom(LeftCount);
            var target = cards[targetIndex];
            cards[targetIndex] = card;
            return target;
        }
    }
}
