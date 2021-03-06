﻿using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players.Frag;
using TaleofMonsters.Core.Interface;
using TaleofMonsters.Core;
using System.Drawing;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Controler.Battle.Data.MemSpell;
using NarlonLib.Log;
using TaleofMonsters.Controler.Battle.Components.CardSelect;
using TaleofMonsters.Controler.Battle.Data.MemWeapon;
using TaleofMonsters.Controler.Battle.Data.Players.AIs;
using TaleofMonsters.Controler.Battle.DataTent;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Cards.Spells;
using TaleofMonsters.Datas.Cards.Weapons;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;
using TaleofMonsters.Datas.Equips;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class Player : IPlayer
    {
        public delegate void PlayerPointEventHandler();
        public event PlayerPointEventHandler ManaChanged;
        public PlayerPointEventHandler CardLeftChanged;
        public PlayerPointEventHandler HandCardChanged;

        public delegate void PlayerHeroSkillStateEventHandler(bool active);
        public event PlayerHeroSkillStateEventHandler HeroSkillChanged;

        public delegate void PlayerUseCardEventHandler(int cardId, int level, bool isLeft);
        public event PlayerUseCardEventHandler OnUseCard;
        public event PlayerUseCardEventHandler OnKillEnemy;

        public delegate void ShowCardSelectorEventHandler(Player p, ICardSelectMethod m);
        public event ShowCardSelectorEventHandler OnShowCardSelector;

        private float recoverTime; //魔法的恢复累计值

        public EnergyGenerator EnergyGenerator { get; set; }
        public SpikeManager SpikeManager { get; set; }
        public CardHandBundle HandCards { get; private set; }//手牌
        public CardOffBundle OffCards { get; protected set; }//牌库的牌
        public EquipModifier Modifier { get; protected set; }
        public IPlayerAction Action { get; private set; }
        public PlayerSpecialAttr SpecialAttr { get; private set; }
        public AIStrategyContext AIContext { get; set; }

        public List<int> HeroSkillList = new List<int>();
        private float heroSkillCd;
        public bool IsAlive { get; set; }//是否活着

        private float comboTime;//>0表示在combo状态
        public bool Combo { get { return comboTime > 0; } }
        private int lastSpellId;
        public bool IsLastSpellAttr(int monAttr)
        {
            if (lastSpellId > 0)
                return ConfigData.GetSpellConfig(lastSpellId).Attr == monAttr;
            return false;
        }

        #region 属性

        public bool IsLeft { get; private set; }

        public float Mp { get; set; }

        public float Lp { get; set; }

        public float Pp { get; set; }
        public float EnergyRate {
            get { return 100*(Mp + Lp + Pp)/(EnergyGenerator.LimitLp + EnergyGenerator.LimitMp + EnergyGenerator.LimitPp);}
        }

        public int Luk { get; set; } //玩家的幸运值

        public int Level { get; protected set; }

        public int Job { get; protected set; }

        public ICardList CardsDesk { get; set; }

        public IMonster Tower { get { return BattleManager.Instance.MonsterQueue.GetKingTower(IsLeft); } }
        public IPlayer Rival { get { return IsLeft ? BattleManager.Instance.PlayerManager.RightPlayer : BattleManager.Instance.PlayerManager.LeftPlayer; } }
        public int SelectCardId { get { return CardsDesk.GetSelectCard().CardId; } }
        public int SelectId { get { return CardsDesk.GetSelectId(); } }

        public int PeopleId { get; set; }
        
        public int CardNumber { get { return HandCards.GetCardNumber(); } } //手牌数量

        public HolyBook HolyBook { get; private set; }

        protected bool noCardOutPunish; //没有卡牌消耗完的惩罚

        #endregion

        public Player(bool isLeft)
        {
            IsLeft = isLeft;
            IsAlive = true;
            HandCards = new CardHandBundle(this);
            EnergyGenerator = new EnergyGenerator(this);
            SpikeManager = new SpikeManager(this);
            Modifier = new EquipModifier();
            Action = new PlayerAction(this);
            SpecialAttr = new PlayerSpecialAttr();
            HolyBook = new HolyBook();
            Lp = 3;
            Mp = 3;
            Pp = 3;
        }

        protected void CalculateEquipAndSkill(List<Equip> equipList, int[] energyData)
        {
            var jobConfig = ConfigData.GetJobConfig(Job);
            if (jobConfig.SkillId > 0)
                HeroSkillList.Add(jobConfig.SkillId);//添加职业技能

            int lukChange = 0;
            foreach (var equip in equipList)
            {
                EquipConfig equipConfig = ConfigData.GetEquipConfig(equip.TemplateId);
                for (int i = 0; i < 3; i++)
                    energyData[i] += equipConfig.EnergyRate[i];

                if (equipConfig.HeroSkillId > 0)
                    HeroSkillList.Add(equipConfig.HeroSkillId); //添加装备附带的技能
                if (equipConfig.Position == (int)EquipTypes.Core)
                    Modifier.CoreId = equipConfig.Id;
                else if (equipConfig.Position == (int)EquipTypes.Wall)
                {
                    if (Modifier.Wall1Id == 0)
                        Modifier.Wall1Id = equipConfig.Id;
                    else
                        Modifier.Wall2Id = equipConfig.Id;
                }
                else if (equipConfig.Position == (int)EquipTypes.Weapon)
                {
                    if (Modifier.Weapon1Id == 0)
                        Modifier.Weapon1Id = equipConfig.Id;
                    else
                        Modifier.Weapon2Id = equipConfig.Id;
                }

                lukChange += equipConfig.Luk;
            }
            if (lukChange != 0)
                AddLuk(null, lukChange);

            foreach (var equip in equipList)
                Modifier.UpdateInfo(equip.GetEquipAddons(false), equip.CommonSkillList);

            if (HeroSkillList.Count > 3)
                HeroSkillList = HeroSkillList.GetRange(HeroSkillList.Count - 3, 3); //最多保留3个技能
        }

        public void Update(bool isFast, float pastRound, int round)
        {
            recoverTime += pastRound * GameConstants.RoundRecoverAddon * (round >= GameConstants.RoundRecoverDoubleRound ? 2 : 1);
            var need = isFast ? GameConstants.DrawManaTimeFast : GameConstants.DrawManaTime;
            need = (float)(need * EnergyGenerator.GainEpRate);
            if (recoverTime >= need)
            {
                recoverTime -= need;
                AddManaData(EnergyGenerator.NextAimMana, 1);
                EnergyGenerator.UseMana();
                EnergyGenerator.Next(round);
            }
            if (ManaChanged != null)//todo 先ws下
                ManaChanged();
            SpikeManager.OnRound(pastRound);
            comboTime -= pastRound;
            if (comboTime <= 0)
            {
                comboTime = 0;
                HandCards.UpdateCardView();
            }
            if (heroSkillCd > 0)
            {
                heroSkillCd -= pastRound;
                if (HeroSkillChanged != null && heroSkillCd <= 0)
                    HeroSkillChanged(true);
            }

            if (AIContext != null)
            {
                AIContext.OnTimePast();
            }
        }

        private void AddRandomMana(int killerId, int count)
        {
            List<PlayerManaTypes> manas = new List<PlayerManaTypes>();
            for (int i = 0; i < count; i++)
            {
                var get = MathTool.GetRandom(100) < 50 ? PlayerManaTypes.Lp : PlayerManaTypes.Mp;
                AddManaData(get,1);
                manas.Add(get);
            }

            var lm =BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(killerId);
            if (lm != null)
                BattleManager.Instance.FlowWordQueue.Add(new FlowManaInfo(manas, lm.Position, 0, 40));
        }

        public float GetRoundRate()
        {
            return (float)(recoverTime / (GameConstants.DrawManaTime * EnergyGenerator.GainEpRate));
        }

        public virtual void InitialCards()
        {
            DrawNextNCard(null, GameConstants.BattleInitialCardCount, AddCardReasons.InitCard);

            BattleManager.Instance.RuleData.CheckInitialCards(this);
        }
        
        public void AddMana(IMonster mon, int type, double addon)
        {
            AddManaData((PlayerManaTypes)type, (int)addon);

            if (addon > 0)
            {
                List<PlayerManaTypes> addonList = new List<PlayerManaTypes>();
                for (int i = 0; i < addon; i++)
                    addonList.Add((PlayerManaTypes)type);
                BattleManager.Instance.FlowWordQueue.Add(new FlowManaInfo(addonList, mon.Position, 0, 40));
            }
        }

        public void AddMp(double addon)
        {
            AddManaData(PlayerManaTypes.Mp, (int) addon);
        }

        public void AddLp(double addon)
        {
            AddManaData(PlayerManaTypes.Lp, (int)addon);
        }

        public void AddPp(double addon)
        {
            AddManaData(PlayerManaTypes.Pp, (int)addon);
        }

        public void AddLuk(IMonster mon, int addon)
        {//可能是负数
            Luk += addon;
            if (mon != null)
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowLukInfo(addon, mon.Position, 20, 50));
            }
        }

        private void AddManaData(PlayerManaTypes type, int num)
        {
            if (type == PlayerManaTypes.Mp || type == PlayerManaTypes.All)
                Mp = Math.Min(Mp + num, EnergyGenerator.LimitMp);
            if (type == PlayerManaTypes.Lp || type == PlayerManaTypes.All)
                Lp = Math.Min(Lp + num, EnergyGenerator.LimitLp);
            if (type == PlayerManaTypes.Pp || type == PlayerManaTypes.All)
                Pp = Math.Min(Pp + num, EnergyGenerator.LimitPp);

            if (ManaChanged != null) //todo 先ws下
                ManaChanged();
        }

        public int CheckUseCard(ActiveCard selectCard, Player left, Player right)
        {
            if (Mp < selectCard.Mp)
                return ErrorConfig.Indexer.BattleLackMp;

            if (Lp < selectCard.Lp)
                return ErrorConfig.Indexer.BattleLackLp;

            if (Pp < selectCard.Pp)
                return ErrorConfig.Indexer.BattleLackPp;

            if (CardConfigManager.GetCardConfig(selectCard.CardId).IsHeroCard && heroSkillCd > 0)
                return ErrorConfig.Indexer.BattleHeroSkillInCd;

            return ErrorConfig.Indexer.OK;
        }

        public bool BeforeUseCard(ActiveCard selectCard, Point location)
        {
            AddMp(-selectCard.Mp);
            AddLp(-selectCard.Lp);
            AddPp(-selectCard.Pp);

            BattleManager.Instance.EventMsgQueue.Pubscribe(EventMsgQueue.EventMsgTypes.UseCard, this, null, null, null, location, selectCard.CardId, (int)selectCard.CardType, selectCard.Level);

            SpikeManager.OnUseCard(selectCard.CardType);
            if (OnUseCard != null)
                OnUseCard(selectCard.CardId, selectCard.Level, IsLeft);
            
            return true;
        }

        public void AfterUseCard(ActiveCard selectCard)
        {
            var oldComboTime = comboTime;
            comboTime = 1;
            if (oldComboTime <= 0)
                HandCards.UpdateCardView();
            NLog.Debug("AfterUseCard pid={0} cid={1}", PeopleId, selectCard.CardId);
            var cardConfig = CardConfigManager.GetCardConfig(selectCard.CardId);
            if (cardConfig.IsHeroCard) //成功使用英雄技能
            {
                heroSkillCd = 1;
                if (HeroSkillChanged != null)
                    HeroSkillChanged(false);
            }
            else
            {
                if(cardConfig.Type == CardTypes.Monster)
                    BattleManager.Instance.StatisticData.GetPlayer(IsLeft).MonsterUsed++;
                else if (cardConfig.Type == CardTypes.Weapon)
                    BattleManager.Instance.StatisticData.GetPlayer(IsLeft).WeaponUsed++;
                else if (cardConfig.Type == CardTypes.Spell)
                {
                    BattleManager.Instance.StatisticData.GetPlayer(IsLeft).SpellUsed++;
                    if (!CardConfigManager.GetCardConfig(selectCard.CardId).IsSpecial)
                        OffCards.AddGrave(selectCard); //放入坟场
                }
                HandCards.DeleteCardAt(SelectId); //可能会造成意外的情况
            }
        }

        public virtual void AddResource(GameResourceType type, int number)
        {
        }

        public void OnTowerHited(double towerHpRate)
        {
            if (AIContext != null)
                AIContext.OnTowerHited(towerHpRate);
        }

        public void OnMonsterDie(int monsterId, byte monsterLevel, bool isSummoned)
        {
            if (!isSummoned && !CardConfigManager.GetCardConfig(monsterId).IsSpecial)
                OffCards.AddGrave(new ActiveCard(monsterId, monsterLevel)); //放入坟场
        }

        public virtual void OnKillMonster(int id, int dieLevel, int dieStar, Point position)
        {
            BattleManager.Instance.StatisticData.GetPlayer(IsLeft).Kill++;
            if (OnKillEnemy != null)
                OnKillEnemy(id, dieLevel, IsLeft);
        }
        
        public void UseMonster(ActiveCard card, Point location)
        {
            if (!BeforeUseCard(card, location))
                return;

            try
            {
                var mon = new Monster(card.CardId);
                mon.UpgradeToLevel(card.Level);
                LiveMonster newMon = new LiveMonster(card.Level, mon, location, IsLeft);
                BattleManager.Instance.MonsterQueue.Add(newMon);
                NLog.Debug("UseMonster pid={0} cid={1}", PeopleId, card.CardId);

                BattleManager.Instance.EventMsgQueue.Pubscribe(EventMsgQueue.EventMsgTypes.Summon, this, newMon, null, null, Point.Empty, 0,0,0);
                if (HolyBook.HasWord("holyman"))
                    newMon.BuffManager.AddBuff(BuffConfig.Indexer.HolyShield, 1, 99);
                if (mon.Luk != 0)
                    AddLuk(newMon, mon.Luk);
            }
            catch (Exception e)
            {
                NLog.Warn(e);
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("未知错误", location, 0, "Red", 26, 0, 0, 2, 15));
                return;
            }
            AfterUseCard(card);
        }

        public void UseWeapon(LiveMonster lm, ActiveCard card, Point p)
        {
            if (!BeforeUseCard(card, lm == null ? Point.Empty : lm.Position))
                return;

            try
            {
                var weaponConfig = ConfigData.GetWeaponConfig(card.CardId);
                if (!string.IsNullOrEmpty(weaponConfig.RelicType))
                {
                    BattleManager.Instance.RelicHolder.AddRelic(this, weaponConfig.Id, card.Level, weaponConfig.Dura);
                    BattleManager.Instance.EffectQueue.Add(new StaticUIEffect(EffectBook.GetEffect("blueflash"), new Point(p.X-50,p.Y-50), new Size(100,100)));
                }
                else
                {
                    Weapon wpn = new Weapon(card.CardId);
                    wpn.UpgradeToLevel(card.Level);

                    var tWeapon = new TrueWeapon(lm, card.Level, wpn);
                    lm.AddWeapon(tWeapon, true);

                    BattleManager.Instance.EffectQueue.Add(new MonsterBindEffect(EffectBook.GetEffect("equip"), lm, true));

                    if (wpn.Luk != 0)
                        AddLuk(lm, wpn.Luk);
                }
            }
            catch (Exception e)
            {
                NLog.Warn(e);
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("未知错误", lm.Position, 0, "Red", 26, 0, 0, 2, 15));
                return;
            }
            AfterUseCard(card);
        }

        public void UseSideKick(LiveMonster lm, ActiveCard card)
        {
            if (!BeforeUseCard(card, lm.Position))
                return;

            try
            {
                Monster mon = new Monster(card.CardId);
                mon.UpgradeToLevel(card.Level);

                var tWeapon = new SideKickWeapon(lm, card.Level, mon);
                lm.AddWeapon(tWeapon, false);
            }
            catch (Exception e)
            {
                NLog.Warn(e);
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("未知错误", lm.Position, 0, "Red", 26, 0, 0, 2, 15));
                return;
            }
            AfterUseCard(card);
        }

        public bool CanSpell(LiveMonster target, ActiveCard card)
        {
            Spell spell = new Spell(card.CardId);
            spell.Addon = SpecialAttr.SpellEffectAddon;
            spell.UpgradeToLevel(card.Level);
            return SpellAssistant.CanCast(this, spell, target);
        }

        public void DoSpell(LiveMonster target, ActiveCard card, Point location)
        {
            if (!BeforeUseCard(card, location))
                return;

            try
            {
                Spell spell = new Spell(card.CardId);
                spell.Addon = SpecialAttr.SpellEffectAddon;
                spell.UpgradeToLevel(card.Level);

                SpellAssistant.Cast(spell, IsLeft, target, location);

                if (SpikeManager.HasSpike("mirrorspell"))
                    Rival.Action.AddCard(null, card.CardId, card.Level);
            }
            catch (Exception e)
            {
                NLog.Warn(card.CardId + " " + e.ToString());
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("未知错误", location, 0, "Red", 26, 0, 0, 2, 15));
                return;
            }
            lastSpellId = card.CardId;
            HandCards.UpdateCardView();
            AfterUseCard(card);
        }

        public void AddCardReason(IMonster mon, AddCardReasons reason)
        {
            string effName = "";
            switch (reason)
            {
                case AddCardReasons.DrawCardBySkillOrSpell:
                    effName = "flycard";
                    break;
                case AddCardReasons.GetCertainCard:
                    goto case AddCardReasons.DrawCardBySkillOrSpell;
                case AddCardReasons.Discover:
                    effName = "flycard2";
                    break;
                case AddCardReasons.RandomCard:
                    effName = "flycard3";
                    break;
            }

            if (IsLeft && !string.IsNullOrEmpty(effName))
            {
                Point startPoint = new Point(BattleManager.Instance.MemMap.StageWidth / 2, BattleManager.Instance.MemMap.StageHeight / 2);
                if (mon != null)
                    startPoint = mon.Position;
                BattleManager.Instance.EffectQueue.Add(new MovingUIEffect(EffectBook.GetEffect(effName), startPoint, 
                    new Point(BattleManager.Instance.MemMap.StageWidth / 2, BattleManager.Instance.MemMap.StageHeight), 16, true));
            }
        }

        public void DrawNextNCard(IMonster mon, int n, AddCardReasons reason)
        {
            if(n <= 0)
                return;

            var cardCount = OffCards.LeftCount;
            for (int i = 0; i < n; i++)
                HandCards.GetNextCard();

            AddCardReason(mon, reason);

            if (CardLeftChanged != null && cardCount != OffCards.LeftCount)
                CardLeftChanged();
        }

        public void DiscoverCard(IMonster mon, int[] cardId, int lv, DiscoverCardActionType type)
        {
            if (AIContext == null)
            {
                CardSelectMethodDiscover discover = new CardSelectMethodDiscover(cardId, lv, type);
                if (OnShowCardSelector != null)
                    OnShowCardSelector(this, discover);
            }
            else
            {
                AIContext.Discover(mon, cardId, lv, type);
            }
        }

        public void AddDiscoverCard(IMonster mon, int cardId, int level, DiscoverCardActionType type)
        {
            switch (type)
            {
                case DiscoverCardActionType.AddCard: HandCards.AddCard(cardId, level, 0); break;
                case DiscoverCardActionType.Add2Cards: HandCards.AddCard(cardId, level, 0); HandCards.AddCard(cardId, level, 0); break;
            }
            AddCardReason(mon, AddCardReasons.Discover);
        }

        public void AddTowerHp(int hp)
        {
            var towerUnit = BattleManager.Instance.MonsterQueue.GetKingTower(IsLeft);
            towerUnit.AddHp(hp);
        }

        public void OnGetCardFail(bool noCard)
        {
            if (noCardOutPunish)
                return;

            BattleManager.Instance.EffectQueue.Add(new MonsterBindEffect(EffectBook.GetEffect("longly"), Tower as LiveMonster, true));
            Tower.OnMagicDamage(null, Tower.MaxHp / 10, (int)CardElements.None);
            BattleManager.Instance.FlowWordQueue.Add(new FlowErrInfo(noCard ? ErrorConfig.Indexer.CardOutPunish :
                ErrorConfig.Indexer.CardFullPunish, Tower.Position, 0, 3));
        }

        public virtual List<int> GetInitialMonster()
        {
            return new List<int>();
        }
    }
}
