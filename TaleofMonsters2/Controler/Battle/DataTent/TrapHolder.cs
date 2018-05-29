using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.MemSpell;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal class TrapHolder
    {
        public List<Trap> TrapList { get; private set; }

        public event Player.PlayerUseCardEventHandler OnTrapRemove;

        public TrapHolder()
        {
            TrapList = new List<Trap>();
        }

        public void AddTrap(Player owner, int id, int spellId, int lv, double rate, int damage, double help)
        {
            if (TrapList.Count >= GameConstants.MaxTrapCount)
                return;

            TrapList.Add(new Trap
            {
                Owner = owner,
                Id = id,
                SpellId = spellId,
                Level = lv,
                Rate = rate,
                Damage = damage,
                Help = help
            });
        }

        public void RemoveRandomTrap(Player target)
        {
            var myTrapList = TrapList.FindAll(trap => trap.Owner == target);
            if (myTrapList.Count > 0)
            {
                var trap = myTrapList[MathTool.GetRandom(myTrapList.Count)];
                TrapList.Remove(trap);
                if (OnTrapRemove != null)
                    OnTrapRemove(trap.SpellId, trap.Level, target.IsLeft);
            }
        }

        private void RemoveTrap(Trap trap, SpellTrapConfig config)
        {
            if (MathTool.GetRandom(100) >= trap.Rate)
                trap.Owner.AddMp(-config.ManaCost);
            TrapList.RemoveAll(s => s.Id == trap.Id);
            if (OnTrapRemove != null)
                OnTrapRemove(trap.SpellId, trap.Level, trap.Owner.IsLeft);
        }

        public bool CheckTrapOnUseCard(ActiveCard selectCard, Point location, IPlayer targetPlayer)
        {
            foreach (var trap in TrapList)
            {
                if (trap.Owner == targetPlayer)
                    continue;

                var trapConfig = ConfigData.GetSpellTrapConfig(trap.Id);
                if (trapConfig.EffectUse != null)
                {
                    if (trap.Owner.Mp >= trapConfig.ManaCost &&
                        trapConfig.EffectUse(trap.Owner, targetPlayer, trap, selectCard.CardId,
                            (int) selectCard.CardType))
                    {
                        RemoveTrap(trap, trapConfig);
                        NLog.Debug("CheckTrapOnUseCard id={0} cardId={1}", trap.Id, selectCard.CardId);
                        BattleManager.Instance.EffectQueue.Add(
                            new MonsterBindEffect(EffectBook.GetEffect(trapConfig.UnitEffect), location, false));

                        return true;
                    }
                }
            }

            return false;
        }

        public void CheckTrapOnSummon(IMonster mon, IPlayer targetPlayer)
        {
            foreach (var trap in TrapList)
            {
                if (trap.Owner == targetPlayer)
                    continue;

                var trapConfig = ConfigData.GetSpellTrapConfig(trap.Id);
                if (trapConfig.EffectSummon != null)
                {
                    if (trap.Owner.Mp >= trapConfig.ManaCost &&
                        trapConfig.EffectSummon(trap.Owner, targetPlayer, trap, mon, trap.Level))
                    {
                        RemoveTrap(trap, trapConfig);
                        NLog.Debug("CheckTrapOnSummon id={0} cardId={1}", trap.Id, mon.Id);
                        BattleManager.Instance.EffectQueue.Add(
                            new MonsterBindEffect(EffectBook.GetEffect(trapConfig.UnitEffect), mon as LiveMonster, false));
                        return;
                    }
                }
            }
        }
    }
}