using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using TaleofMonsters.Controler.Battle.Data.MemMissile;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards.Monsters;

namespace TaleofMonsters.Controler.Battle.Data.Players.Frag
{
    internal class PlayerAction : IPlayerAction
    {
        private Player self;
        public PlayerAction(Player p)
        {
            self = p;
        }

        public void AddResource(int type, int number)
        {
            self.AddResource((GameResourceType)type, number);
        }

        public void AddMonster(int cardId, int level, Point location)
        {
            int size = BattleManager.Instance.MemMap.CardSize;
            var truePos = new Point(location.X / size * size, location.Y / size * size);
            var mon = new Monster(cardId);
            mon.UpgradeToLevel(level);
            LiveMonster newMon = new LiveMonster(level, mon, truePos, self.IsLeft);
            BattleManager.Instance.MonsterQueue.Add(newMon);
            NLog.Debug("AddMonster pid={0} cid={1}", self.PeopleId, cardId);
        }

        public void ExchangeMonster(IMonster target, int lv)
        {
            target.Action.Transform(MonsterBook.GetRandMonsterId());
        }
        public void AddTrap(int id, int spellId, int lv, double rate, int damage, double help)
        {
            self.TrapHolder.AddTrap(id, spellId, lv, rate, damage, help);
        }

        public void RemoveRandomTrap()
        {
            self.TrapHolder.RemoveRandomTrap();
        }

        public void AddSpellMissile(IMonster target, ISpell spell, Point mouse, string effect)
        {
            BasicMissileControler controler = new SpellTraceMissileControler((LiveMonster)target, spell);
            Missile mi = new Missile(effect, mouse.X, mouse.Y, controler, spell.Attr, spell.Damage);
            BattleManager.Instance.MissileQueue.Add(mi);
        }

        public void AddSpellRowMissile(ISpell spell, int count, Point mouse, string effect)
        {
            int size = BattleManager.Instance.MemMap.CardSize;
            int ybase = mouse.Y / size * size;
            int xstart = self.IsLeft ? 0 : BattleManager.Instance.MemMap.StageWidth - size;
            for (int i = -count / 2; i <= count / 2; i++)
            {
                int yoff = ybase + i * size;
                int xend = self.IsLeft ? xstart + spell.Range / 10 * size : xstart - spell.Range / 10 * size;
                BasicMissileControler controler = new SpellLandMissileControler(self, new Point(xend, yoff), spell);
                Missile mi = new Missile(effect, xstart, yoff, controler, spell.Attr, spell.Damage);
                BattleManager.Instance.MissileQueue.Add(mi);
            }
        }

        public void DeleteRandomCardFor(IPlayer p, int levelChange)
        {
            self.HandCards.DeleteRandomCardFor(p, levelChange);
        }

        public void CopyRandomCardFor(IPlayer p, int levelChange)
        {
            self.HandCards.CopyRandomCardFor(p, levelChange);
        }

        public void AddCard(IMonster mon, int cardId, int level)
        {
            self.HandCards.AddCard(cardId, level, 0);
            self.AddCardReason(mon, AddCardReasons.GetCertainCard);
        }
        public void AddCard(IMonster mon, int cardId, int level, int modify)
        {
            self.HandCards.AddCard(cardId, level, modify);
            self.AddCardReason(mon, AddCardReasons.GetCertainCard);
        }

        public void GetNextNCard(IMonster mon, int n)
        {
            self.DrawNextNCard(mon, n, AddCardReasons.DrawCardBySkillOrSpell);
        }
        
        public int GetGraveMonsterId()
        {
            return self.OffCards.GetRandomMonsterFromGrave();
        }

        public void CopyRandomNCard(int n, int spellid)
        {
            self.HandCards.CopyRandomNCard(n, spellid);
        }

        public void DeleteAllCard()
        {
            self.HandCards.DeleteAllCard();
        }

        public void DeleteSelectCard()
        {
            self.HandCards.DeleteCardAt(self.SelectId);
            self.CardsDesk.DisSelectCard();
        }

        public void RecostSelectCard()
        {
            var selectCard = self.HandCards.GetDeckCardAt(self.SelectId);
            if (selectCard == null)
            {
                NLog.Error("RecostSelectCard id={0} not Found", self.SelectId);
                return;
            }
            self.AddMp(-selectCard.Mp);
            self.AddLp(-selectCard.Lp);
            self.AddPp(-selectCard.Pp);
        }
        public void ConvertCard(int count, int cardId, int levelChange)
        {
            self.HandCards.ConvertCard(count, cardId, levelChange);
        }

        public void CardLevelUp(int n, int type)
        {
            self.HandCards.CardLevelUp(n, type);
        }

        public void AddRandomCard(IMonster mon, int type, int lv)
        {
            int cardId = CardConfigManager.GetRandomTypeCard(type);
            if (cardId != 0)
            {
                self.HandCards.AddCard(cardId, lv, 0);
                self.AddCardReason(mon, AddCardReasons.RandomCard);
            }
        }

        public void AddRandomCardJob(IMonster mon, int job, int lv)
        {
            var cardId = CardConfigManager.GetRandomJobCard(job);
            if (cardId != 0)
            {
                self.HandCards.AddCard(cardId, lv, 0);
                self.AddCardReason(mon, AddCardReasons.RandomCard);
            }
        }

        public void AddRandomCardRace(IMonster mon, int race, int lv)
        {
            var cardId = CardConfigManager.GetRandomRaceCard(race);
            if (cardId != 0)
            {
                self.HandCards.AddCard(cardId, lv, 0);
                self.AddCardReason(mon, AddCardReasons.RandomCard);
            }
        }

        public void DiscoverCardType(IMonster mon, int type, int lv, string dtype)
        {
            List<int> cardIds = new List<int>();
            for (int i = 0; i < GameConstants.DiscoverCardCount; i++)
            {
                int cardId = CardConfigManager.GetRandomTypeCard(type);
                cardIds.Add(cardId);
            }
            self.DiscoverCard(mon, cardIds.ToArray(), lv, (DiscoverCardActionType)Enum.Parse(typeof(DiscoverCardActionType), dtype));
        }
        public void DiscoverCardRace(IMonster mon, int race, int lv, string dtype)
        {
            List<int> cardIds = new List<int>();
            for (int i = 0; i < GameConstants.DiscoverCardCount; i++)
            {
                int cardId = CardConfigManager.GetRandomRaceCard(race);
                cardIds.Add(cardId);
            }
            self.DiscoverCard(mon, cardIds.ToArray(), lv, (DiscoverCardActionType)Enum.Parse(typeof(DiscoverCardActionType), dtype));
        }

        public void AddSpellEffect(double rate)
        {
            self.SpecialAttr.SpellEffectAddon += rate;
        }

        public void AddSpellVibrate(double rate)
        {
            self.SpecialAttr.SpellVibrate += rate;
        }

        public void AddSpike(int id)
        {
            self.SpikeManager.AddSpike(id);
        }

        public void RemoveSpike(int id)
        {
            self.SpikeManager.RemoveSpike(id);
        }
    }
}
