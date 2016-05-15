using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMissile;
using TaleofMonsters.Controler.Battle.Data.MemMonster.Component;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.CardPieces;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Equips.Addons;
using TaleofMonsters.DataType.Skills;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Core;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemWeapon;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Decks;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster
{
    internal class LiveMonster : IMonster
    {
        private readonly MonsterAi aiController;
        private readonly HpBar hpBar;
        public SkillManager SkillManager { get; private set; }
        public BuffManager BuffManager { get; private set; }

        private int life;
        private int oldLife;
        private int lastDamagerId;

        private List<MonsterAuro> auroList;//光环
        private int[] antiMagic;//魔法抗性
        private int roundPast; //经过的调用数，每次调用大约200ms
        private float pastRoundTotal; //消耗的时间片，累计到1清理一次

        #region 属性

        public int Id { get; private set; }
        public int Level { get; private set; }
        public Monster Avatar { get; private set; }
        public int HpReg { get; set; }
        public int GhostTime { get; set; }
        public Point Position { get; set; }
        public bool IsHero { get { return Avatar.MonsterConfig.Type == (int) CardTypeSub.Hero; } }
        public IMap Map { get { return BattleManager.Instance.MemMap; } }
        public bool IsLeft { get; set; }
        public int Action { get; set; }
        public IBattleWeapon Weapon { get; set; }
        
        public int AttackType { get; set; }//只有武器可以改变，技能不行
        public AttrModifyData Atk { get; set; }
        public AttrModifyData MaxHp { get; set; }

        public AttrModifyData Def { get; set; }
        public AttrModifyData Mag { get; set; }
        public AttrModifyData Spd { get; set; }
        public AttrModifyData Hit { get; set; }
        public AttrModifyData Dhit { get; set; }
        public AttrModifyData Crt { get; set; }
        public AttrModifyData Luk { get; set; }


        public bool IsSummoned { get; set; } //是否召唤单位

        public int Life
        {
            get { return Math.Max(life, 0); }
            set { life = value; if (life > RealMaxHp) life = RealMaxHp;
            hpBar.Rate = life * 100 / RealMaxHp;                
            }
        }

        public int LossLife
        {
            get { int value = oldLife - life; oldLife = life; return value; }
        }
        
        public Point CenterPosition
        {
            get { return new Point(Position.X + BattleManager.Instance.MemMap.CardSize / 2, 
                Position.Y + BattleManager.Instance.MemMap.CardSize / 2); }
        }

        public bool IsAlive
        {
            get { return life > 0; }
        }

        public int RealAtk
        {
            get
            {
                double diff = (Atk.Source + Atk.Adder) * (1 + Atk.Multiter) - Atk.Source;
                return Math.Max((int)(Atk.Source + diff), 0);
            }
        }

        public int RealMaxHp
        {
            get
            {
                double diff = (MaxHp.Source + MaxHp.Adder) * (1 + MaxHp.Multiter) - MaxHp.Source;
                return Math.Max((int)(MaxHp.Source + diff), 0);
            }
        }

        public int RealDef
        {
            get
            {
                double diff = (Def.Source + Def.Adder) * (1 + Def.Multiter) - Def.Source;
                return (int)(Def.Source + diff);
            }
        }

        public int RealMag
        {
            get
            {
                double diff = (Mag.Source + Mag.Adder) * (1 + Mag.Multiter) - Mag.Source;
                return (int)(Mag.Source + diff);
            }
        }

        public int RealSpd
        {
            get
            {
                double diff = (Spd.Source + Spd.Adder) * (1 + Spd.Multiter) - Spd.Source;
                return (int)(Spd.Source + diff);
            }
        }

        public int RealHit
        {
            get
            {
                double diff = (Hit.Source + Hit.Adder) * (1 + Hit.Multiter) - Hit.Source;
                return (int)(Hit.Source + diff);
            }
        }

        public int RealDHit
        {
            get
            {
                double diff = (Dhit.Source + Dhit.Adder) * (1 + Dhit.Multiter) - Dhit.Source;
                return (int)(Dhit.Source + diff);
            }
        }

        public int RealCrt
        {
            get
            {
                double diff = (Crt.Source + Crt.Adder) * (1 + Crt.Multiter) - Crt.Source;
                return (int)(Crt.Source + diff);
            }
        }

        public int RealLuk
        {
            get
            {
                double diff = (Luk.Source + Luk.Adder) * (1 + Luk.Multiter) - Luk.Source;
                return (int)(Luk.Source + diff);
            }
        }

        public string Arrow
        {
            get
            {
                if (Weapon == null || Weapon.Arrow == "null")
                    return Avatar.MonsterConfig.Arrow;
                return Weapon.Arrow;
            }
        }

        public int RealRange
        {
            get
            {
                if (Weapon == null || Weapon.Range == 0)
                    return Avatar.Range;
                return Weapon.Range;
            }
        }

        public int ReadMov
        {
            get
            {
                if (Weapon == null || Weapon.Mov == 0)
                    return Avatar.Mov;
                return Weapon.Mov;
            }
        }

        public Player OwnerPlayer
        {
            get { return IsLeft ? BattleManager.Instance.PlayerManager.LeftPlayer : BattleManager.Instance.PlayerManager.RightPlayer; }
        }

        public IPlayer Owner
        {
            get { return IsLeft ? BattleManager.Instance.PlayerManager.LeftPlayer : BattleManager.Instance.PlayerManager.RightPlayer; }
        }

        public IPlayer Rival
        {
            get { return IsLeft ? BattleManager.Instance.PlayerManager.RightPlayer : BattleManager.Instance.PlayerManager.LeftPlayer; }
        }

        public bool IsGhost
        {
            get { return GhostTime > 0; }
        }

        #endregion

        public LiveMonster(int id, int level, Monster mon, Point point, bool isLeft)
        {
            hpBar = new HpBar();

            Id = id;
            Level = level;
            Avatar = mon;
            if (Avatar.MonsterConfig.Type != (int)CardTypeSub.Hero)
                Avatar.UpgradeToLevel(level);          

            Position = point;
            IsLeft = isLeft;
            Action = 0;
            roundPast = 0;
            SkillManager = new SkillManager(this);
            AttackType = (int)CardElements.None;
            CanAttack = true;
            
            BuffManager = new BuffManager(this);
            aiController = new MonsterAi(this);
            LiveMonsterToolTip = new LiveMonsterToolTip(this);

            SetBasicData();
            Life = Avatar.Hp;
            oldLife = life;
            MonsterCoverBox = new MonsterCoverBox(this);
        }

        private void SetBasicData()
        {
            BuffManager.Reload();
            auroList = new List<MonsterAuro>();
            SkillManager.Reload();
            
            antiMagic = new int[6];//6个属性

            Atk = new AttrModifyData(Avatar.Atk);
            MaxHp = new AttrModifyData(Avatar.Hp);
            Def = new AttrModifyData(Avatar.Def);
            Mag = new AttrModifyData(Avatar.Mag);
            Spd = new AttrModifyData(Avatar.Spd);
            Hit = new AttrModifyData(Avatar.Hit);
            Dhit = new AttrModifyData(Avatar.Dhit);
            Crt = new AttrModifyData(Avatar.Crt);
            Luk = new AttrModifyData(Avatar.Luk);

            if (Avatar.MonsterConfig.Type != (int)CardTypeSub.Hero)
                EAddonBook.UpdateMonsterData(this, OwnerPlayer.State.Monsterskills.Keys(), OwnerPlayer.State.Monsterskills.Values());
        }

        public bool BeHited(LiveMonster src)
        {
            int hitrate = SkillAssistant.GetHit(src, this);
            if (hitrate > MathTool.GetRandom(100))
            {
                HitDamage damage = SkillAssistant.GetDamage(src, this);
                CheckDamageBuffEffect(src, damage);
                Life -= damage.Value;    
                SkillAssistant.CheckHitEffectAfter(src, this, damage);
                if (src.WeaponId > 0)
                {
                    src.Weapon.OnHit();
                }
                if (WeaponId > 0)
                {
                    Weapon.OnHited();
                }
                if (damage.Value > 0)
                {
                    BuffManager.CheckRecoverOnHit();
                    lastDamagerId = src.Id;
                }
                return true;
            }
            return false;
        }

        public void OnDie()
        {
            GhostTime = 1;//一回合ghost
            BattleManager.Instance.MemMap.GetMouseCell(Position.X,Position.Y).UpdateOwner(-Id);
            if (Avatar.MonsterConfig.Type == (int)CardTypeSub.Hero)
            {
                if (!IsLeft)
                {
                    if (Rival is HumanPlayer)
                    {
                        UserProfile.Profile.OnKillMonster(Avatar.MonsterConfig.Star, Avatar.MonsterConfig.Type, Avatar.MonsterConfig.Type);
                    }
                }
                BattleManager.Instance.BattleInfo.GetPlayer(!IsLeft).HeroKill++;
                OwnerPlayer.IsAlive = false;
            }
            else
            {
                if (!IsLeft)
                {
                    if (Rival is HumanPlayer)
                    {
                        int itemId = CardPieceBook.CheckPiece(Avatar.Id);
                        if (itemId > 0)
                        {
                            BattleManager.Instance.BattleInfo.AddItemGet(itemId);
                            UserProfile.InfoBag.AddItem(itemId, 1);
                            BattleManager.Instance.FlowWordQueue.Add(new FlowItemInfo(itemId, Position, 20, 50), true);
                        }
                        UserProfile.Profile.OnKillMonster(Avatar.MonsterConfig.Star, Avatar.MonsterConfig.Type, Avatar.MonsterConfig.Type);
                    }
                }
                BattleManager.Instance.BattleInfo.GetPlayer(!IsLeft).Kill++;
            }

            SkillManager.CheckRemoveEffect();
            if (lastDamagerId != 0)
            {
                var rival = Rival as Player;
                rival.OnKillMonster(lastDamagerId, Level, Avatar.MonsterConfig.Star, Position);
            }
        }

        public void Next(float pastRound, bool tileMatching)//附带判断地形因素
        {
            roundPast++;
            pastRoundTotal += pastRound;

            hpBar.Update();
            SkillAssistant.CheckAuroState(this, tileMatching);

            if (pastRoundTotal >= 1)
            {
                pastRoundTotal -= 1;
                if (HpReg > 0)
                {
                    Life += HpReg;
                }

                if (Avatar.MonsterConfig.LifeRound > 0)
                {//这里使用默认的生命值来扣
                    Life -= (int)(Avatar.Hp / Avatar.MonsterConfig.LifeRound);
                }
            }

            BuffManager.BuffCount();

            if (SkillManager.CheckSpecial(pastRound))
            {
                return;//特殊技能触发
            }

            aiController.CheckAction();
        }

        public bool AddAts()
        {
            if (BuffManager.HasBuff(BuffEffectTypes.NoAction))
                return false;
            Action += GameConstants.RoundAts;//200ms + 30
            if (Action >= GameConstants.LimitAts)
            {
                Action = Action - GameConstants.LimitAts;
                return true;
            }
            return false;
        }

        public void HitTarget(LiveMonster target, bool isMelee)
        {
            if (target != null)
            {
                SkillAssistant.CheckBurst(this, target, isMelee);
                bool isMiss = !target.BeHited(this);
                if (isMiss)
                    BattleManager.Instance.FlowWordQueue.Add(new FlowWord("Miss!", new Point(Position.X + 40, Position.Y + 40), 0, "red", -10, 0), false);
            }
        }

        public void CheckMagicDamage(HitDamage damage)
        {
            if (damage.Element>0&&antiMagic[damage.Element-1]>0)
            {
                damage.SetDamage(DamageTypes.Magic,Math.Max(damage.Value*(100 - antiMagic[damage.Element - 1])/100,0));
            }
        }

        public bool CanAddWeapon()
        {
            return !Avatar.MonsterConfig.IsBuilding && !IsGhost;
        }
        
        public void AddWeapon(IBattleWeapon tw)
        {
            if (Avatar.MonsterConfig.IsBuilding)
            {
                NLog.Warn(string.Format("AddWeapon to building {0}", Avatar.Id));
                return;
            }

            if (Weapon != null)
                Weapon.CheckWeaponEffect(this, -1);
            Weapon = tw;
            //  EAddonBook.UpdateWeaponData(Weapon, OwnerPlayer.State.Weaponskills.Keys(), OwnerPlayer.State.Weaponskills.Values());
            Weapon.CheckWeaponEffect(this, 1);
        }

        public void DeleteWeapon()
        {
            if (Weapon != null)
            {
                Weapon.CheckWeaponEffect(this, -1);
                Weapon = null;
            }
        }

        private void CheckDamageBuffEffect(LiveMonster src, HitDamage dam)
        {
            if (dam.Dtype == DamageTypes.Physical)
            {
                if (BuffManager.HasBuff(BuffEffectTypes.Chaos) && MathTool.GetRandom(100) < 25)
                {
                    src.Life -= dam.Value;
                    dam.SetDamage(DamageTypes.Physical, 0);
                }
            }

            if (BuffManager.HasBuff(BuffEffectTypes.Shield))
            {
                dam.SetDamage(DamageTypes.All, 1);
            }
        }

        public void CheckAuroEffect()
        {
            foreach (var auro in auroList)
            {
                auro.CheckAuroState();
            }
        }

        public void Revive()
        {
            GhostTime = 0;
            BattleManager.Instance.MemMap.GetMouseCell(Position.X, Position.Y).UpdateOwner(Id);
            Life++;

            SkillManager.CheckInitialEffect();
        }

		public void AddStrengthLevel(int value)
		{
			int basedata = value * MathTool.GetSqrtMulti10(Avatar.MonsterConfig.Star);
		    Atk += (double) basedata/10;
//		    Def += (double) basedata/10;
//			MaxHp=Avatar.Hp + 3 * basedata / 10;
		}

        public void DrawOnBattle(Graphics g2, Color uponColor)
        {
            Bitmap image = new Bitmap(100, 100);
            Graphics g = Graphics.FromImage(image);

            if (!IsGhost)
            {
                var monImg = MonsterBook.GetMonsterImage(Avatar.Id, 100, 100);
                if (monImg != null)
                {
                    g.DrawImage(monImg, 0, 0, 100, 100);
                }
              
                if (uponColor != Color.White)
                {
                    SolidBrush brush = new SolidBrush(Color.FromArgb(150, uponColor));
                    g.FillRectangle(brush, 0, 0, 100, 100);
                    brush.Dispose();
                }
                var pen = new Pen(!IsLeft ? Brushes.Blue : Brushes.Red, 3);
                Font font2 = new Font("Arial", 14*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                Font fontLevel = new Font("Arial", 20*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                g.DrawRectangle(pen, 1, 1, 98, 98);
                pen.Dispose();

                hpBar.Draw(g);

                g.FillPie(Brushes.Gray, 65, 65, 30, 30, 0, 360);
                g.FillPie(CanAttack? Brushes.Yellow:Brushes.LightGray, 65, 65, 30, 30, 0, Action*360 / GameConstants.LimitAts);

                var starIcon = HSIcons.GetIconsByEName("sysstar");
                for (int i = 0; i < Avatar.MonsterConfig.Star; i++)
                {
                    g.DrawImage(starIcon, i*12, 8, 16, 16);
                }

                g.DrawString(Level.ToString(), fontLevel, Brushes.Wheat, Level < 10 ? 71 : 67, 68);
                g.DrawString(Level.ToString(), fontLevel, Brushes.DarkBlue, Level < 10 ? 70 : 66, 67);
                font2.Dispose();
                fontLevel.Dispose();

                if (Weapon != null)
                {
                    g.DrawImage(Weapon.GetImage(32, 32), 5, 60, 32, 32);
                    g.DrawRectangle(Pens.Lime, 5, 60, 32, 32);
                }
                BuffManager.DrawBuff(g, roundPast / 20);
            }
            else
            {
                Image img = PicLoader.Read("System", "Rip.PNG");
                g.DrawImage(img, 19, 11, 63, 78);
                img.Dispose();

                g.FillRectangle(Brushes.Red, 0, 2, 100, 5);
                g.FillRectangle(Brushes.Cyan, 0, 2, Math.Min(GhostTime, 100), 5);
            }

            g.Dispose();
            int size = BattleManager.Instance.MemMap.CardSize;
            g2.DrawImage(image, new Rectangle(Position.X, Position.Y, size, size), 0, 0, 100, 100, GraphicsUnit.Pixel);
            image.Dispose();
        }

        #region IMonster 成员

        public int CardId
        {
            get { return Avatar.Id; }
        }

        public int WeaponId
        {
            get { return Weapon == null ? 0 : Weapon.CardId; }
        }

        public void AddHp(double addon)
        {
            Life += (int)addon;
        }

        public void AddHpRate(double value)
        {
            Life += (int)(RealMaxHp * value);
        }

        public IMonsterAuro AddAuro(int buff, int lv, string tar)
        {
            var auro = new MonsterAuro(this, buff, lv, tar);
            auroList.Add(auro);
            return auro;
        }

        public void AddAntiMagic(string type, int value)
        {
            if (type=="All")
            {
                for (int i = 0; i < antiMagic.Length; i++)
                {
                    antiMagic[i] += value;
                }
            }
            else
            {
                antiMagic[(int)Enum.Parse(typeof(CardElements), type) - 1] += value;    
            }            
        }

        public void AddItem(int itemId)
        {
            if (OwnerPlayer is HumanPlayer)
            {
                BattleManager.Instance.BattleInfo.AddItemGet(itemId);
                UserProfile.InfoBag.AddItem(itemId, 1);
                BattleManager.Instance.FlowWordQueue.Add(new FlowItemInfo(itemId, Position, 20, 50), true);
            }
        }

        public void Transform(int monId)
        {
            if (IsHero)
            {
                NLog.Warn("hero cannot be Transform");
                return;
            }

            int cardId = Weapon == null ? 0 : Weapon.CardId;
            var savedWeapon = Weapon == null ? null : Weapon.GetCopy();
            DeleteWeapon();
            int lifp = Life * 100 / Avatar.Hp;
            MonsterCoverBox.RemoveAllCover();
            SkillManager.CheckRemoveEffect();
            OwnerPlayer.State.CheckMonsterEvent(false, Avatar);
            Avatar = new Monster(monId);
            Avatar.UpgradeToLevel(Level);
            OwnerPlayer.State.CheckMonsterEvent(true, Avatar);
            SetBasicData();
            MonsterCoverBox.CheckCover();
            SkillManager.CheckInitialEffect();
            if (cardId > 0)
            {
                AddWeapon(savedWeapon);
            }
            Life = Avatar.Hp * lifp / 100;
        }

        public void AddActionRate(double value)
        {
            Action += (int)(GameConstants.LimitAts*value);
        }

        public void StealWeapon(IMonster target)
        {
            var monster = target as LiveMonster;
            if (monster != null)
            {
                var weapon = monster.Weapon;
                AddWeapon(weapon);
                target.BreakWeapon();
            }
        }

        public void BreakWeapon()
        {
            if (Weapon != null)
                DeleteWeapon();
        }

        public void WeaponReturn()
        {
            if (Weapon != null && Weapon is TrueWeapon)
            {
                ActiveCard card = new ActiveCard(new DeckCard(Weapon.CardId, (byte)Weapon.Level, 0));
                OwnerPlayer.CardManager.AddCard(card);
                DeleteWeapon();
            }
        }

        public void AddRandSkill()
        {
            SkillManager.AddSkill(SkillBook.GetRandSkillId(), Level / 2, 100, SkillSourceTypes.Skill);
        }

        public int GetMonsterCountByRace(int rid)
        {
            return OwnerPlayer.State.GetMonsterCountByType((MonsterCountTypes)(rid + 20));
        }

        public int GetMonsterCountByType(int type)
        {
            return OwnerPlayer.State.GetMonsterCountByType((MonsterCountTypes)(type+10));
        }

        public void AddMissile(IMonster target, string arrow)
        {
            BasicMissileControler controler = new TraceMissileControler(this, target as LiveMonster);
            Missile mi = new Missile(arrow, Position.X, Position.Y, controler);
            BattleManager.Instance.MissileQueue.Add(mi);
        }

        public int Star
        {
            get { return Avatar.MonsterConfig.Star; }
        }

        public double HpRate
        {
            get { return (double)life*100/Avatar.Hp; }
        }

        public int Hp
        {
            get { return Life; }
        }

        public bool IsTileMatching
        {
            get { return BuffManager.IsTileMatching; }
        }

        public bool CanAttack { get; set; }

        public bool CanMove
        {
            get { return !BuffManager.HasBuff(BuffEffectTypes.NoMove); }
        }

        public bool IsElement(string ele)
        {
            return (int) Enum.Parse(typeof (CardElements), ele) == Avatar.MonsterConfig.Type;
        }

        public bool IsRace(string rac)
        {
            return (int)Enum.Parse(typeof(CardTypeSub), rac) == Avatar.MonsterConfig.Type;
        }

        public bool IsNight
        {
            get { return BattleManager.Instance.IsNight; }
        }

        public bool HasScroll
        {
            get
            {
                if (Weapon != null)
                {
                    return Weapon.Type == CardTypeSub.Scroll;
                }
                return false;
            }
        }

        public bool HasSkill(int sid)
        {
            return SkillManager.HasSkill(sid);
        }

        //遗忘所有武器技能除外
        public void ForgetSkill()
        {
            SkillManager.Forget();
        }

        public int CardNumber
        {
            get { return OwnerPlayer.GetCardNumber(); }
        }


        public int Attr { get { return Avatar.MonsterConfig.Attr; } }

        public int Type { get { return Avatar.MonsterConfig.Type; } }

        public int SkillParm { get; set; }

        public LiveMonsterToolTip LiveMonsterToolTip { get; private set; }

        public MonsterCoverBox MonsterCoverBox { get; private set; }

        public void AddBuff(int buffId, int blevel, double dura)
        {
            BuffManager.AddBuff(buffId, blevel, dura);
        }

        public void ClearDebuff()
        {
            BuffManager.ClearDebuff();
        }

        public void ExtendDebuff(double count)
        {
            BuffManager.ExtendDebuff(count);
        }

        public bool HasBuff(int buffid)
        {
            return BuffManager.HasBuff(buffid);
        }

        public void SetToPosition(string type)
        {
            Point dest = BattleLocationManager.GetMonsterNearPoint(Position, type, !IsLeft);
            if (dest.X != -1 && dest.Y != -1)
            {
                if (!Avatar.MonsterConfig.IsBuilding)
                {
                    BattleLocationManager.SetToPosition(this, dest);
                }
            }
        }

        public void OnMagicDamage(double damage, int element)
        {
            Life -= SkillAssistant.GetMagicDamage(this, new HitDamage((int)damage, (int)damage, element, DamageTypes.Magic));
        }

        public void SuddenDeath()
        {
            Life = 0;
        }

        public void Rebel()
        {
            if (!Avatar.MonsterConfig.IsBuilding)//建筑无法策反
            {
                IsLeft = !IsLeft;
            }
        }

        public void AddMaxHp(double value)
        {
            MaxHp.Source += value;
            if (value > 0)
            {
                AddHp(value);//顺便把hp也加上
            }
        }

        public void Summon(int type, int id)
        {
            if (IsSummoned)
            {//召唤生物不能继续召唤，防止无限循环
                return;
            }

            List<Point> posLis = new List<Point>();
            if (type == 1)//上下各一个
            {
                int size = BattleManager.Instance.MemMap.CardSize;
                posLis.Add(new Point(Position.X, Position.Y - size));
                posLis.Add(new Point(Position.X, Position.Y + size));
            }
            else if (type == 2)//4格随机位置
            {
                posLis.Add(BattleLocationManager.GetMonsterNearPoint(Position, "around", IsLeft));
            }
            else if (type == 3)//后面
            {
                posLis.Add(BattleLocationManager.GetMonsterNearPoint(Position, "come", IsLeft));
            }
            else if (type == 4)//前面
            {
                posLis.Add(BattleLocationManager.GetMonsterNearPoint(Position, "back", IsLeft));
            }
            else if (type == 5)//4格位置
            {
                int size = BattleManager.Instance.MemMap.CardSize;
                posLis.Add(new Point(Position.X, Position.Y - size));
                posLis.Add(new Point(Position.X, Position.Y + size));
                posLis.Add(new Point(Position.X - size, Position.Y));
                posLis.Add(new Point(Position.X + size, Position.Y));
            }

            foreach (var pos in posLis)
            {
                if (BattleManager.Instance.MemMap.IsMousePositionCanSummon(pos.X, pos.Y))
                {
                    var mon = new Monster(id);
                    mon.UpgradeToLevel(Level);
                    LiveMonster newMon = new LiveMonster(World.WorldInfoManager.GetCardFakeId(), Level, mon, pos, IsLeft);
                    newMon.IsSummoned = true;
                    BattleManager.Instance.MonsterQueue.AddDelay(newMon);
                }
            }
        }

        public void MadDrug()
        {
            if (!Avatar.MonsterConfig.IsBuilding && RealRange > 0)
            {
                Atk.Source = MaxHp.Source/5;
            }
        }

        #endregion
    }
}
