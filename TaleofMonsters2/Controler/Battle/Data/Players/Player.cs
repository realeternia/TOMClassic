using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
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
using TaleofMonsters.DataType.Cards.Spells;
using TaleofMonsters.DataType.Cards.Weapons;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class Player : IPlayer
    {
        public delegate void PlayerPointEventHandler();
        public event PlayerPointEventHandler ManaChanged;

        private float recoverTime;

        public EnergyGenerator EnergyGenerator { get; set; }
        public SpikeManager SpikeManager { get; set; }
        public CardManager CardManager { get; private set; }

        public double SpellEffectAddon { get; set; }//法术牌效果加成

        private List<Trap> trapList = new List<Trap>();
        private bool isPlayerControl; //是否玩家控制

        public List<int> HeroSkillList = new List<int>();
        public bool IsAlive { get; set; }//是否活着

        #region 属性

        public bool IsLeft { get; private set; }

        public float Mp { get; set; }

        public float Lp { get; set; }

        public float Pp { get; set; }

        public int Level { get; protected set; }

        public int Job { get; protected set; }

        public ICardList CardsDesk { get; set; }

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


        public void AddSpike(int id)
        {
            SpikeManager.AddSpike(id);
        }

        public void RemoveSpike(int id)
        {
            SpikeManager.RemoveSpike(id);
        }

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
            {
                return HSErrorTypes.BattleLackMp;
            }

            if (Lp < selectCard.Lp)
            {
                return HSErrorTypes.BattleLackLp;
            }

            if (Pp < selectCard.Pp)
            {
                return HSErrorTypes.BattleLackPp;
            }

            return HSErrorTypes.OK;
        }

        public bool CheckUseCard(ActiveCard selectCard)
        {
            AddMp(-selectCard.Mp);
            AddLp(-selectCard.Lp);
            AddPp(-selectCard.Pp);

            var rival = IsLeft ? BattleManager.Instance.PlayerManager.RightPlayer : BattleManager.Instance.PlayerManager.LeftPlayer;
            if (rival.CheckTrapOnUseCard(selectCard, rival, this))
            {
                return false;
            }

            SpikeManager.OnUseCard(selectCard.CardType);

            return true;
        }

        public virtual void AddResource(GameResourceType type, int number)
        {
        }

        public void AddResource(int type, int number)
        {
            AddResource((GameResourceType)type, number);
        }

        public virtual void OnKillMonster(int killerId, int dieLevel, int dieStar, Point position)
        {

        }

        public void AddMonster(int cardId, int level,Point location)
        {
            int size = BattleManager.Instance.MemMap.CardSize;
            var truePos = new Point(location.X/size*size, location.Y/size*size);
            var mon = new Monster(cardId);
            mon.UpgradeToLevel(level);
            LiveMonster newMon = new LiveMonster(World.WorldInfoManager.GetCardFakeId(), level, mon, truePos, IsLeft);
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
            if (!CheckUseCard(card))
            {
                return;
            }

            try
            {
                var mon = new Monster(card.CardId);
                mon.UpgradeToLevel(card.Level);
                BattleManager.Instance.BattleInfo.GetPlayer(IsLeft).MonsterAdd++;

                LiveMonster newMon = new LiveMonster(card.Id, card.Level, mon, location, IsLeft);
                BattleManager.Instance.MonsterQueue.Add(newMon);

                var rival = IsLeft ? BattleManager.Instance.PlayerManager.RightPlayer : BattleManager.Instance.PlayerManager.LeftPlayer;
                rival.CheckTrapOnSummon(newMon, rival, this);
            }
            catch (Exception e)
            {
                NLog.Warn(e);
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("未知错误", location, 0, "Red", 26, 0, 0, 2, 15), false);
                return;
            }

            CardManager.DeleteCardAt(SelectId);
        }

        public void UseWeapon(LiveMonster lm, ActiveCard card)
        {
            if (!CheckUseCard(card))
            {
                return;
            }

            try
            {
                Weapon wpn = new Weapon(card.CardId);
                wpn.UpgradeToLevel(card.Level);
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

            CardManager.DeleteCardAt(SelectId);
        }

        public void UseSideKick(LiveMonster lm, ActiveCard card)
        {
            if (!CheckUseCard(card))
            {
                return;
            }

            try
            {
                Monster mon = new Monster(card.CardId);
                mon.UpgradeToLevel(card.Level);
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

            CardManager.DeleteCardAt(SelectId);
        }

        public void DoSpell(LiveMonster target, ActiveCard card, Point location)
        {
            if (!CheckUseCard(card))
            {
                return;
            }

            try
            {
                Spell spell = new Spell(card.CardId);
                spell.Addon = SpellEffectAddon;
                spell.UpgradeToLevel(card.Level);
                BattleManager.Instance.BattleInfo.GetPlayer(IsLeft).SpellAdd++;

                SpellAssistant.CheckSpellEffect(spell, IsLeft, target, location);

                if (SpikeManager.HasSpike("mirrorspell"))
                {
                    var rival = IsLeft ? BattleManager.Instance.PlayerManager.RightPlayer : BattleManager.Instance.PlayerManager.LeftPlayer;
                    rival.AddCard(card.CardId, card.Level);
                }
            }
            catch (Exception e)
            {
                NLog.Warn(e);
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("未知错误", location, 0, "Red", 26, 0, 0, 2, 15),false);
                return;
            }
            
            CardManager.DeleteCardAt(SelectId);
        }

        #region 陷阱
        public void AddTrap(int id, int lv, double rate, int damage)
        {
            trapList.Add(new Trap {Id = id, Level = lv, Rate = rate, Damage = damage});
        }

        private void RemoveTrap(int id)
        {
            trapList.RemoveAll(s => s.Id == id);
        }

        private bool CheckTrapOnUseCard(ActiveCard selectCard, Player left, Player right)
        {
            foreach (var trap in trapList)
            {
                var effect = ConfigData.GetSpellTrapConfig(trap.Id).EffectUse;
                if (effect != null)
                {
                    if (MathTool.GetRandom(100) < trap.Rate && effect(left, right, trap, selectCard.CardId, selectCard.Id, (int)selectCard.CardType))
                    {
                        RemoveTrap(trap.Id);
                        NLog.Debug(string.Format("RemoveTrap UseCard id={0} cardId={1}", trap.Id, selectCard.CardId));

                        return true;
                    }
                }
            }

            return false;
        }

        private void CheckTrapOnSummon(LiveMonster mon, Player left, Player right)
        {
            foreach (var trap in trapList)
            {
                var effect = ConfigData.GetSpellTrapConfig(trap.Id).EffectSummon;
                if (effect != null)
                {
                    if (MathTool.GetRandom(100) < trap.Rate && effect(left, right, trap, mon, trap.Level))
                    {
                        RemoveTrap(trap.Id);
                        NLog.Debug(string.Format("RemoveTrap Summon id={0} cardId={1}", trap.Id, mon.Id));

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

        public void AddCard(int cardId, int level)
        {
            CardManager.AddCard(cardId, level);
        }

        public void GetNextNCard(int n)
        {
          //  BattleManager.Instance.EffectQueue.Add(new FlyEffect(EffectBook.GetEffect("yellowflash"), pos, new Point(300, 300), 20, true));
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

        public void AddRandomCard(int type, int lv)
        {
            int cardId = 0;
            switch (type)
            {
                case 1: cardId = MonsterBook.GetRandMonsterId();break;
                case 2: cardId = WeaponBook.GetRandWeaponId(); break;
                case 3: cardId = SpellBook.GetRandSpellId(); break;
            }
            AddCard(cardId, lv);
        }

        public void AddRandomCardJob(int job, int lv)
        {
            var cardId = CardConfigManager.GetRandomJobCard(job);
            if (cardId != 0)
            {
                AddCard(cardId, lv);
            }
        }

        public void AddSpellEffect(double rate)
        {
            SpellEffectAddon += rate;
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
            tipData.AddLine();
            if (trapList.Count>0)
            {
                tipData.AddTextNewLine("陷阱", "White");
                tipData.AddLine();
            }
            foreach (var trap in trapList)
            {
                var trapConfig = ConfigData.GetSpellTrapConfig(trap.Id);
                if (isPlayerControl)
                {
                    tipData.AddTextNewLine(trapConfig.Name, "Lime");
                    tipData.AddText(string.Format("Lv{0} {1:0.0}%", trap.Level,trap.Rate), "White");
                }
                else
                {
                    tipData.AddTextNewLine("???", "Red");
                }
            }
            return tipData.Image;
        }
    }
}
