using System;
using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;

namespace TaleofMonsters.Controler.Battle.Data.Players.Frag
{
    internal class SpikeManager
    {
        internal class Spike
        {
            public int Id { get; set; }//配置表id
            public bool RemoveOnUseMonster { get; set; }
            public bool RemoveOnUseWeapon { get; set; }
            public bool RemoveOnUseSpell { get; set; }
            public bool CanTimeOut { get; set; }
            public float RoundLeft { get; set; }
        }

        private Player self;
        private List<Spike> spikeList = new List<Spike>();

        public int LpCost { get; private set; }

        public int MpCost { get; private set; }

        public int PpCost { get; private set; }
        
        public SpikeManager(Player player)
        {
            self = player;
        }

        public void AddSpike(int id)
        {
            var configData = ConfigData.GetSpikeConfig(id);
            Spike spike = new Spike
            {
                Id = id,
                RemoveOnUseMonster = configData.RemoveOnUseMonster,
                RemoveOnUseSpell = configData.RemoveOnUseSpell,
                RemoveOnUseWeapon = configData.RemoveOnUseWeapon,
                RoundLeft = configData.Round,
                CanTimeOut = configData.Round > 0
            };

            spikeList.Add(spike);
            ReCheckSpike();
        }

        public bool HasSpike(string name)
        {
            foreach (var spike in spikeList)
            {
                SpikeConfig spikeConfig = ConfigData.GetSpikeConfig(spike.Id);
                if (spikeConfig.Tag == name)
                    return true;
            }
            return false;
        }

        public void RemoveSpike(int id)
        {
            for (int i = 0; i < spikeList.Count; i++)
            {
                if (spikeList[i].Id == id)
                {
                    spikeList.RemoveAt(i);
                    break;
                }
            }
            ReCheckSpike();
        }

        private void ReCheckSpike()
        {
            LpCost = 0;
            PpCost = 0;
            MpCost = 0;
            foreach (var spikeData in spikeList)
            {
                var spikeConfig = ConfigData.GetSpikeConfig(spikeData.Id);
                LpCost += spikeConfig.LpCostChange;
                PpCost += spikeConfig.PpCostChange;
                MpCost += spikeConfig.MpCostChange;
            }
            self.HandCards.UpdateCardCost();
        }

        public void OnRound(float pastRound)
        {
            foreach (var spike in spikeList)
            {
                if (spike.CanTimeOut)
                    spike.RoundLeft -= pastRound;
            }
            var toRemove = spikeList.FindAll(a => a.CanTimeOut && a.RoundLeft <= 0);
            foreach (var spike in toRemove)
                RemoveSpike(spike.Id);
        }

        public void OnUseCard(CardTypes type)
        {
            List<Spike> toRemove = null;
            if (type == CardTypes.Monster)
                toRemove = spikeList.FindAll(a => a.RemoveOnUseMonster);
            else if (type == CardTypes.Spell)
                toRemove = spikeList.FindAll(a => a.RemoveOnUseSpell);
            else if (type == CardTypes.Weapon)
                toRemove = spikeList.FindAll(a => a.RemoveOnUseWeapon);
            if (toRemove != null)
            {
                foreach (var spike in toRemove)
                    RemoveSpike(spike.Id);
            }
        }

        public void CheckCardCost(ActiveCard card)
        {
            var cardType = ConfigIdManager.GetCardType(card.CardId);
            var cardCost = CardConfigManager.GetCardConfig(card.CardId).Cost;
            card.Mp = cardType != CardTypes.Spell ? 0 : Math.Max(0, cardCost + MpCost + card.CostModify);
            card.Lp = cardType != CardTypes.Monster ? 0 : Math.Max(0, cardCost + LpCost + card.CostModify);
            card.Pp = cardType != CardTypes.Weapon ? 0 : Math.Max(0, cardCost + PpCost + card.CostModify);
            if (HasSpike("lp2mp"))
            {
                card.Mp = card.Lp;
                card.Lp = 0;
            }
        }

        public void CheckCardCost(ActiveCard[] cards)
        {
            foreach (var activeCard in cards)
                CheckCardCost(activeCard);
        }
    }
}
