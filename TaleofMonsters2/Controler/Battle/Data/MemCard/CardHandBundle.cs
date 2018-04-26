using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using NarlonLib.Tools;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;

namespace TaleofMonsters.Controler.Battle.Data.MemCard
{
    internal class CardHandBundle
    {
        private Player self;
        private ActiveCard[] cardArray = new ActiveCard[GameConstants.CardSlotMaxCount];
        
        public CardHandBundle(Player p)
        {
            self = p;
            for (int i = 0; i < GameConstants.CardSlotMaxCount; i++)
                cardArray[i] = ActiveCard.NoneCard;
        }

        public void GetNextCard()
        {
            if (GetCardNumber() < GameConstants.CardSlotMaxCount)
            {
                ActiveCard next = self.OffCards.GetNextCard();
                if (next != ActiveCard.NoneCard)
                    AddCard(next);
                else
                    self.OnGetCardFail(true); //卡组抽完有惩罚
            }
            else
            {
                self.OnGetCardFail(false);//手牌满了有惩罚
            }
        }
        private void SetCard(int id, ActiveCard card)
        {
            int count = GetCardNumber();
            if (id < count)
                cardArray[id] = card;
            UpdateCardView();
        }

        public void AddCard(ActiveCard card)
        {
            var spikeManager = self.SpikeManager;
            spikeManager.CheckCardCost(card);
            int count = GetCardNumber();
            if (count < GameConstants.CardSlotMaxCount)
                cardArray[count] = card;
            if (spikeManager.HasSpike("copycard") && count < GameConstants.CardSlotMaxCount - 1)
                cardArray[count + 1] = card.GetCopy();
            UpdateCardView();
        }

        public void UpdateCardCost()
        {
            var spikeManager = self.SpikeManager;
            spikeManager.CheckCardCost(cardArray);
            UpdateCardView();
        }

        public void UpdateCardView()
        {
            if (self.CardsDesk != null)
                self.CardsDesk.UpdateSlot(cardArray);
        }

        public ActiveCard GetCardAt(int index)
        {
            if (index > GameConstants.CardSlotMaxCount || index <= 0)
                return ActiveCard.NoneCard;

            return cardArray[index - 1];
        }

        public int GetCardNumber()
        {
            int count = 0;
            for (int i = 0; i < GameConstants.CardSlotMaxCount; i++)
            {
                if (cardArray[i].CardId != 0)
                    count++;
            }
            return count;
        }

        public void DeleteCardAt(int index)
        {
            if (index <= 0)
            {//使用英雄技能卡
                return;
            }

            cardArray[index - 1] = ActiveCard.NoneCard;
            for (int i = 0; i < GameConstants.CardSlotMaxCount - 1; i++)
            {
                if (cardArray[i].CardId == 0 && cardArray[i + 1].CardId > 0)
                {
                    ActiveCard tempCard = cardArray[i];
                    cardArray[i] = cardArray[i + 1];
                    cardArray[i + 1] = tempCard;
                }
            }
            UpdateCardView();
        }

        /// <summary>
        /// 把第几张牌替换成下一张卡片,初始化使用
        /// </summary>
        /// <param name="index">偏移</param>
        public void RedrawCardAt(int index)
        {
            var newCard = self.OffCards.ReplaceCard(cardArray[index - 1]);
            if (newCard == ActiveCard.NoneCard)
                return;

            cardArray[index - 1] = newCard;
            UpdateCardView();
        }

        public void DeleteRandomCardFor(IPlayer p, int levelChange)
        {
            if (GetCardNumber() > 0)
            {
                int id = MathTool.GetRandom(GetCardNumber());
                ActiveCard card = cardArray[id];
                DeleteCardAt(id + 1);

                card.ChangeLevel(levelChange);
                Player player = p as Player;
                if (player != null)
                    player.HandCards.AddCard(card);
            }
        }

        public void CopyRandomCardFor(IPlayer p, int levelChange)
        {
            if (GetCardNumber() > 0)
            {
                int id = MathTool.GetRandom(GetCardNumber());
                ActiveCard card = cardArray[id].GetCopy();

                card.ChangeLevel(levelChange);
                Player player = p as Player;
                if (player != null)
                    player.HandCards.AddCard(card);
            }
        }

        public void AddCard(int cardId, int level, int modify)
        {
            ActiveCard card = new ActiveCard(cardId, (byte)level);
            card.CostModify = modify;
            AddCard(card);
        }

        public void CopyRandomNCard(int n, int spellId)
        {
            List<int> indexs = new List<int>();
            for (int i = 0; i < GameConstants.CardSlotMaxCount; i++)
            {
                if (cardArray[i].CardId != 0 && (cardArray[i].CardId != spellId || cardArray[i].CardType != CardTypes.Spell))
                    indexs.Add(i);
            }
            ArraysUtils.RandomShuffle(indexs);
            for (int i = 0; i < Math.Min(n, indexs.Count); i++)
                AddCard(cardArray[indexs[i]].GetCopy());
        }

        public void DeleteAllCard()
        {
            for (int i = 0; i < GameConstants.CardSlotMaxCount; i++)
                cardArray[i] = ActiveCard.NoneCard;

            UpdateCardView();
        }


        public void ConvertCard(int count, int cardId, int levelChange)
        {
            int num = GetCardNumber();
            int id = MathTool.GetRandom(num);
            for (int i = 0; i < count; i++)
            {
                if (num <= i) continue;

                var oldLevel = cardArray[id].Level;
                SetCard((id + i) % num, new ActiveCard(cardId, (byte)Math.Max(1, oldLevel + levelChange)));
            }
        }

        public void CardLevelUp(int n, int type)
        {
            foreach (var pickCard in cardArray)
            {
                if (type != 0 && ConfigIdManager.GetCardType(pickCard.CardId) != (CardTypes)type)
                    continue;

                pickCard.ChangeLevel(n);
            }

            UpdateCardView();
        }
    }
}
