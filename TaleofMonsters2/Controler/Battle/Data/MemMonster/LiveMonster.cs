using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMissile;
using TaleofMonsters.Controler.Battle.Data.MemMonster.Component;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.CardPieces;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Effects;
using TaleofMonsters.DataType.Skills;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Core;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemWeapon;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards.Weapons;
using TaleofMonsters.DataType.Decks;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster
{
    internal class LiveMonster : IMonster
    {
        private readonly MonsterAi aiController;
        public HpBar HpBar { get; private set; }
        public SkillManager SkillManager { get; private set; }
        public BuffManager BuffManager { get; private set; }
        
        private int lastDamagerId; //最后一击的怪物id
        private int peakDamagerLuk; //最高攻击者的幸运值

        private List<MonsterAuro> auroList;//光环
        private int[] antiMagic;//魔法抗性
        private int roundPast; //经过的调用数，每次调用大约200ms
        private float pastRoundTotal; //消耗的时间片，累计到1清理一次

        #region 属性

        public int Id { get; private set; }
        public int Level { get; private set; }
        public Monster Avatar { get; private set; }

        public float GhostTime { get; set; } //0-1表示在墓地状态
        public bool IsDefence { get { return ReadMov == 0 && !Avatar.MonsterConfig.IsBuilding; } }
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

        public double CrtDamAddRate { get; set; }
        public int MovRound { get; set; }
        
        public bool IsSummoned { get; set; } //是否召唤单位

        public int Life
        {
            get { return HpBar.Life; }
        }
        
        public Point CenterPosition
        {
            get { return new Point(Position.X + BattleManager.Instance.MemMap.CardSize / 2, 
                Position.Y + BattleManager.Instance.MemMap.CardSize / 2); }
        }

        public bool IsAlive
        {
            get { return Life > 0; }
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

        public LiveMonster(int level, Monster mon, Point point, bool isLeft)
        {
            Id = World.WorldInfoManager.GetCardFakeId();
            Level = level;
            Avatar = mon;
            Avatar.UpgradeToLevel(level);          

            Position = point;
            IsLeft = isLeft;
            Action = 0;
            roundPast = 0;
            HpBar = new HpBar(this);
            SkillManager = new SkillManager(this);
            AttackType = (int)CardElements.None;
            CanAttack = true;
            
            BuffManager = new BuffManager(this);
            aiController = new MonsterAi(this);
            LiveMonsterToolTip = new LiveMonsterToolTip(this);

            SetBasicData();
            HpBar.SetHp(Avatar.Hp);
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
        }

        public void AddBasicData(int attrIndex, int addon)
        {
            switch (attrIndex)
            {
                case 1: Def += addon; return;
                case 2: Mag += addon; return;
                case 3: Spd += addon; return;
                case 4: Hit += addon; return;
                case 5: Dhit += addon; return;
                case 6: Crt += addon; return;
                case 7: Luk += addon; return;
            }
        }

        public void OnInit()
        {
            SkillManager.CheckInitialEffect();
            MakeSound(true);
        }

        public bool BeHited(LiveMonster src)
        {
            int hitrate = SkillAssistant.GetHit(src, this);
            if (hitrate > MathTool.GetRandom(100))
            {
                HitDamage damage = SkillAssistant.GetDamage(src, this);
                CheckDamageBuffEffect(src, damage);

                HpBar.OnDamage(damage);
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
                    peakDamagerLuk = Math.Max(peakDamagerLuk, src.RealLuk);
                }
                return true;
            }
            return false;
        }

        public void OnDie()
        {
            GhostTime = 0.01f;//开始死亡
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
                OwnerPlayer.IsAlive = false;
            }
            else
            {
                if (!IsLeft)
                {
                    if (Rival is HumanPlayer)
                    {
                        if (BattleManager.Instance.BattleInfo.Items.Count < GameConstants.MaxDropItemGetOnBattle)
                        {
                            int itemId = CardPieceBook.CheckPieceDrop(Avatar.Id, peakDamagerLuk);
                            if (itemId > 0)
                            {
                                BattleManager.Instance.BattleInfo.AddItemGet(itemId);
                                BattleManager.Instance.FlowWordQueue.Add(new FlowItemInfo(itemId, Position, 20, 50), true);
                            }
                            UserProfile.Profile.OnKillMonster(Avatar.MonsterConfig.Star, Avatar.MonsterConfig.Type, Avatar.MonsterConfig.Type);
                        }
                    }
                }
                BattleManager.Instance.BattleInfo.GetPlayer(!IsLeft).Kill++;
            }

            SkillManager.CheckRemoveEffect();
            var rival = Rival as Player;
            rival.OnKillMonster(Level, Avatar.MonsterConfig.Star, Position);

            MakeSound(false);
        }

        public void Next(float pastRound, bool tileMatching)//附带判断地形因素
        {
            roundPast++;
            pastRoundTotal += pastRound;

            HpBar.Update();
            SkillAssistant.CheckAuroState(this, tileMatching);

            if (pastRoundTotal >= 1)
            {
                pastRoundTotal -= 1;
                HpBar.OnRound();
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
                    src.HpBar.OnDamage(dam);
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
            HpBar.SetHp(1);//复活给1hp

            SkillManager.CheckInitialEffect();
        }

		public void AddStrengthLevel(int value)
		{
			int basedata = value * MathTool.GetSqrtMulti10(Avatar.MonsterConfig.Star);
		    Atk += (double) basedata/10;
//		    Def += (double) basedata/10;
//			MaxHp=Avatar.Hp + 3 * basedata / 10;
		}

        private void MakeSound(bool onSummon)
        {
            if (Avatar.MonsterConfig.Sound == "")
            {
                return;
            }

            var soundPath = string.Format(onSummon ? "{0}_Play_01.mp3" : "{0}_Death_03.mp3", Avatar.MonsterConfig.Sound);
            SoundManager.Play("Unit", soundPath);
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

                HpBar.Draw(g);

                g.FillPie(Brushes.Gray, 65, 65, 30, 30, 0, 360);
                var skillPercent = SkillManager.GetRoundSkillPercent();
                if (skillPercent > 0)
                {                
                    //画集气槽
                    g.FillPie(Brushes.Purple, 65, 65, 30, 30, 0, skillPercent * 360 / 100);
                    //画行动槽
                    g.FillPie(CanAttack ? Brushes.Yellow : Brushes.LightGray, 70, 70, 20, 20, 0, Action * 360 / GameConstants.LimitAts);
                }
                else
                {
                    //画行动槽
                    g.FillPie(CanAttack ? Brushes.Yellow : Brushes.LightGray, 65, 65, 30, 30, 0, Action * 360 / GameConstants.LimitAts);
                }

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

                var pen = new Pen(!IsLeft ? Brushes.Blue : Brushes.Red, 3);
                g.DrawRectangle(pen, 1, 1, 98, 98);
                pen.Dispose();

                g.FillRectangle(Brushes.Red, 0, 2, 100, 5);
                g.FillRectangle(Brushes.Cyan, 0, 2, Math.Min(GhostTime * 100, 100), 5);
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
        public int WeaponType
        {
            get { return Weapon != null ? Weapon.Type - CardTypeSub.Weapon + 1 : 0; }
        }
        public void AddHp(double addon)
        {
            HpBar.AddHp((int)addon);
        }

        public void AddHpRate(double value)
        {
            HpBar.AddHp((int)(RealMaxHp * value));
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
            OwnerPlayer.State.CheckMonsterEvent(false, this);
            Avatar = new Monster(monId);
            Avatar.UpgradeToLevel(Level);
            OwnerPlayer.State.CheckMonsterEvent(true, this);
            SetBasicData();
            MonsterCoverBox.CheckCover();
            SkillManager.CheckInitialEffect();
            if (cardId > 0)
            {
                AddWeapon(savedWeapon);
            }
            HpBar.SetHp(Avatar.Hp * lifp / 100);
        }

        public void AddActionRate(double value)
        {
            Action += (int)(GameConstants.LimitAts*value);
        }

        public void Return(int costChange)
        {
            if (Avatar.MonsterConfig.IsBuilding)
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("抵抗", Position, 0, "Gold", 26, 0, 0, 1, 15), false);
                return;
            }

            BattleManager.Instance.MemMap.GetMouseCell(Position.X, Position.Y).UpdateOwner(0);
            SkillManager.CheckRemoveEffect();
            Owner.AddCard(this, CardId, Level, costChange);

            BattleManager.Instance.MonsterQueue.RemoveDirect(Id);
        }

        public void AddWeapon(int weaponId, int lv)
        {
            if (!CanAddWeapon())
            {
                return;
            }

            Weapon wpn = new Weapon(weaponId);
            wpn.UpgradeToLevel(lv);
            var tWeapon = new TrueWeapon(this, lv, wpn);
            AddWeapon(tWeapon);
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
            if (Weapon is TrueWeapon)
            {
                ActiveCard card = new ActiveCard(new DeckCard(Weapon.CardId, (byte)Weapon.Level, 0));
                OwnerPlayer.CardManager.AddCard(card);
                DeleteWeapon();
            }
        }

        public void LevelUpWeapon(int lv)
        {
            if (Weapon != null)
            {
                var weaponId = Weapon.CardId;
                var weaponLevel = Weapon.Level;
                BreakWeapon();
                AddWeapon(weaponId, weaponLevel+lv);
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
            get { return (double)Life * 100/Avatar.Hp; }
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

        public bool HasSkill(int sid)
        {
            return SkillManager.HasSkill(sid);
        }

        //遗忘所有武器技能除外
        public void Silent()
        {
            SkillManager.Forget();

            BuffManager.ClearBuff(false);//清除所有buff
        }

        public int Attr { get { return Avatar.MonsterConfig.Attr; } }

        public int Type { get { return Avatar.MonsterConfig.Type; } }

        public LiveMonsterToolTip LiveMonsterToolTip { get; private set; }

        public MonsterCoverBox MonsterCoverBox { get; private set; }

        public void Disappear()
        {
            if (IsGhost)
            {
                GhostTime = 1;//让坟场消失
            }
        }

        public void AddBuff(int buffId, int blevel, double dura)
        {
            BuffManager.AddBuff(buffId, blevel, dura);
        }

        public void ClearDebuff()
        {
            BuffManager.ClearBuff(true);
        }

        public void ExtendDebuff(double count)
        {
            BuffManager.ExtendDebuff(count);
        }

        public bool HasBuff(int buffid)
        {
            return BuffManager.HasBuff(buffid);
        }

        public void SetToPosition(string type, int step)
        {
            if (Avatar.MonsterConfig.IsBuilding)
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("抵抗", Position, 0, "Gold", 26, 0, 0, 1, 15), false);
                return;
            }

            Point dest = MonsterPositionHelper.GetAvailPoint(Position, type, IsLeft, step);
            if (dest.X != Position.X || dest.Y != Position.Y)
            {
                BattleLocationManager.SetToPosition(this, dest);
            }
        }

        public void OnMagicDamage(IMonster source, double damage, int element)
        {
            var dam = new HitDamage((int) damage, (int) damage, element, DamageTypes.Magic);
            lastDamagerId = source == null ? 0 : source.Id;
            SkillAssistant.CheckMagicDamage(this, dam);
            HpBar.OnDamage(dam);
        }

        public void SuddenDeath()
        {
            if (Avatar.MonsterConfig.IsBuilding)
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("抵抗", Position, 0, "Gold", 26, 0, 0, 1, 15), false);
                return;
            }
            HpBar.SetHp(0);
        }

        public void Rebel()
        {
            if (Avatar.MonsterConfig.IsBuilding)
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("抵抗", Position, 0, "Gold", 26, 0, 0, 1, 15), false);
                return;
            }
            IsLeft = !IsLeft;
        }

        public void AddMaxHp(double value)
        {
            if (Avatar.MonsterConfig.IsBuilding)
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("抵抗", Position, 0, "Gold", 26, 0, 0, 1, 15), false);
                return;
            }

            var lifeRate = Life/MaxHp;
            MaxHp.Source += value;
            if (value > 0)
            {
                AddHp(value);//顺便把hp也加上
            }
            else
            {
                AddHp(lifeRate * value);
            }
        }

        public void Summon(string type, int id, int count)
        {
            if (IsSummoned)
            {//召唤生物不能继续召唤，防止无限循环
                return;
            }

            List<Point> posLis = MonsterPositionHelper.GetAvailPointList(Position, type, IsLeft, count);

            foreach (var pos in posLis)
            {
                var mon = new Monster(id);
                mon.UpgradeToLevel(Level);
                LiveMonster newMon = new LiveMonster(Level, mon, pos, IsLeft);
                newMon.IsSummoned = true;
                BattleManager.Instance.MonsterQueue.AddDelay(newMon);
            }
        }

        public void SummonRandomAttr(string type, int attr)
        {
            int cardId;
            while (true)
            {
                cardId = CardConfigManager.GetRandomAttrCard(attr);
                if(CardConfigManager.GetCardConfig(cardId).Type == CardTypes.Monster)
                    break;
            }
            Summon(type, cardId, 1);
        }

        public void MadDrug()
        {
            if (!Avatar.MonsterConfig.IsBuilding && RealRange > 0)
            {
                Atk.Source = MaxHp.Source/5;
            }
        }

        public void CureRandomAlien(double rate)
        {
            IMonster target = null;
            foreach (IMonster o in Map.GetAllMonster(Position))
            {
                if (o.IsLeft == IsLeft && o.HpRate < 100 && o.Id != Id)
                {
                    if (target == null || target.HpRate>o.HpRate)
                    {
                        target = o;
                    }
                }
            }
            if (target!=null)
            {
                target.AddHpRate(rate);
                BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect("yellowstar"), (LiveMonster)target, false));
            }
        }

        public bool ResistBuffType(int type)
        {
            var rate = BuffManager.GetBuffImmuneRate(type);
            return MathTool.GetRandom(0.0, 1.0) < rate;
        }

        public void EatTomb(IMonster tomb)
        {
            Atk.Source *= 1.1;
            AddMaxHp(MaxHp.Source*0.1);
            (tomb as LiveMonster).Disappear();
        }

        public void ClearTarget()
        {
            aiController.ClearTarget();
        }

        public void AddPArmor(double val)
        {
            HpBar.AddPArmor((int)val);
        }

        public void AddMArmor(double val)
        {
            HpBar.AddMArmor((int)val);
        }
        #endregion
    }
}
