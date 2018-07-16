using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal class RelicHolder
    {
        internal class Relic : IRelic
        {
            public Player Owner { get; set; }
            public int Id { get; set; }//weapon表id
            public int Level { get; set; }//技能的等级
            public int Life { get; set; }
        }

        private List<Relic> relicList;

        public RelicHolder()
        {
            relicList = new List<Relic>();
        }

        public void AddRelic(Player owner, int id, int lv, int life)
        {
            relicList.Insert(0, new Relic
            {
                Owner = owner,
                Id = id,
                Level = lv,
                Life = life
            });

            if (relicList.Count > GameConstants.MaxRelicCount)
            {
                relicList.RemoveAt(relicList.Count);
            }

        }

        private void TriggerRelic(Relic relic)
        {
            relic.Life--;
            if (relic.Life <= 0)
                relicList.Remove(relic);
        }

        public void CheckOnUseCard(ActiveCard selectCard, Point location, IPlayer targetPlayer)
        {
            foreach (var relic in relicList)
            {
                if (relic.Owner.IsLeft == targetPlayer.IsLeft)
                    continue;

                var relicConfig = ConfigData.GetWeaponConfig(relic.Id);
                if (relicConfig.EffectUse != null)
                {
                    bool result = false;
                    relicConfig.EffectUse(relic.Owner, targetPlayer, relic, selectCard.CardId, (int) selectCard.CardType, ref result);
                    if (result)
                    {
                        TriggerRelic(relic);
                        NLog.Debug("CheckOnUseCard id={0} cardId={1}", relic.Id, selectCard.CardId);
                        BattleManager.Instance.EffectQueue.Add(
                            new MonsterBindEffect(EffectBook.GetEffect(relicConfig.RelicEffect), location, false));

                        return;
                    }
                }
            }
        }

        public void CheckOnSummon(IMonster mon, IPlayer targetPlayer)
        {
            foreach (var relic in relicList)
            {
                if (relic.Owner.IsLeft == targetPlayer.IsLeft)
                    continue;

                var relicConfig = ConfigData.GetWeaponConfig(relic.Id);
                if (relicConfig.EffectSummon != null)
                {
                    bool result = false;
                    relicConfig.EffectSummon(relic.Owner, targetPlayer, relic, mon, relic.Level, ref result);
                    if (result)
                    {
                        TriggerRelic(relic);
                        NLog.Debug("CheckOnSummon id={0} cardId={1}", relic.Id, mon.Id);
                        BattleManager.Instance.EffectQueue.Add(
                            new MonsterBindEffect(EffectBook.GetEffect(relicConfig.RelicEffect), mon as LiveMonster, false));
                        return;
                    }
                }
            }
        }

        public void Draw(Graphics g)
        {
            for (int i = 0; i < relicList.Count; i++)
            {
                var relicInfo = relicList[i];
                var rect = new Rectangle(6 + 35 * i, 35, 30, 30);
                g.DrawImage(CardAssistant.GetCardImage(relicInfo.Id, 30, 30), rect);

                Pen colorPen = new Pen(relicInfo.Owner.IsLeft ? Color.Red : Color.Blue, 3);
                g.DrawRectangle(colorPen, rect);
                colorPen.Dispose();
            }

            var bgImg = PicLoader.Read("System", "w0.JPG");
            for (int i = relicList.Count; i < GameConstants.MaxRelicCount; i++)
            {
                var rect = new Rectangle(6 + 35 * i, 35, 30, 30);
                g.DrawImage(bgImg, rect);
            }
            bgImg.Dispose();
        }
    }
}