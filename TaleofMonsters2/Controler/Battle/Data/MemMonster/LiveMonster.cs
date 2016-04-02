using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster.Component;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.CardPieces;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Cards.Weapons;
using TaleofMonsters.DataType.Effects;
using TaleofMonsters.DataType.Equips.Addons;
using TaleofMonsters.DataType.Skills;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Core;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType;

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
        public TrueWeapon TWeapon { get; set; }
        
        public bool IsMagicAtk { get; set; }//只有武器可以改变，技能不行
        public int AttackType { get; set; }//只有武器可以改变，技能不行
        public AttrModifyData Atk { get; set; }

        public int Life
        {
            get { return Math.Max(life, 0); }
            set { life = value; if (life > Avatar.Hp) life = Avatar.Hp;
            hpBar.Rate = life * 100 / Avatar.Hp;                
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
                if (SkillManager.HasSpecialMark(SkillMarks.AtkDefBonus))
                {
                    diff = diff * 3 / 2;
                }
                return Math.Max((int)(Atk.Source + diff), 0);
            }
        }

        public int RealDef
        {
            get
            {
                if (TWeapon.CardId == 0 || TWeapon.Avatar.Def == 0)
                    return Avatar.Def;
                return TWeapon.Avatar.Def + Avatar.Def;
            }
        }

        public int RealMag
        {
            get
            {
                if (TWeapon.CardId == 0 || TWeapon.Avatar.Mag == 0)
                    return Avatar.Mag;
                return TWeapon.Avatar.Mag + Avatar.Mag;
            }
        }

        public int RealLuk
        {
            get
            {
                if (TWeapon.CardId == 0 || TWeapon.Avatar.Luk == 0)
                    return Avatar.Luk;
                return TWeapon.Avatar.Luk + Avatar.Luk;
            }
        }

        public int RealSpd
        {
            get
            {
                if (TWeapon.CardId == 0 || TWeapon.Avatar.Spd == 0)
                    return Avatar.Spd;
                return TWeapon.Avatar.Spd + Avatar.Spd;
            }
        }

        public int RealHit
        {
            get
            {
                if (TWeapon.CardId == 0 || TWeapon.Avatar.Hit == 0)
                    return Avatar.Hit;
                return TWeapon.Avatar.Hit + Avatar.Hit;
            }
        }

        public int RealDHit
        {
            get
            {
                if (TWeapon.CardId == 0 || TWeapon.Avatar.Dhit == 0)
                    return Avatar.Dhit;
                return TWeapon.Avatar.Dhit + Avatar.Dhit;
            }
        }

        public int RealCrt
        {
            get
            {
                if (TWeapon.CardId == 0 || TWeapon.Avatar.Crt == 0)
                    return Avatar.Crt;
                return TWeapon.Avatar.Crt + Avatar.Crt;
            }
        }

        public string Arrow
        {
            get
            {
                if (TWeapon.CardId==0 || TWeapon.Avatar.WeaponConfig.Arrow == "null")
                    return Avatar.MonsterConfig.Arrow;
                return TWeapon.Avatar.WeaponConfig.Arrow;
            }
        }

        public int RealRange
        {
            get
            {
                if (TWeapon.CardId == 0 || TWeapon.Avatar.Range == 0)
                    return Avatar.Range;
                return TWeapon.Avatar.Range;
            }
        }

        public int ReadMov
        {
            get
            {
                if (TWeapon.CardId == 0 || TWeapon.Avatar.Mov == 0)
                    return Avatar.Mov;
                return TWeapon.Avatar.Mov;
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
            Life = Avatar.Hp;
            oldLife = life;
            Position = point;
            IsLeft = isLeft;
            Action = 0;
            roundPast = 0;
            SkillManager = new SkillManager(this);
            TWeapon = new TrueWeapon();
            AttackType = (int)CardElements.None;
            CanAttack = true;
            
            BuffManager = new BuffManager(this);
            aiController = new MonsterAi(this);
            LiveMonsterToolTip = new LiveMonsterToolTip(this);

            SetBasicData();
            MonsterCoverBox = new MonsterCoverBox(this);
        }

        private void SetBasicData()
        {
            BuffManager.Reload();
            auroList = new List<MonsterAuro>();
            SkillManager.Reload();
            
            antiMagic = new int[9];//8个属性，+1圣属性

            Atk = new AttrModifyData(Avatar.Atk);

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
                    src.TWeapon.OnHit();
                }
                if (WeaponId > 0)
                {
                    TWeapon.OnHited();
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
                        if (itemId == 0 && DropAdd)
                        {
                            itemId = CardPieceBook.CheckPiece(Avatar.Id);
                        }
                        if (itemId > 0)
                        {
                            BattleManager.Instance.BattleInfo.AddItemGet(itemId);
                            UserProfile.InfoBag.AddItem(itemId, 1);
                            BattleManager.Instance.FlowWordQueue.Add(new FlowItemInfo(itemId, Position, 20, 50), true);
                        }
                        UserProfile.Profile.OnKillMonster(Avatar.MonsterConfig.Star, Avatar.MonsterConfig.Type, Avatar.MonsterConfig.Type);
                    }
                }
                if (Avatar.Id == MonsterConfig.Indexer.ArrowTowerId)
                {
                    var tower = BattleManager.Instance.MonsterQueue.GetKingTower(IsLeft);
                    if (tower != null)
                    {
                        tower.CanAttack = true;
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

        public void Next(float pastRound, TileMatchResult tileMatching)//附带判断地形因素
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
                {
                    Life -= (int)(MaxHp / Avatar.MonsterConfig.LifeRound);
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
            if (BuffManager.HasBuff(BuffEffectTypes.NoAttack))
                return false;
            Action += GameConstants.RoundAts;//200ms + 30
            if (Action >= GameConstants.LimitAts)
            {
                Action = Action - GameConstants.LimitAts;
                return true;
            }
            return false;
        }

        public void HitTarget(int eid)
        {
            LiveMonster target = BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(eid);
            if (target != null)
            {
                BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(Arrow), target, false));

                SkillAssistant.CheckBurst(this, target);
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
        
        public void AddWeapon(TrueWeapon tw)
        {
            if (Avatar.MonsterConfig.IsBuilding)
            {
                NLog.Warn(string.Format("AddWeapon to building {0}", Avatar.Id));
                return;
            }

            if (TWeapon.CardId > 0)
                WeaponAssistant.CheckWeaponEffect(this, TWeapon, -1);
            TWeapon = tw;
            EAddonBook.UpdateWeaponData(TWeapon, OwnerPlayer.State.Weaponskills.Keys(), OwnerPlayer.State.Weaponskills.Values());
            WeaponAssistant.CheckWeaponEffect(this, TWeapon, 1);
        }

        public void DeleteWeapon()
        {
            WeaponAssistant.CheckWeaponEffect(this, TWeapon, -1);
            TWeapon = new TrueWeapon();
        }

        private void CheckDamageBuffEffect(LiveMonster src, HitDamage dam)
        {
            if (dam.Dtype == DamageTypes.Physical)
            {
                if (BuffManager.HasBuff(BuffEffectTypes.Chaos) && MathTool.GetRandom(100) < 20)
                {
                    src.Life -= dam.Value;
                    dam.SetDamage(DamageTypes.Physical, 0);
                }
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

        public void AddSpecialMark(SkillMarks mark)
        {
            SkillManager.AddSpecialMark(mark);
        }

		public void AddStrengthLevel(int value)
		{
			int basedata = value * MathTool.GetSqrtMulti10(Avatar.MonsterConfig.Star);
		    Atk += (double) basedata/10;
//		    Def += (double) basedata/10;
			MaxHp=Avatar.Hp + 3 * basedata / 10;
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

                if (TWeapon.CardId > 0)
                {
                    g.DrawImage(TWeapon.GetImage(32, 32), 5, 60, 32, 32);
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
            get { return TWeapon.CardId; }
        }

        public void AddHp(double addon)
        {
            if (addon > 0)//治疗
            {
                if (BuffManager.HasBuff(BuffEffectTypes.NoCure))
                {
                    return;
                }
            }
            Life += (int)addon;
        }

        public void AddHpRate(double value)
        {
            Life += (int)(MaxHp * value);
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
                NarlonLib.Log.NLog.Warn("hero cannot be Transform");
                return;
            }

            int cardId = TWeapon.CardId;
            var savedWeapon = TWeapon.GetCopy();
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

        [Obsolete("to remove")]
        public void AddCardRate(int monId, int rate)
        {
            
        }

        public void AddResource(int type, int count)
        {
            OwnerPlayer.AddResource((GameResourceType)type, count);
            BattleManager.Instance.FlowWordQueue.Add(new FlowResourceInfo(type + 1, count, Position, 20, 50), false);
        }

        public void AddActionRate(double value)
        {
            Action += (int)(GameConstants.LimitAts*value);
        }

        public void StealWeapon(IMonster target)
        {
            if (target is LiveMonster)
            {
                var weapon = (target as LiveMonster).TWeapon;
                AddWeapon(weapon);
                target.BreakWeapon();
            }
        }

        public void BreakWeapon()
        {
            if (TWeapon.CardId > 0)
                DeleteWeapon();
        }

        public void WeaponReturn(int type)
        {
            if (TWeapon.Avatar.WeaponConfig.Type == type)
            {
                ActiveCard card = new ActiveCard(TWeapon.Card);
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


        public int Star
        {
            get { return Avatar.MonsterConfig.Star; }
        }

        public double HpRate
        {
            get { return (double)life*100/Avatar.Hp; }
        }

        public int MaxHp
        {
            get { return Avatar.Hp; }
            set
            {
                int realvalue = Math.Max(value, 1);
                int hpper = Life * 100 / Avatar.Hp;
                Avatar.Hp = realvalue;
                Life = realvalue * hpper / 100;
            }
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

        public bool HasScroll {
            get {
                if (WeaponId != 0)
                {
                    return TWeapon.Avatar.WeaponConfig.Type == (int)CardTypeSub.Scroll;
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

        public bool DropAdd { get; set; }

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
                BattleLocationManager.SetToPosition(this, dest);
            }
        }

        public void OnMagicDamage(int damage, int element)
        {
            Life -= SkillAssistant.GetMagicDamage(this, new HitDamage(damage, element, DamageTypes.Magic));
        }

        public void SuddenDeath()
        {
            Life = 0;
        }

        public void Summon(int type, int id)
        {
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

            foreach (var pos in posLis)
            {
                if (BattleManager.Instance.MemMap.IsMousePositionCanSummon(pos.X, pos.Y))
                {
                    var mon = new Monster(id);
                    mon.UpgradeToLevel(Level);
                    LiveMonster newMon = new LiveMonster(World.WorldInfoManager.GetCardFakeId(), Level, mon, pos, IsLeft);
                    BattleManager.Instance.MonsterQueue.AddDelay(newMon);
                }
            }
        }

        #endregion
    }
}
