using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMissile;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players.Frag;
using TaleofMonsters.Core.Interface;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.Core;
using System.Drawing;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Controler.Battle.Data.MemSpell;
using NarlonLib.Log;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Battle.Data.MemWeapon;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Cards.Spells;
using TaleofMonsters.DataType.Cards.Weapons;
using TaleofMonsters.DataType.Effects;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class Player : IPlayer
    {
        public delegate void PlayerPointEventHandler();
        public event PlayerPointEventHandler ManaChanged;

        public delegate void PlayerHeroSkillStateEventHandler(bool active);
        public event PlayerHeroSkillStateEventHandler HeroSkillChanged;

        private float recoverTime;

        public EnergyGenerator EnergyGenerator { get; set; }
        public SpikeManager SpikeManager { get; set; }
        public CardManager CardManager { get; private set; }

        public double SpellEffectAddon { get; set; }//法术牌效果加成

        private List<Trap> trapList = new List<Trap>();
        private bool isPlayerControl; //是否玩家控制

        public List<int> HeroSkillList = new List<int>();
        public bool IsAlive { get; set; }//是否活着
        public int[] InitialMonster { get; set; } //由peoplebook决定

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

        public PlayerState State { get; protected set; }

        public int PeopleId { get; set; }

        public ActiveCards Cards { get; protected set; }//自己搭配的卡组

        public int DirectDamage { get; set; }

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
            State = new PlayerState();
        }

        protected void InitBase()
        {
            Lp = 3;
            Mp = 3;
            Pp = 3;
            EnergyGenerator.Next(0);
        }

        public void Update(bool isFast, float pastRound, int round)
        {
            recoverTime += pastRound * GameConstants.RoundRecoverAddon * ((round >= GameConstants.RoundRecoverDoubleRound) ? 2 : 1);
            var need = isFast ? GameConstants.DrawManaTimeFast : GameConstants.DrawManaTime;
            if (recoverTime >= need)
            {
                recoverTime -= need;
                AddManaData(EnergyGenerator.NextAimMana, 1);
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
                var get = MathTool.GetRandom(100) < 50 ? PlayerManaTypes.LeaderShip : PlayerManaTypes.Mana;
                AddManaData(get,1);
                manas.Add(get);
            }

            var lm =BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(killerId);
            if (lm != null)
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowManaInfo(manas, lm.Position, 0, 40), true);
            }
        }

        public float GetRoundRate()
        {
            return recoverTime / GameConstants.DrawManaTime;
        }

        public virtual void InitialCards()
        {
            for (int i = 0; i < GameConstants.BattleInitialCardCount; i++)
            {
                CardManager.GetNextCard();
            }
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
                BattleManager.Instance.FlowWordQueue.Add(new FlowManaInfo(addonList, mon.Position, 0, 40), true);
            }
        }

        public void AddMp(double addon)
        {
            AddManaData(PlayerManaTypes.Mana, (int) addon);
        }

        public void AddLp(double addon)
        {
            AddManaData(PlayerManaTypes.LeaderShip, (int)addon);
        }

        public void AddPp(double addon)
        {
            AddManaData(PlayerManaTypes.Power, (int)addon);
        }

        private void AddManaData(PlayerManaTypes type, int num)
        {
            if (type == PlayerManaTypes.Mana || type == PlayerManaTypes.All)
            {
                Mp = Math.Min(Mp + num, EnergyGenerator.LimitMp);
            }
            if (type == PlayerManaTypes.LeaderShip || type == PlayerManaTypes.All)
            {
                Lp = Math.Min(Lp + num, EnergyGenerator.LimitLp);
            }
            if (type == PlayerManaTypes.Power || type == PlayerManaTypes.All)
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
            if (rival.CheckTrapOnUseCard(selectCard, location, rival, this))
            {
                return false;
            }

            SpikeManager.OnUseCard(selectCard.CardType);
            
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

        public void AddResource(int type, int number)
        {
            AddResource((GameResourceType)type, number);
        }

        public virtual void OnKillMonster(int dieLevel, int dieStar, Point position)
        {

        }

        public void AddMonster(int cardId, int level,Point location)
        {
            int size = BattleManager.Instance.MemMap.CardSize;
            var truePos = new Point(location.X/size*size, location.Y/size*size);
            var mon = new Monster(cardId);
            mon.UpgradeToLevel(level);
            LiveMonster newMon = new LiveMonster(level, mon, truePos, IsLeft);
            BattleManager.Instance.MonsterQueue.Add(newMon);
        }

        public void ExchangeMonster(IMonster target, int lv)
        {
            if ((target as LiveMonster).Avatar.MonsterConfig.IsBuilding)
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("抵抗", target.Position, 0, "Gold", 26, 0, 0, 1, 15), false);
                return;
            }

            target.Transform(MonsterBook.GetRandMonsterId());
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
                    BattleManager.Instance.BattleInfo.GetPlayer(IsLeft).MonsterAdd++;

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
                rival.CheckTrapOnSummon(newMon, rival, this);
            }
            catch (Exception e)
            {
                NLog.Warn(e);
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("未知错误", location, 0, "Red", 26, 0, 0, 2, 15), false);
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
                    BattleManager.Instance.BattleInfo.GetPlayer(IsLeft).WeaponAdd++;

                var tWeapon = new TrueWeapon(lm, card.Level, wpn);
                lm.AddWeapon(tWeapon);
            }
            catch (Exception e)
            {
                NLog.Warn(e);
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("未知错误", lm.Position, 0, "Red", 26, 0, 0, 2, 15), false);
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
                    BattleManager.Instance.BattleInfo.GetPlayer(IsLeft).MonsterAdd++;

                var tWeapon = new SideKickWeapon(lm, card.Level, mon);
                lm.AddWeapon(tWeapon);
            }
            catch (Exception e)
            {
                NLog.Warn(e);
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("未知错误", lm.Position, 0, "Red", 26, 0, 0, 2, 15), false);
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
                spell.Addon = SpellEffectAddon;
                spell.UpgradeToLevel(card.Level);
                if (!card.IsHeroSkill)
                    BattleManager.Instance.BattleInfo.GetPlayer(IsLeft).SpellAdd++;

                SpellAssistant.CheckSpellEffect(spell, IsLeft, target, location);

                if (SpikeManager.HasSpike("mirrorspell"))
                {
                    Rival.AddCard(null, card.CardId, card.Level);
                }
            }
            catch (Exception e)
            {
                NLog.Warn(e);
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("未知错误", location, 0, "Red", 26, 0, 0, 2, 15),false);
                return;
            }
            AfterUseCard(card);
            CardManager.DeleteCardAt(SelectId);
        }

        #region 陷阱
        public void AddTrap(int id, int lv, double rate, int damage, double help)
        {
            trapList.Add(new Trap {Id = id, Level = lv, Rate = rate, Damage = damage, Help = help});
        }

        private void RemoveTrap(int id)
        {
            trapList.RemoveAll(s => s.Id == id);
        }

        private bool CheckTrapOnUseCard(ActiveCard selectCard, Point location, IPlayer left, IPlayer right)
        {
            foreach (var trap in trapList)
            {
                var trapConfig = ConfigData.GetSpellTrapConfig(trap.Id);
                if (trapConfig.EffectUse != null)
                {
                    if (MathTool.GetRandom(100) < trap.Rate && trapConfig.EffectUse(left, right, trap, selectCard.CardId, (int)selectCard.CardType))
                    {
                        RemoveTrap(trap.Id);
                        NLog.Debug(string.Format("RemoveTrap UseCard id={0} cardId={1}", trap.Id, selectCard.CardId));
                        BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(trapConfig.UnitEffect), location, false));

                        return true;
                    }
                }
            }

            return false;
        }

        private void CheckTrapOnSummon(IMonster mon, IPlayer left, IPlayer right)
        {
            foreach (var trap in trapList)
            {
                var trapConfig = ConfigData.GetSpellTrapConfig(trap.Id);
                if (trapConfig.EffectSummon != null)
                {
                    if (MathTool.GetRandom(100) < trap.Rate && trapConfig.EffectSummon(left, right, trap, mon, trap.Level))
                    {
                        RemoveTrap(trap.Id);
                        NLog.Debug(string.Format("RemoveTrap Summon id={0} cardId={1}", trap.Id, mon.Id));
                        BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(trapConfig.UnitEffect), mon as LiveMonster, false));
                        return;
                    }
                }
            }
        }

        #endregion

        public void AddSpellMissile(IMonster target, ISpell spell, Point mouse, string effect)
        {
            BasicMissileControler controler = new SpellTraceMissileControler((LiveMonster)target, spell);
            Missile mi = new Missile(effect, mouse.X, mouse.Y, controler);
            BattleManager.Instance.MissileQueue.Add(mi);
        }

        public void AddSpellRowMissile(ISpell spell, int count, Point mouse, string effect)
        {
            int size = BattleManager.Instance.MemMap.CardSize;
            int ybase = mouse.Y/size*size;
            int xstart = IsLeft ? 0 : BattleManager.Instance.MemMap.StageWidth - size;
            for (int i = -count/2; i <= count/2; i++)
            {
                int yoff = ybase + i*size;
                int xend = IsLeft ? xstart + spell.Range / 10 * size : xstart - spell.Range / 10 * size;
                BasicMissileControler controler = new SpellLandMissileControler(this, new Point(xend, yoff), spell);
                Missile mi = new Missile(effect, xstart, yoff, controler);
                BattleManager.Instance.MissileQueue.Add(mi);
            }
        }

        public void DeleteRandomCardFor(IPlayer p, int levelChange)
        {
            CardManager.DeleteRandomCardFor(p, levelChange);
        }

        public void CopyRandomCardFor(IPlayer p, int levelChange)
        {
            CardManager.CopyRandomCardFor(p, levelChange);
        }

        public void AddCard(IMonster mon, int cardId, int level)
        {
            CardManager.AddCard(cardId, level, 0);
        }
        public void AddCard(IMonster mon, int cardId, int level, int modify)
        {
            CardManager.AddCard(cardId, level, modify);
        }

        public void GetNextNCard(IMonster mon, int n)
        {
            if (IsLeft)
            {
                Point startPoint = new Point(BattleManager.Instance.MemMap.StageWidth / 2, BattleManager.Instance.MemMap.StageHeight / 2);
                if (mon != null)
                    startPoint = mon.Position;
                BattleManager.Instance.EffectQueue.Add(new UIEffect(EffectBook.GetEffect("flycard"), startPoint, new Point(BattleManager.Instance.MemMap.StageWidth / 2, BattleManager.Instance.MemMap.StageHeight), 16, true));
            }
            CardManager.GetNextNCard(n);
        }

        public void CopyRandomNCard(int n, int spellid)
        {
            CardManager.CopyRandomNCard(n ,spellid);
        }

        public void DeleteAllCard()
        {
           CardManager.DeleteAllCard();
        }

        public void DeleteSelectCard()
        {
            CardManager.DeleteCardAt(SelectId);
            CardsDesk.DisSelectCard();
        }

        public void RecostSelectCard()
        {
            var selectCard = CardManager.GetDeckCardAt(SelectId);
            if (selectCard == null)
            {
                NLog.Error(string.Format("RecostSelectCard id={0} not Found", SelectId));
                return;
            }
            AddMp(-selectCard.Mp);
            AddLp(-selectCard.Lp);
            AddPp(-selectCard.Pp);
        }

        public int CardNumber
        {
            get
            {
                return CardManager.GetCardNumber();
            }
        }

        public void ConvertCard(int count, int cardId, int levelChange)
        {
            CardManager.ConvertCard(count, cardId, levelChange);
        }

        public void CardLevelUp(int n, int type)
        {
            CardManager.CardLevelUp(n, type);
        }

        public void AddRandomCard(IMonster mon, int type, int lv)
        {
            int cardId = 0;
            switch (type)
            {
                case 1: cardId = MonsterBook.GetRandMonsterId();break;
                case 2: cardId = WeaponBook.GetRandWeaponId(); break;
                case 3: cardId = SpellBook.GetRandSpellId(); break;
            }
            AddCard(mon, cardId, lv);
        }

        public void AddRandomCardJob(IMonster mon, int job, int lv)
        {
            var cardId = CardConfigManager.GetRandomJobCard(job);
            if (cardId != 0)
            {
                AddCard(mon, cardId, lv);
            }
        }

        public void AddRandomCardRace(IMonster mon, int race, int lv)
        {
            var cardId = CardConfigManager.GetRandomRaceCard(race);
            if (cardId != 0)
            {
                AddCard(mon, cardId, lv);
            }
        }

        public void AddSpellEffect(double rate)
        {
            SpellEffectAddon += rate;
        }

        public void AddSpike(int id)
        {
            SpikeManager.AddSpike(id);
        }

        public void RemoveSpike(int id)
        {
            SpikeManager.RemoveSpike(id);
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

        public void DrawToolTips(Graphics g)
        {
            int x = 0, y = 0;
            var img = GetPlayerImage();
            if (!IsLeft) //右边那人
                x = 899 - img.Width;
         
            g.DrawImage(img, x, y, img.Width, img.Height);
            img.Dispose();
        }

        private Image GetPlayerImage()
        {
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(string.Format("Lv{0}", Level), "LightBlue", 20);

            if (trapList.Count>0)
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
            
            var rival = Rival as Player;
            if (rival.HasHolyWord("witcheye"))
            {
                tipData.AddLine();
                tipData.AddTextNewLine("手牌", "White");
                for (int i = 0; i < 10; i++)
                {
                    var card = CardManager.GetDeckCardAt(i);
                    if (card.CardId > 0)
                    {
                        var cardConfig = CardConfigManager.GetCardConfig(card.CardId);
                        tipData.AddTextNewLine("-", "White");
                        tipData.AddImage(CardAssistant.GetCardImage(card.CardId, 20, 20));
                        tipData.AddText(string.Format("{0}({1}★)Lv{2}", cardConfig.Name, cardConfig.Star, card.Level), HSTypes.I2QualityColor(cardConfig.Quality));
                    }
                }
            }
            return tipData.Image;
        }
    }
}
