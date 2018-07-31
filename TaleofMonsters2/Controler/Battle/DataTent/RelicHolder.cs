using System;
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
    internal class RelicHolder : ISubscribeUser
    {
        internal class Relic : IRelic
        {
            public IPlayer Owner { get; set; }
            public int Id { get; set; }//weapon表id
            public int Level { get; set; }//技能的等级
            public int Life { get; set; }
        }

        private List<Relic> relicList;
        private Rectangle[] rects;

        public RelicHolder()
        {
            relicList = new List<Relic>();
            rects = new Rectangle[3];
            for (int i = 0; i < rects.Length; i++)
                rects[i] = new Rectangle(6 + 35 * i, 35, 30, 30);

            BattleManager.Instance.EventMsgQueue.Subscribe(this);
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

        public Relic GetRelic(int mx, int my)
        {
            for (int i = 0; i < rects.Length; i++)
            {
                if (rects[i].Contains(new Point(mx, my)))
                {
                    if (i < relicList.Count)
                        return relicList[i];
                }
            }
            return null;
        }

        public Rectangle GetRelicRect(int mx, int my)
        {
            for (int i = 0; i < rects.Length; i++)
            {
                if (rects[i].Contains(new Point(mx, my)))
                {
                    if (i < relicList.Count)
                        return rects[i];
                }
            }
            return new Rectangle();
        }

        public void Draw(Graphics g)
        {
            var bgImg = PicLoader.Read("System", "w0.JPG");
            Font ft1 = new Font("宋体", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            for (int i = 0; i < rects.Length; i++)
            {
                g.DrawImage(bgImg, rects[i]);

                if (i < relicList.Count)
                {
                    var relicInfo = relicList[i];
                    var rect = rects[i];
                    g.DrawImage(CardAssistant.GetCardImage(relicInfo.Id, 30, 30), rect);

                    g.DrawString(relicInfo.Life.ToString(), ft1, Brushes.White, rect.X, rect.Y);
                    Pen colorPen = new Pen(relicInfo.Owner.IsLeft ? Color.Red : Color.Blue, 3);
                    g.DrawRectangle(colorPen, rect);
                    colorPen.Dispose();
                }
            }
            bgImg.Dispose();
            ft1.Dispose();
        }

        public void OnMessage(EventMsgQueue.EventMsgTypes type, IPlayer p, IMonster src, IMonster dest, HitDamage damage, Point l, int cardId, int cardType, int cardLevel)
        {
            foreach (var relic in relicList)
            {
                var relicConfig = ConfigData.GetWeaponConfig(relic.Id);
                var typeV = (EventMsgQueue.EventMsgTypes)Enum.Parse(typeof(EventMsgQueue.EventMsgTypes), relicConfig.RelicType);
                if (typeV == type)
                {
                    bool result = false;
                    relicConfig.RelicUseEffect(p, relic, cardId, cardType, src, ref result);
                    if (result)
                    {
                        TriggerRelic(relic);
                        NLog.Debug("OnMessage id={0}", relic.Id);
                        if (src != null)
                            BattleManager.Instance.EffectQueue.Add(new MonsterBindEffect(EffectBook.GetEffect(relicConfig.RelicEffect), src as LiveMonster, false));
                    }
                }
            }
        }
    }
}