using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.DataType;

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
        private List<Spike> spikeList = new List<Spike>();

        public int LpCost { get; private set; }

        public int MpCost { get; private set; }

        public int PpCost { get; private set; }

        private Player self;

        public SpikeManager(Player player)
        {
            self = player;
        }

        public void AddSpike(int id)
        {
            Spike spike = new Spike();
            spike.Id = id;
            var configData = ConfigData.GetSpikeConfig(id);
            spike.RemoveOnUseMonster = configData.RemoveOnUseMonster;
            spike.RemoveOnUseSpell = configData.RemoveOnUseSpell;
            spike.RemoveOnUseWeapon = configData.RemoveOnUseWeapon;
            spike.RoundLeft = configData.Round;
            spike.CanTimeOut = configData.Round > 0;
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
            self.CardManager.UpdateCardCost();
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
            {
                toRemove = spikeList.FindAll(a => a.RemoveOnUseMonster);
            }
            else if (type == CardTypes.Spell)
            {
                toRemove = spikeList.FindAll(a => a.RemoveOnUseSpell);
            }
            else if (type == CardTypes.Weapon)
            {
                toRemove = spikeList.FindAll(a => a.RemoveOnUseWeapon);
            }
            if (toRemove != null)
            {
                foreach (var spike in toRemove)
                    RemoveSpike(spike.Id);
            }
        }
    }
}
