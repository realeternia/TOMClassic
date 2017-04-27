using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.MemSpell;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Effects;

namespace TaleofMonsters.Controler.Battle.Data.Players.Frag
{
    internal class TrapHolder
    {
        private List<Trap> trapList = new List<Trap>();
        private Player self;

        public TrapHolder(Player p)
        {
            self = p;
        }

        public int Count
        {
            get { return trapList.Count; }
        }

        public void AddTrap(int id, int lv, double rate, int damage, double help)
        {
            trapList.Add(new Trap { Id = id, Level = lv, Rate = rate, Damage = damage, Help = help });
            self.OnTrapChange();
        }

        public void RemoveRandomTrap()
        {
            if (trapList.Count > 0)
            {
                trapList.RemoveAt(MathTool.GetRandom(trapList.Count));
                self.OnTrapChange();
            }
        }

        private void RemoveTrap(Trap trap, SpellTrapConfig config)
        {
            if (MathTool.GetRandom(100) >= trap.Rate)
            {
                self.AddMp(-config.ManaCost);
            }
            trapList.RemoveAll(s => s.Id == trap.Id);
            self.OnTrapChange();
        }

        public bool CheckTrapOnUseCard(ActiveCard selectCard, Point location, IPlayer rival)
        {
            foreach (var trap in trapList)
            {
                var trapConfig = ConfigData.GetSpellTrapConfig(trap.Id);
                if (trapConfig.EffectUse != null)
                {
                    if (self.Mp >= trapConfig.ManaCost && trapConfig.EffectUse(self, rival, trap, selectCard.CardId, (int)selectCard.CardType))
                    {
                        RemoveTrap(trap, trapConfig);
                        NLog.Debug(string.Format("RemoveTrap UseCard id={0} cardId={1}", trap.Id, selectCard.CardId));
                        BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(trapConfig.UnitEffect), location, false));

                        return true;
                    }
                }
            }

            return false;
        }

        public void CheckTrapOnSummon(IMonster mon, IPlayer rival)
        {
            foreach (var trap in trapList)
            {
                var trapConfig = ConfigData.GetSpellTrapConfig(trap.Id);
                if (trapConfig.EffectSummon != null)
                {
                    if (self.Mp >= trapConfig.ManaCost && trapConfig.EffectSummon(self, rival, trap, mon, trap.Level))
                    {
                        RemoveTrap(trap, trapConfig);
                        NLog.Debug(string.Format("RemoveTrap Summon id={0} cardId={1}", trap.Id, mon.Id));
                        BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(trapConfig.UnitEffect), mon as LiveMonster, false));
                        return;
                    }
                }
            }
        }

        public void GenerateImage(ControlPlus.TipImage tipData, bool isPlayerControl)
        {
            if (trapList.Count > 0)
            {
                tipData.AddLine();
                tipData.AddTextNewLine("陷阱", "White");
                foreach (var trap in trapList)
                {
                    var trapConfig = ConfigData.GetSpellTrapConfig(trap.Id);
                    if (isPlayerControl)
                    {
                        tipData.AddTextNewLine(trapConfig.Name, "Lime");
                        tipData.AddText(string.Format("Lv{0} {1:0.0}%", trap.Level, trap.Rate), "White");
                    }
                    else
                    {
                        tipData.AddTextNewLine("???", "Red");
                    }
                }
            }
        }
    }
}