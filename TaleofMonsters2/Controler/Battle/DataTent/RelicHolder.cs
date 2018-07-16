using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.MemWeapon;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal class RelicHolder
    {
        public List<Relic> RelicList { get; private set; }

        public event Player.PlayerUseCardEventHandler OnTrapRemove;

        public RelicHolder()
        {
            RelicList = new List<Relic>();
        }

        public void AddRelic(Player owner, int id, int spellId, int lv, double rate, int damage, double help)
        {
            if (RelicList.Count >= GameConstants.MaxTrapCount)
                return;

            RelicList.Add(new Relic
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

        public void RemoveRandomRelic(Player target)
        {
            var myTrapList = RelicList.FindAll(trap => trap.Owner.IsLeft == target.IsLeft);
            if (myTrapList.Count > 0)
            {
                var trap = myTrapList[MathTool.GetRandom(myTrapList.Count)];
                RelicList.Remove(trap);
                if (OnTrapRemove != null)
                    OnTrapRemove(trap.SpellId, trap.Level, target.IsLeft);
            }
        }

        private void RemoveRelic(Relic relic, RelicConfig config)
        {
            if (MathTool.GetRandom(100) >= relic.Rate)
                relic.Owner.AddMp(-config.ManaCost);
            RelicList.RemoveAll(s => s.Id == relic.Id);
            if (OnTrapRemove != null)
                OnTrapRemove(relic.SpellId, relic.Level, relic.Owner.IsLeft);
        }

        public bool CheckOnUseCard(ActiveCard selectCard, Point location, IPlayer targetPlayer)
        {
            foreach (var trap in RelicList)
            {
                if (trap.Owner.IsLeft == targetPlayer.IsLeft)
                    continue;

                var relicConfig = ConfigData.GetRelicConfig(trap.Id);
                if (relicConfig.EffectUse != null)
                {
                    if (trap.Owner.Mp >= relicConfig.ManaCost &&
                        relicConfig.EffectUse(trap.Owner, targetPlayer, trap, selectCard.CardId,
                            (int) selectCard.CardType))
                    {
                        RemoveRelic(trap, relicConfig);
                        NLog.Debug("CheckOnUseCard id={0} cardId={1}", trap.Id, selectCard.CardId);
                        BattleManager.Instance.EffectQueue.Add(
                            new MonsterBindEffect(EffectBook.GetEffect(relicConfig.UnitEffect), location, false));

                        return true;
                    }
                }
            }

            return false;
        }

        public void CheckOnSummon(IMonster mon, IPlayer targetPlayer)
        {
            foreach (var relic in RelicList)
            {
                if (relic.Owner.IsLeft == targetPlayer.IsLeft)
                    continue;

                var relicConfig = ConfigData.GetRelicConfig(relic.Id);
                if (relicConfig.EffectSummon != null)
                {
                    if (relic.Owner.Mp >= relicConfig.ManaCost &&
                        relicConfig.EffectSummon(relic.Owner, targetPlayer, relic, mon, relic.Level))
                    {
                        RemoveRelic(relic, relicConfig);
                        NLog.Debug("CheckOnSummon id={0} cardId={1}", relic.Id, mon.Id);
                        BattleManager.Instance.EffectQueue.Add(
                            new MonsterBindEffect(EffectBook.GetEffect(relicConfig.UnitEffect), mon as LiveMonster, false));
                        return;
                    }
                }
            }
        }
    }
}