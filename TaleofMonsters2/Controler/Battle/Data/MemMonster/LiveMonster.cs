using System;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster.Component;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.CardPieces;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Skills;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Core;
using TaleofMonsters.Controler.Battle.Data.MemWeapon;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Formulas;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster
{
    internal class LiveMonster : IMonster
    {
        private readonly MonsterAi aiController;
        public HpBar HpBar { get; private set; }
        public SkillManager SkillManager { get; private set; }
        public BuffManager BuffManager { get; private set; }
        public LiveMonsterToolTip LiveMonsterToolTip { get; private set; }
        public MonsterCoverBox MonsterCoverBox { get; private set; }
        public AuroManager AuroManager { get; private set; }
        private int lastDamagerId; //最后一击的怪物id
        private int peakDamagerLuk; //最高攻击者的幸运值

    
        private int[] antiMagic;//魔法抗性
        private int roundPast; //经过的调用数，每次调用大约200ms
        private float pastRoundTotal; //消耗的时间片，累计到1清理一次

        #region 属性

        public int Id { get; private set; }
        public int Level { get; private set; }
        public Monster Avatar { get; set; }

        public float GhostTime { get; set; } //0-1表示在墓地状态
        public bool IsDefence { get { return ReadMov == 0 && !Avatar.MonsterConfig.IsBuilding; } }
        public Point Position { get; set; }
        public IMap Map { get { return BattleManager.Instance.MemMap; } }
        public IMonsterAction Action { get; private set; }
        public bool IsLeft { get; set; }
        private int actPoint; //行动点数
        public IBattleWeapon Weapon { get; set; }

        public int CardId { get { return Avatar.Id; } }
        public int WeaponId { get { return Weapon == null ? 0 : Weapon.CardId; } }
        public int WeaponType { get { return Weapon != null ? Weapon.Type - CardTypeSub.Weapon + 1 : 0; } }

        public int Star { get { return Avatar.Star; } }
        public int Attr { get { return Avatar.MonsterConfig.Attr; } }
        public int Type { get { return Avatar.MonsterConfig.Type; } } //种族

        public virtual bool CanMove { get { return !BuffManager.HasBuff(BuffEffectTypes.NoMove); } }
        public bool CanAttack { get; set; }
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

        public int Cure { get { return Avatar.Cure; } }
        public double CrtDamAddRate { get; set; }
        public int MovRound { get; set; }

        public bool IsSummoned { get; set; } //是否召唤单位

        public int Hp { get { return HpBar.Life; } }

        public double HpRate { get { return (double)Hp * 100 / Avatar.Hp; } }
        public bool IsAlive { get { return Hp > 0; } }

        public Point CenterPosition
        {
            get { return new Point(Position.X + BattleManager.Instance.MemMap.CardSize / 2, 
                Position.Y + BattleManager.Instance.MemMap.CardSize / 2); }
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

        public virtual string Arrow
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
            actPoint = 0;
            roundPast = 0;
            HpBar = new HpBar(this);
            SkillManager = new SkillManager(this);
            AttackType = (int)CardElements.None;
            CanAttack = true;
            
            BuffManager = new BuffManager(this);
            aiController = new MonsterAi(this);
            LiveMonsterToolTip = new LiveMonsterToolTip(this);
            AuroManager = new AuroManager(this);
            SetBasicData();
            HpBar.SetHp(Avatar.Hp);
            MonsterCoverBox = new MonsterCoverBox(this);
            Action = new MonsterAction(this);
        }

        public void SetBasicData()
        {
            BuffManager.Reload();
            AuroManager.Reload();
            SkillManager.Reload();
            
            antiMagic = new int[7];//7个属性

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
                    src.Weapon.OnHit();
                if (WeaponId > 0)
                    Weapon.OnHited();
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

        public virtual void OnDie()
        {
            SkillManager.DeathSkill(); // 亡语

            GhostTime = 0.01f;//开始死亡
            BattleManager.Instance.MemMap.GetMouseCell(Position.X,Position.Y).UpdateOwner(-Id);
            if (!IsLeft)
            {
                if (Rival is HumanPlayer)
                {
                    if (BattleManager.Instance.StatisticData.Items.Count < GameConstants.MaxDropItemGetOnBattle)
                    {
                        int itemId = CardPieceBook.CheckPieceDrop(Avatar.Id, peakDamagerLuk);
                        if (itemId > 0)
                        {
                            BattleManager.Instance.StatisticData.AddItemGet(itemId);
                            BattleManager.Instance.FlowWordQueue.Add(new FlowItemInfo(itemId, Position, 20, 50));
                        }
                        UserProfile.Profile.OnKillMonster(Avatar.Star, Avatar.MonsterConfig.Type, Avatar.MonsterConfig.Type);
                    }
                }
            }
            BattleManager.Instance.StatisticData.GetPlayer(!IsLeft).Kill++;

            SkillManager.CheckRemoveEffect();
            var rival = Rival as Player;
            rival.OnKillMonster(Avatar.Id, Level, Avatar.Star, Position);

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
                if (WeaponId > 0)
                    Weapon.OnRound();
            }

            BuffManager.BuffCount();

            if (SkillManager.CheckSpecial(pastRound))
                return;//特殊技能触发

            aiController.CheckAction();
        }

        public bool AddAts()
        {
            if (BuffManager.HasBuff(BuffEffectTypes.NoAction))
                return false;
            actPoint += GameConstants.RoundAts;//200ms + 30
            if (actPoint >= GameConstants.LimitAts)
            {
                actPoint = actPoint - GameConstants.LimitAts;
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
                    BattleManager.Instance.FlowWordQueue.Add(new FlowWord("Miss!", new Point(Position.X + 40, Position.Y + 40), 0, "red", -10, 0));
            }
        }

        public void CheckMagicDamage(HitDamage damage)
        {
            if (antiMagic[damage.Element] > 0)
                damage.SetDamage(DamageTypes.Magic, Math.Max(damage.Value*(100 - antiMagic[damage.Element])/100, 0));
        }

        public bool CanAddWeapon()
        {
            return !Avatar.MonsterConfig.IsBuilding && !IsGhost;
        }
        
        public void AddWeapon(IBattleWeapon tw)
        {
            if (!CanAddWeapon())
            {
                NLog.Warn("AddWeapon to building {0}", Avatar.Id);
                return;
            }

            if (Weapon != null)
                Weapon.CheckWeaponEffect(this, -1);
            Weapon = tw;
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
                dam.SetDamage(DamageTypes.All, 1);
        }


        public void Revive()
        {
            GhostTime = 0;
            BattleManager.Instance.MemMap.GetMouseCell(Position.X, Position.Y).UpdateOwner(Id);
            HpBar.SetHp(1);//复活给1hp

            SkillManager.CheckInitialEffect();
        }

        private void MakeSound(bool onSummon)
        {
            if (Avatar.MonsterConfig.Sound == "")
                return;

            var soundPath = string.Format(onSummon ? "{0}_Play_01.mp3" : "{0}_Death_03.mp3", Avatar.MonsterConfig.Sound);
            SoundManager.Play("Unit", soundPath);
        }

        protected virtual void DrawImg(Graphics g)
        {
            var img = MonsterBook.GetMonsterImage(Avatar.Id, 100, 100);
            if (img != null)
                g.DrawImage(img, 0, 0, 100, 100);
        }

        public void DrawOnBattle(Graphics g2, Color uponColor)
        {
            Bitmap image = new Bitmap(100, 100);
            Graphics g = Graphics.FromImage(image);

            if (!IsGhost)
            {
                DrawImg(g);
              
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
                    g.FillPie(CanAttack ? Brushes.Yellow : Brushes.LightGray, 70, 70, 20, 20, 0, actPoint * 360 / GameConstants.LimitAts);
                }
                else
                {
                    //画行动槽
                    g.FillPie(CanAttack ? Brushes.Yellow : Brushes.LightGray, 65, 65, 30, 30, 0, actPoint * 360 / GameConstants.LimitAts);
                }

                var starIcon = HSIcons.GetIconsByEName("sysstar");
                for (int i = 0; i < Avatar.Star; i++)
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

        public void AddHp(double addon)
        {
            if (Type == (int)CardTypeSub.Machine || Type == (int)CardTypeSub.KingTower || Type == (int)CardTypeSub.Machine)
                addon=1;
            HpBar.AddHp((int)addon);
        }

        public void AddHpRate(double value)
        {
            var healValue = (int) (RealMaxHp*value);
            if (Type == (int)CardTypeSub.Machine || Type == (int)CardTypeSub.KingTower || Type == (int)CardTypeSub.Machine)
                healValue = 1;
            HpBar.AddHp(healValue);
        }

        public void DecHp(double addon)
        {
            HpBar.AddHp(-(int)addon);
        }

        public void RepairHp(double addon)
        {
            if (Type != (int)CardTypeSub.Machine && Type != (int)CardTypeSub.KingTower && Type != (int)CardTypeSub.Machine)
                addon = 1;
            HpBar.AddHp((int)addon);
        }

        public bool HasSkill(int sid)
        {
            return SkillManager.HasSkill(sid);
        }
        
        public void AddMaxHp(double value)
        {
            if (Avatar.MonsterConfig.IsBuilding)
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("抵抗", Position, 0, "Gold", 26, 0, 0, 1, 15));
                return;
            }

            var lifeRate = Hp / MaxHp;
            MaxHp.Source += value;
            if (value > 0)
                AddHp(value);//顺便把hp也加上
            else
                AddHp(lifeRate * value);
        }
        public void AddAntiMagic(string type, int value)
        {
            if (type == "All")
            {
                for (int i = 0; i < antiMagic.Length; i++)
                    antiMagic[i] += value;
            }
            else
            {
                antiMagic[(int)Enum.Parse(typeof(CardElements), type)] += value;
            }
        }

        public void OnMagicDamage(IMonster source, double damage, int element)
        {
            lastDamagerId = source == null ? 0 : source.Id;
            var damValue = damage * (1 - FormulaBook.GetMagDefRate(RealMag));
            var dam = new HitDamage((int)damValue, (int)damValue, element, DamageTypes.Magic);
            CheckMagicDamage(dam);
            HpBar.OnDamage(dam);
        }

        public void OnSpellDamage(double damage, int element)
        {
            OnSpellDamage(damage, element, 0);
        }

        public void OnSpellDamage(double damage, int element, double vibrate)
        {
            if (vibrate > 0)
            {
                var vAddon = OwnerPlayer.SpecialAttr.SpellVibrate;
                damage = damage*(1 + MathTool.GetRandom(-vibrate + vAddon*vibrate*2, vibrate));
            }
            var damValue = damage * (1 - FormulaBook.GetMagDefRate(RealMag));
            var dam = new HitDamage((int)damValue, (int)damValue, element, DamageTypes.Magic);
            CheckMagicDamage(dam);
            HpBar.OnDamage(dam);
        }

        public void AddActionRate(double value)
        {
            actPoint += (int)(GameConstants.LimitAts * value);
        }

        public void ClearTarget()
        {
            aiController.ClearTarget();
        }

    }
}
