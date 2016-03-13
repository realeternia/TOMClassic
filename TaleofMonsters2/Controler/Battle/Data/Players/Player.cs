using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Core.Interface;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.Core;
using System.Drawing;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Controler.Battle.Data.MemSpell;
using NarlonLib.Log;
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

        public CardManager CardManager { get; private set; }

        private int lpCost;//额外的anger消耗
        private int mpCost;//额外的mana消耗
        private int ppCost;//额外的mana消耗
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

        public int LpCost
        {
            get { return lpCost; }
            set { lpCost = value;  CardManager.UpdateCardCost(); }
        }
        
        public int MpCost
        {
            get { return mpCost; }
            set { mpCost = value; CardManager.UpdateCardCost(); }
        }

        public int PpCost
        {
            get { return ppCost; }
            set { ppCost = value; CardManager.UpdateCardCost(); }
        }

        public int RoundCardPlus { get; set; }

        #endregion

        public Player(bool playerControl, bool isLeft)
        {
            IsLeft = isLeft;
            IsAlive = true;
            isPlayerControl = playerControl;
            CardManager = new CardManager(this);
            EnergyGenerator = new EnergyGenerator();
            State = new PlayerState();
        }

        protected void InitBase()
        {
            Lp = 1;
            Mp = 1;
            Pp = 1;
            EnergyGenerator.Next(0);
        }

        public void Update(bool isFast, float timePast, int round)
        {
            recoverTime += timePast * GameConstants.RoundRecoverAddon * ((round >= GameConstants.RoundRecoverDoubleRound) ? 2 : 1);
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

        public void AddHeroUnit()
        {
            int x = IsLeft ? 0 : BattleManager.Instance.MemMap.ColumnCount - 1;
            int y = BattleManager.Instance.MemMap.RowCount / 2;
            int size = BattleManager.Instance.MemMap.CardSize;

            var id = World.WorldInfoManager.GetCardFakeId();
            var heroData = new Monster(MonsterConfig.Indexer.KingTowerId);
            LiveMonster lm = new LiveMonster(id, heroData.Level, heroData, new Point(x*size,y*size), IsLeft);
            lm.CanAttack = false;
            BattleManager.Instance.MonsterQueue.Add(lm);

            id = World.WorldInfoManager.GetCardFakeId();
            var arrowData = new Monster(MonsterConfig.Indexer.ArrowTowerId);
            lm = new LiveMonster(id, heroData.Level, arrowData, new Point((IsLeft ? (x + 2) : (x - 2)) * size, (y - 3) * size), IsLeft);
            BattleManager.Instance.MonsterQueue.Add(lm);

            id = World.WorldInfoManager.GetCardFakeId();
            arrowData = new Monster(MonsterConfig.Indexer.ArrowTowerId);
            lm = new LiveMonster(id, heroData.Level, arrowData, new Point((IsLeft ? (x + 2) : (x - 2)) * size, (y + 3) * size), IsLeft);
            BattleManager.Instance.MonsterQueue.Add(lm);
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
                Mp = Math.Min(Mp + num, 10);
            }
            if (type == PlayerManaTypes.LeaderShip || type == PlayerManaTypes.All)
            {
                Lp = Math.Min(Lp + num, 10);
            }
            if (type == PlayerManaTypes.Power || type == PlayerManaTypes.All)
            {
                Pp = Math.Min(Pp + num, 10);
            }

            if (ManaChanged != null) //todo 先ws下
            {
                ManaChanged();
            }
        }

        public int CheckUseCard(ActiveCard selectCard, Player left, Player right)
        {
            if (selectCard.CardType == CardTypes.Spell && Mp < selectCard.Mp)
            {
                return HSErrorTypes.BattleLackMp;
            }

            if (selectCard.CardType == CardTypes.Monster && Lp < selectCard.Lp)
            {
                return HSErrorTypes.BattleLackLp;
            }

            if (selectCard.CardType == CardTypes.Weapon && Pp < selectCard.Pp)
            {
                return HSErrorTypes.BattleLackPp;
            }

            return HSErrorTypes.OK;
        }

        public void OnUseCard(ActiveCard selectCard)
        {
            AddMp(-selectCard.Mp);
            AddLp(-selectCard.Lp);
            AddPp(-selectCard.Pp);
        }

        public void OnSummon(Monster mon)
        {
            //todo 通过heroskillcard修改

            BattleManager.Instance.BattleInfo.GetPlayer(IsLeft).MonsterAdd++;
        }

        public void OnUseWeapon(Weapon wpn)
        {
            //todo 通过heroskillcard修改

            BattleManager.Instance.BattleInfo.GetPlayer(IsLeft).WeaponAdd++;
        }

        public void OnDoSpell(Spell spl)
        {
            //todo 通过heroskillcard修改

            BattleManager.Instance.BattleInfo.GetPlayer(IsLeft).SpellAdd++;
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

        #region 陷阱
        public void AddTrap(int id, int lv, double rate, int damage)
        {
            trapList.Add(new Trap {Id = id, Level = lv, Rate = rate, Damage = damage});
        }

        private void RemoveTrap(int id)
        {
            trapList.RemoveAll(s => s.Id == id);
        }

        public bool CheckTrapOnUseCard(ActiveCard selectCard, Player left, Player right)
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

        public void CheckTrapOnSummon(LiveMonster mon, Player left, Player right)
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

        public int GetCardNumber()
        {
            return CardManager.GetCardNumber();
        }

        public void ConvertCard(int count, int cardId, int levelChange)
        {
            CardManager.ConvertCard(count, cardId, levelChange);
        }

        public void CardLevelUp(int n, int type)
        {
            CardManager.CardLevelUp(n, type);
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
