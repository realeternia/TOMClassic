using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class CardManager
    {
        private Player self;
        private ActiveCard[] cards = new ActiveCard[SysConstants.CardSlotMaxCount];

        public CardManager(Player p)
        {
            self = p;
            for (int i = 0; i < SysConstants.CardSlotMaxCount; i++)
                cards[i] = ActiveCards.NoneCard;
        }

        public void GetNextCard()
        {
            if (GetCardNumber() < SysConstants.CardSlotMaxCount)
            {
                ActiveCard next = self.Cards.GetNextCard();
                if (next != ActiveCards.NoneCard)
                {
                    AddCard(next);
                }
            }
        }
        private void SetCard(int id, ActiveCard card)
        {
            int count = GetCardNumber();
            if (id < count)
            {
                cards[id] = card;
            }
            if (self.CardsDesk != null)
                self.CardsDesk.UpdateSlot(cards);
        }

        public void AddCard(ActiveCard card)
        {
            if (card.CardType == CardTypes.Spell && self.MpCost != 0)
            {
                card.MpCostChange = self.MpCost;
            }
            else if (card.CardType == CardTypes.Monster && self.LpCost != 0)
            {
                card.LpCostChange = self.LpCost;
            }
            else if (card.CardType == CardTypes.Weapon && self.PpCost != 0)
            {
                card.PpCostChange = self.PpCost;
            }
            int count = GetCardNumber();
            if (count < SysConstants.CardSlotMaxCount)
            {
                cards[count] = card;
            }
            if (self.CardsDesk != null)
                self.CardsDesk.UpdateSlot(cards);
        }

        public void UpdateCardCost()
        {
            foreach (ActiveCard activeCard in cards)
            {
                activeCard.LpCostChange = self.LpCost;
                activeCard.MpCostChange = self.MpCost;
                activeCard.PpCostChange = self.PpCost;
            }
            if (self.CardsDesk != null)
                self.CardsDesk.UpdateSlot(cards);
        }

        public ActiveCard GetDeckCardAt(int index)
        {
            if (index > SysConstants.CardSlotMaxCount || index <= 0)
                return ActiveCards.NoneCard;

            return cards[index - 1];
        }

        public void DeleteCardAt(int index)
        {
            if (index <= 0)
            {//使用英雄技能卡
                return;
            }

            cards[index - 1] = ActiveCards.NoneCard;
            for (int i = 0; i < SysConstants.CardSlotMaxCount - 1; i++)
            {
                if (cards[i].Id == 0 && cards[i + 1].Id > 0)
                {
                    ActiveCard tempCard = cards[i];
                    cards[i] = cards[i + 1];
                    cards[i + 1] = tempCard;
                }
            }
            if (self.CardsDesk != null)
            {
                self.CardsDesk.UpdateSlot(cards);
            }
        }

        /// <summary>
        /// 把第几张牌替换成下一张卡片
        /// </summary>
        /// <param name="index">偏移</param>
        public void GetNextCardAt(int index)
        {
            cards[index - 1] = self.Cards.GetNextCard();
            if (self.CardsDesk != null)
            {
                self.CardsDesk.UpdateSlot(cards);
            }
        }

        public void DeleteRandomCardFor(IPlayer p, int levelChange)
        {
            if (GetCardNumber() > 0)
            {
                int id = MathTool.GetRandom(GetCardNumber());
                ActiveCard card = cards[id];
                DeleteCardAt(id + 1);

                card.ChangeLevel((byte)(card.Level + levelChange));
                Player player = p as Player;
                if (player != null)
                {
                    player.CardManager.AddCard(card);
                }
            }
        }

        public void CopyRandomCardFor(IPlayer p, int levelChange)
        {
            if (GetCardNumber() > 0)
            {
                int id = MathTool.GetRandom(GetCardNumber());
                ActiveCard card = cards[id].GetCopy();

                card.ChangeLevel((byte)(card.Level + levelChange));
                Player player = p as Player;
                if (player != null)
                {
                    player.CardManager.AddCard(card);
                }
            }
        }

        public void AddCard(int cardId, int level)
        {
            ActiveCard card = new ActiveCard(cardId, (byte)level, 0);
            AddCard(card);
        }

        public void GetNextNCard(int n)
        {
            for (int i = 0; i < n; i++)
                GetNextCard();
        }

        public void CopyRandomNCard(int n, int spellid)
        {
            List<int> indexs = new List<int>();
            for (int i = 0; i < SysConstants.CardSlotMaxCount; i++)
            {
                if (cards[i].CardId != 0 && (cards[i].CardId != spellid || cards[i].CardType != CardTypes.Spell))
                    indexs.Add(i);
            }
            indexs = RandomShuffle.Process(indexs.ToArray());
            for (int i = 0; i < Math.Min(n, indexs.Count); i++)
            {
                AddCard(cards[indexs[i]].GetCopy());
            }
        }

        public void DeleteAllCard()
        {
            for (int i = 0; i < SysConstants.CardSlotMaxCount; i++)
            {
                cards[i] = ActiveCards.NoneCard;
            }
            if (self.CardsDesk != null)
            {
                self.CardsDesk.UpdateSlot(cards);
            }
        }

        public int GetCardNumber()
        {
            int count = 0;
            for (int i = 0; i < SysConstants.CardSlotMaxCount; i++)
            {
                if (cards[i].Id != 0)
                    count++;
            }
            return count;
        }

        public void ConvertCard(int count, int cardId, int levelChange)
        {
            int num = GetCardNumber();
            int id = MathTool.GetRandom(num);
            for (int i = 0; i < count; i++)
            {
                if (num <= i) continue;

                var oldLevel = cards[id].Level;
                SetCard((id + i) % num, new ActiveCard(cardId, (byte)Math.Max(1, oldLevel + levelChange), 0));
            }
        }

        public void CardLevelUp(int n, int type)
        {
            foreach (ActiveCard activeCard in cards)
            {
                if (type != 0 && CardAssistant.GetCardType(activeCard.CardId) != (CardTypes)type)
                {
                    continue;
                }

                activeCard.Level = (byte)(activeCard.Level + n);
                if (activeCard.Level < 1)
                {
                    activeCard.Level = 1;
                }
                else if (activeCard.Level > SysConstants.CardMaxLevel)
                {
                    activeCard.Level = SysConstants.CardMaxLevel;
                }
            }

            if (self.CardsDesk != null)
            {
                self.CardsDesk.UpdateSlot(cards);
            }
        }
    }
}
