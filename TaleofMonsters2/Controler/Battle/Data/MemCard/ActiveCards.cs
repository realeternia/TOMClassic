using System;
using System.Collections.Generic;
using NarlonLib.Math;
using TaleofMonsters.Config;
using TaleofMonsters.Core;
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
            ActiveCard[] tcards = new ActiveCard[GameConstants.DeckCardCount];
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                tcards[i] = new ActiveCard(itsCards[i]);
            }
            for (int i = 0; i < 100; i++)
            {
                int x = MathTool.GetRandom(GameConstants.DeckCardCount);
                int y = MathTool.GetRandom(GameConstants.DeckCardCount);
                ActiveCard temp = tcards[x];
                tcards[x] = tcards[y];
                tcards[y] = temp;
            }
            cards = new List<ActiveCard>(tcards);
            index = 0;
        }

        public ActiveCards GetCopy()
        {
            ActiveCards tcards = new ActiveCards();
            tcards.cards = new List<ActiveCard>();
            foreach (ActiveCard activeCard in cards)
            {
                tcards.cards.Add(new ActiveCard(activeCard.CardId, activeCard.Level, 0));
            }
            return tcards;
        }

        public ActiveCard GetNextCard()
        {
            if (cards.Count == 0)
            {
                return NoneCard;
            }

            int rt = index;
            if (rt >= cards.Count)
            {
                return NoneCard;
            }
            index++;

            if (CardConfigManager.GetCardConfig(cards[rt].CardId).Id == 0)
            {//卡牌配置可能已经过期，用下一个卡
                NarlonLib.Log.NLog.Warn(string.Format("card is outofdate id={0}", cards[rt].CardId));
                return GetNextCard();
            }

            return cards[rt];
        }

        public ActiveCard ReplaceCard(ActiveCard card)
        {
            if (LeftCount <= 0)
            {
                return NoneCard;
            }

            var targetIndex = index + MathTool.GetRandom(LeftCount);
            var target = cards[targetIndex];
            cards[targetIndex] = card;
            return target;
        }
    }
}
