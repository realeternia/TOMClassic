using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players.Frag;
using TaleofMonsters.Core.Interface;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.Core;
using System.Drawing;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Controler.Battle.Data.MemSpell;
using NarlonLib.Log;
using TaleofMonsters.Controler.Battle.Components.CardSelect;
using TaleofMonsters.Controler.Battle.Data.MemWeapon;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Cards.Spells;
using TaleofMonsters.DataType.Cards.Weapons;
using TaleofMonsters.DataType.Effects;
using TaleofMonsters.DataType.Equips;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class Player : IPlayer
    {
        public delegate void PlayerPointEventHandler();
        public event PlayerPointEventHandler ManaChanged;
        public event PlayerPointEventHandler CardLeftChanged;
        public event PlayerPointEventHandler TrapStateChanged;

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
        public CardManager CardManager { get; private set; }
        public TrapHolder TrapHolder { get; private set; }
        public EquipModifier Modifier { get; protected set; }
        public IPlayerAction Action { get; private set; }
        public PlayerSpecialAttr SpecialAttr { get; private set; }

        private bool isPlayerControl; //是否玩家控制

        public List<int> HeroSkillList = new List<int>();
        public bool IsAlive { get; set; }//是否活着

        private float comboTime;//>0表示在combo状态
        public bool Combo { get { return comboTime > 0; } }

        #region 属性

        public bool IsLeft { get; private set; }

        public float Mp { get; set; }

        public float Lp { get; set; }

        public float Pp { get; set; }

        public int Level { get; protected set; }

        public int Job { get; protected set; }

        public ICardList CardsDesk { get; set; }

        public IMonster Tower
        {
            get { return BattleManager.Instance.MonsterQueue.GetKingTower(IsLeft); }
        }

        public IPlayer Rival
        {
            get { return IsLeft ? BattleManager.Instance.PlayerManager.RightPlayer : BattleManager.Instance.PlayerManager.LeftPlayer; }
        }
        public int SelectCardId
        {
            get { return CardsDesk.GetSelectCard().CardId; }
        }

        public int SelectId
        {
            get { return CardsDesk.GetSelectId(); }
        }

        public int PeopleId { get; set; }

        public ActiveCards Cards { get; protected set; }//自己搭配的卡组

        public int CardNumber { get { return CardManager.GetCardNumber(); } }

        private List<string> holyWordList = new List<string>(); //圣言，一些特殊效果的指令
        private List<int[]> monsterAddonOnce = new List<int[]>(); //一次性的强化

        #endregion

        public Player(bool playerControl, bool isLeft)
        {
            IsLeft = isLeft;
            IsAlive = true;
            isPlayerControl = playerControl;
            CardManager = new CardManager(this);
            EnergyGenerator = new EnergyGenerator();
            SpikeManager = new SpikeManager(this);
            TrapHolder = new TrapHolder(this);
            Modifier = new EquipModifier();
            Action = new PlayerAction(this);
            SpecialAttr = new PlayerSpecialAttr();
            Lp = 3;
            Mp = 3;
            Pp = 3;
        }

        protected void CalculateEquipAndSkill(List<int> equipids, int[] energyData)
        {
            var jobConfig = ConfigData.GetJobConfig(Job);
            if (jobConfig.SkillId > 0)
            {
                HeroSkillList.Add(jobConfig.SkillId);//添加职业技能
            }

            var equipList = equipids.ConvertAll(equipId => new Equip(equipId));
            List<int> monsterBoostItemList = new List<int>();
            foreach (var equip in equipList)
            {
                EquipConfig equipConfig = ConfigData.GetEquipConfig(equip.TemplateId);
                for (int i = 0; i < 3; i++)
                {
                    energyData[i] += equipConfig.EnergyRate[i];
                }

                if (equipConfig.HeroSkillId > 0)
                {
                    HeroSkillList.Add(equipConfig.HeroSkillId); //添加装备附带的技能
                }

                if (equipConfig.PickMethod != null)
                {
                    monsterBoostItemList.Add(equip.TemplateId);
                }

                if (equipConfig.Position == (int)EquipTypes.Core)
                {
                    Modifier.CoreId = equipConfig.Id;
                }
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
            }

            var addon = EquipBook.GetVirtualEquips(equipList);
            Modifier.UpdateInfo(addon, monsterBoostItemList);

            if (HeroSkillList.Count > 3)
            {
                HeroSkillList = HeroSkillList.GetRange(HeroSkillList.Count - 3, 3); //最多保留3个技能
            }
        }

        public void Update(bool isFast, float pastRound, int round)
        {
            recoverTime += pastRound * GameConstants.RoundRecoverAddon * (round >= GameConstants.RoundRecoverDoubleRound ? 2 : 1);
            var need = isFast ? GameConstants.DrawManaTimeFast : GameConstants.DrawManaTime;
            if (recoverTime >= need)
            {
                recoverTime -= need;
                AddManaData(EnergyGenerator.NextAimMana, 1);
                EnergyGenerator.UseMana();
                EnergyGenerator.Next(round);
            }
            if (ManaChanged != null)//todo 先ws下
            {
                ManaChanged();
            }
            SpikeManager.OnRound(pastRound);
            comboTime -= pastRound;
            if (comboTime<=0)
            {
                comboTime = 0;
                CardManager.UpdateCardCombo();
            }
            if (CardManager.HeroSkillCd > 0)
            {
                CardManager.HeroSkillCd -= pastRound;
                if (HeroSkillChanged != null && CardManager.HeroSkillCd <= 0)
                    HeroSkillChanged(true);
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
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowManaInfo(manas, lm.Position, 0, 40));
            }
        }

        public float GetRoundRate()
        {
            return recoverTime / GameConstants.DrawManaTime;
        }

        public virtual void InitialCards()
        {
            DrawNextNCard(null, GameConstants.BattleInitialCardCount, Frag.AddCardReason.InitCard);

            BattleManager.Instance.RuleData.CheckInitialCards(this);
        }
        
        public void AddMana(IMonster mon, int type, double addon)
        {
            AddManaData((PlayerManaTypes)type, (int)addon);

            if (addon > 0)
            {
                List<PlayerManaTypes> addonList = new List<PlayerManaTypes>();
                for (int i = 0; i < addon; i++)
                {
                    addonList.Add((PlayerManaTypes)type);
                }
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

        private void AddManaData(PlayerManaTypes type, int num)
        {
            if (type == PlayerManaTypes.Mp || type == PlayerManaTypes.All)
            {
                Mp = Math.Min(Mp + num, EnergyGenerator.LimitMp);
            }
            if (type == PlayerManaTypes.Lp || type == PlayerManaTypes.All)
            {
                Lp = Math.Min(Lp + num, EnergyGenerator.LimitLp);
            }
            if (type == PlayerManaTypes.Pp || type == PlayerManaTypes.All)
            {
                Pp = Math.Min(Pp + num, EnergyGenerator.LimitPp);
            }

            if (ManaChanged != null) //todo 先ws下
            {
                ManaChanged();
            }
        }

        public int CheckUseCard(ActiveCard selectCard, Player left, Player right)
        {
            if (Mp < selectCard.Mp)
                return HSErrorTypes.BattleLackMp;

            if (Lp < selectCard.Lp)
                return HSErrorTypes.BattleLackLp;

            if (Pp < selectCard.Pp)
                return HSErrorTypes.BattleLackPp;

            if (selectCard.IsHeroSkill && CardManager.HeroSkillCd > 0)
                return HSErrorTypes.BattleHeroSkillInCd;

            return HSErrorTypes.OK;
        }

        public bool BeforeUseCard(ActiveCard selectCard, Point location)
        {
            AddMp(-selectCard.Mp);
            AddLp(-selectCard.Lp);
            AddPp(-selectCard.Pp);

            var rival = Rival as Player;
            if (rival.TrapHolder.CheckTrapOnUseCard(selectCard, location, rival))
            {
                return false;
            }

            SpikeManager.OnUseCard(selectCard.CardType);
            BattleManager.Instance.MonsterQueue.OnPlayerUseCard(this, (int)selectCard.CardType, selectCard.Level);
            if (OnUseCard != null)
                OnUseCard(selectCard.CardId, selectCard.Level, IsLeft);
            
            return true;
        }

        public void AfterUseCard(ActiveCard selectCard)
        {
            var oldComboTime = comboTime;
            comboTime = 1;
            if (oldComboTime <= 0)
                CardManager.UpdateCardCombo();

            if (selectCard.IsHeroSkill) //成功使用英雄技能
            {
                CardManager.HeroSkillCd = 1;
                if (HeroSkillChanged != null)
                    HeroSkillChanged(false);
            }
        }

        public virtual void AddResource(GameResourceType type, int number)
        {
        }

        public void OnKillMonster(int id, int dieLevel, int dieStar, Point position)
        {
            if (OnKillEnemy != null)
            {
                OnKillEnemy(id, dieLevel, IsLeft);
            }
        }
        
        public void UseMonster(ActiveCard card, Point location)
        {
            if (!BeforeUseCard(card, location))
            {
                return;
            }

            try
            {
                var mon = new Monster(card.CardId);
                mon.UpgradeToLevel(card.Level);
                if (!card.IsHeroSkill)
                    BattleManager.Instance.StatisticData.GetPlayer(IsLeft).MonsterAdd++;

                LiveMonster newMon = new LiveMonster(card.Level, mon, location, IsLeft);
                BattleManager.Instance.MonsterQueue.Add(newMon);

                var addon = GetAllMonsterAddonAndClear();//这个属性目前可以来自药水
                if (addon.Length > 0)
                {
                    foreach (var add in addon)
                        if (add.Length > 1)
                            newMon.AddBasicData(add[0], add[1]);
                }

                var rival = Rival as Player;
                rival.TrapHolder.CheckTrapOnSummon(newMon, rival);
            }
            catch (Exception e)
            {
                NLog.Warn(e);
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("未知错误", location, 0, "Red", 26, 0, 0, 2, 15));
                return;
            }
            AfterUseCard(card);
            CardManager.DeleteCardAt(SelectId);
        }

        public void UseWeapon(LiveMonster lm, ActiveCard card)
        {
            if (!BeforeUseCard(card, lm.Position))
            {
                return;
            }

            try
            {
                Weapon wpn = new Weapon(card.CardId);
                wpn.UpgradeToLevel(card.Level);
                if (!card.IsHeroSkill)
                    BattleManager.Instance.StatisticData.GetPlayer(IsLeft).WeaponAdd++;

                var tWeapon = new TrueWeapon(lm, card.Level, wpn);
                lm.AddWeapon(tWeapon);
            }
            catch (Exception e)
            {
                NLog.Warn(e);
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("未知错误", lm.Position, 0, "Red", 26, 0, 0, 2, 15));
                return;
            }
            AfterUseCard(card);
            CardManager.DeleteCardAt(SelectId);
        }

        public void UseSideKick(LiveMonster lm, ActiveCard card)
        {
            if (!BeforeUseCard(card, lm.Position))
            {
                return;
            }

            try
            {
                Monster mon = new Monster(card.CardId);
                mon.UpgradeToLevel(card.Level);
                if (!card.IsHeroSkill)
                    BattleManager.Instance.StatisticData.GetPlayer(IsLeft).MonsterAdd++;

                var tWeapon = new SideKickWeapon(lm, card.Level, mon);
                lm.AddWeapon(tWeapon);
            }
            catch (Exception e)
            {
                NLog.Warn(e);
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("未知错误", lm.Position, 0, "Red", 26, 0, 0, 2, 15));
                return;
            }
            AfterUseCard(card);
            CardManager.DeleteCardAt(SelectId);
        }

        public void DoSpell(LiveMonster target, ActiveCard card, Point location)
        {
            if (!BeforeUseCard(card, location))
            {
                return;
            }

            try
            {
                Spell spell = new Spell(card.CardId);
                spell.Addon = SpecialAttr.SpellEffectAddon;
                spell.UpgradeToLevel(card.Level);
                if (!card.IsHeroSkill)
                    BattleManager.Instance.StatisticData.GetPlayer(IsLeft).SpellAdd++;

                SpellAssistant.CheckSpellEffect(spell, IsLeft, target, location);

                if (SpikeManager.HasSpike("mirrorspell"))
                {
                    Rival.Action.AddCard(null, card.CardId, card.Level);
                }
            }
            catch (Exception e)
            {
                NLog.Warn(e);
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("未知错误", location, 0, "Red", 26, 0, 0, 2, 15));
                return;
            }
            AfterUseCard(card);
            CardManager.DeleteCardAt(SelectId);
        }

        public void AddCardReason(IMonster mon, AddCardReason reason)
        {
            string effName = "";
            switch (reason)
            {
                case Frag.AddCardReason.DrawCardBySkillOrSpell:
                    effName = "flycard";
                    break;
                case Frag.AddCardReason.GetCertainCard:
                    goto case Frag.AddCardReason.DrawCardBySkillOrSpell;
                case Frag.AddCardReason.Discover:
                    effName = "flycard2";
                    break;
                case Frag.AddCardReason.RandomCard:
                    effName = "flycard3";
                    break;
            }

            if (IsLeft && !string.IsNullOrEmpty(effName))
            {
                Point startPoint = new Point(BattleManager.Instance.MemMap.StageWidth / 2, BattleManager.Instance.MemMap.StageHeight / 2);
                if (mon != null)
                    startPoint = mon.Position;
                BattleManager.Instance.EffectQueue.Add(new UIEffect(EffectBook.GetEffect(effName), startPoint, 
                    new Point(BattleManager.Instance.MemMap.StageWidth / 2, BattleManager.Instance.MemMap.StageHeight), 16, true));
            }
        }

        public void DrawNextNCard(IMonster mon, int n, AddCardReason reason)
        {
            var cardCount = Cards.LeftCount;
            for (int i = 0; i < n; i++)
                CardManager.GetNextCard();

            AddCardReason(mon, reason);

            if (CardLeftChanged != null && cardCount != Cards.LeftCount)
            {
                CardLeftChanged();
            }
        }
        public void DiscoverCard(IMonster mon, int[] cardId, int lv, DiscoverCardActionType type)
        {
            if (isPlayerControl)
            {
                CardSelectMethodDiscover discover = new CardSelectMethodDiscover(cardId, lv, type);
                if (OnShowCardSelector != null)
                {
                    OnShowCardSelector(this, discover);
                }
            }
            else
            {
                AIStrategy.Discover(this, mon, cardId, lv, type);
            }
        }

        public void AddDiscoverCard(IMonster mon, int cardId, int level, DiscoverCardActionType type)
        {
            switch (type)
            {
                case DiscoverCardActionType.AddCard: CardManager.AddCard(cardId, level, 0); break;
                case DiscoverCardActionType.Add2Cards: CardManager.AddCard(cardId, level, 0); CardManager.AddCard(cardId, level, 0); break;
            }
            AddCardReason(mon, Frag.AddCardReason.Discover);
        }

        public void AddHolyWord(string word)
        {
            if (!holyWordList.Contains(word))
            {
                holyWordList.Add(word);
            }
        }

        public bool HasHolyWord(string word)
        {
            return holyWordList.Contains(word);
        }

        public void AddMonsterAddon(int[] addon)
        {
            monsterAddonOnce.Add(addon);
        }

        public int[][] GetAllMonsterAddonAndClear()
        {
            var data = monsterAddonOnce.ToArray();
            monsterAddonOnce.Clear();
            return data;
        }

        public void AddTowerHp(int hp)
        {
            var unit = BattleManager.Instance.MonsterQueue.GetKingTower(IsLeft);
            unit.AddHp(hp);
        }

        public void OnGetCardFail(bool noCard)
        {
            BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect("longly"), Tower as LiveMonster, true));
            Tower.OnMagicDamage(null, Tower.MaxHp.Source / 10, (int)CardElements.None);
            BattleManager.Instance.FlowWordQueue.Add(new FlowErrInfo(noCard ? HSErrorTypes.CardOutPunish :
                HSErrorTypes.CardFullPunish, Tower.Position, 0, 3));
        }

        public virtual List<int> GetInitialMonster()
        {
            return new List<int>();
        }

        public void OnTrapChange()
        {
            if (TrapStateChanged != null)
                TrapStateChanged();
        }
    }
}
