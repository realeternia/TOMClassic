using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster.Component;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Controler.Battle.Data.MemWeapon;
using TaleofMonsters.Controler.Battle.DataTent;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;
using TaleofMonsters.Datas.Formulas;
using TaleofMonsters.Datas.Skills;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster
{
    internal class LiveMonster : IMonster
    {
        public class AttrModifyInfo
        {
            internal enum AttrModifyTypes
            {
                Skill = 1, Buff, Weapon, WeaponSide, Spell
            }
            internal enum AttrTypes
            {
                Atk = 1, Def, Mag, Spd, Hit, Dhit, Crt, Luk, MaxHp
            }
            public AttrModifyTypes Type { get; set; }
            public int ItemId { get; set; }
            public AttrTypes Attr { get; set; }
            public int Val { get; set; }
        }

        private readonly MonsterAi aiController;
        public HpBar HpBar { get; private set; }
        public SkillManager SkillManager { get; private set; }
        public BuffManager BuffManager { get; private set; }
        public LiveMonsterToolTip LiveMonsterToolTip { get; private set; }
        public MonsterCoverBox CoverBox { get; private set; }
        public AuroManager AuroManager { get; private set; }
        
        #region 属性
        private int lastDamagerId; //最后一击的怪物id
        
        private int[] antiMagic;//魔法抗性
        private int roundPast; //经过的调用数，每次调用大约200ms
        private float pastRoundTotal; //消耗的时间片，累计到1清理一次

        public int PrepareAtsNeed { get; set; }

        public int Id { get; private set; }
        public int Level { get; private set; }
        public Monster Avatar { get; set; }

        public float GhostTime { get; set; } //0-1表示在墓地状态
        public bool IsDefence { get { return ReadMov == 0 || Avatar.MonsterConfig.IsBuilding; } }
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
        public int Atk { get; set; }
        public int MaxHp { get; set; }

        public int Def { get; set; }
        public int Mag { get; set; }
        public int Spd { get; set; }
        public int Hit { get; set; }
        public int Dhit { get; set; }
        public int Crt { get; set; }
        public int Luk { get; set; }

        public List<AttrModifyInfo> ModifyList { get; private set; } //属性加成和修改
        public int Cure { get { return Avatar.Cure; } }
        public double CrtDamAddRate { get; set; }
        public int MovRound { get; set; }

        public bool IsPrepare { get; set; } //是否在准备状态
        public bool IsSummoned { get; set; } //是否召唤单位

        public int Hp { get { return HpBar.Life; } }

        public double HpRate { get { return (double)Hp * 100 / Avatar.Hp; } }
        public bool IsAlive { get { return Hp > 0; } }

        public Point CenterPosition
        {
            get { return new Point(Position.X + BattleManager.Instance.MemMap.CardSize / 2, 
                Position.Y + BattleManager.Instance.MemMap.CardSize / 2); }
        }

        public int RealAtk { get; private set; }

        public int RealMaxHp { get; set; }

        public int RealDef { get; private set; }

        public int RealMag { get; private set; }

        public int RealSpd { get; private set; }

        public int RealHit { get; private set; }

        public int RealDHit { get; private set; }

        public int RealCrt { get; private set; }

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
            CoverBox = new MonsterCoverBox(this);
            Action = new MonsterAction(this);
            IsPrepare = true;
            PrepareAtsNeed = GameConstants.PrepareAts;
            ModifyList = new List<AttrModifyInfo>();
        }

        public void SetBasicData()
        {
            BuffManager.Reload();
            AuroManager.Reload();
            SkillManager.Reload();
            
            antiMagic = new int[7];//7个属性

            RealAtk = Atk = Avatar.Atk;
            RealMaxHp = MaxHp = Avatar.Hp;
            RealDef = Def = Avatar.Def;
            RealMag = Mag = Avatar.Mag;
            RealSpd = Spd = Avatar.Spd;
            RealHit = Hit = Avatar.Hit;
            RealDHit = Dhit = Avatar.Dhit;
            RealCrt = Crt = Avatar.Crt;
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
                DamageData damage = SkillAssistant.GetDamage(src, this);
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
                }

                if (OwnerPlayer.Tower == this)
                {
                    OwnerPlayer.OnTowerHited(HpRate);
                }
                return true;
            }
            return false;
        }

        public virtual void OnDie()
        {
            var killer = BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(lastDamagerId);
            BattleManager.Instance.EventMsgQueue.Pubscribe(EventMsgQueue.EventMsgTypes.MonsterDie, Owner, this, killer, null, Position, 0, 0, 0);

            GhostTime = 0.01f;//开始死亡
            BattleManager.Instance.MemMap.UpdateCellOwner(Position, -Id);

            SkillManager.CheckRemoveEffect();
            DeleteWeapon(true);
            
            OwnerPlayer.OnMonsterDie(CardId, (byte)Level, IsSummoned);
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
            var checkVal = IsPrepare ? PrepareAtsNeed : GameConstants.LimitAts;
            if (actPoint >= checkVal)
            {
                actPoint = actPoint - checkVal;
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

        public void CheckMagicDamage(DamageData damage)
        {
            if (antiMagic[damage.Element] > 0)
                damage.SetDamage(DamageTypes.Magic, Math.Max(damage.Value*(100 - antiMagic[damage.Element])/100, 0));
        }

        public bool CanAddWeapon()
        {
            return !Avatar.MonsterConfig.IsBuilding && !IsGhost;
        }
        
        public void AddWeapon(IBattleWeapon weapon, bool isAdd)
        {
            if (!CanAddWeapon())
            {
                NLog.Warn("AddWeapon to building {0}", Avatar.Id);
                return;
            }

            if (Weapon != null)
                Weapon.CheckWeaponEffect(this, false);
            Weapon = weapon;
            Weapon.CheckWeaponEffect(this, true);
        }

        public void DeleteWeapon(bool toGrave)
        {
            if (Weapon != null)
            {
                Weapon.CheckWeaponEffect(this, false);
                Weapon = null;

                if (toGrave && !CardConfigManager.GetCardConfig(CardId).IsSpecial)
                    OwnerPlayer.OffCards.AddGrave(new ActiveCard(CardId, (byte)Level));
            }
        }

        private void CheckDamageBuffEffect(LiveMonster src, DamageData dam)
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
            BattleManager.Instance.MemMap.UpdateCellOwner(Position, Id);
            HpBar.SetHp(1);//复活给1hp

            SkillManager.CheckInitialEffect();
        }

        public void AddHp(double addon)
        {
            if (Type == (int) CardTypeSub.Machine || Type == (int) CardTypeSub.KingTower || Type == (int) CardTypeSub.NormalTower)
                addon = 1;
            HpBar.AddHp((int)addon);
        }

        public void AddHpRate(double value)
        {
            var healValue = (int) (RealMaxHp*value);
            if (Type == (int)CardTypeSub.Machine || Type == (int)CardTypeSub.KingTower || Type == (int)CardTypeSub.NormalTower)
                healValue = 1;
            HpBar.AddHp(healValue);
        }

        public void DecHp(double addon)
        {
            HpBar.AddHp(-(int)addon);
        }

        public void RepairHp(double addon)
        {
            if (Type != (int)CardTypeSub.Machine && Type != (int)CardTypeSub.KingTower && Type != (int)CardTypeSub.NormalTower)
                addon = 1;
            HpBar.AddHp((int)addon);
        }

        public bool HasSkill(int sid)
        {
            return SkillManager.HasSkill(sid);
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
            var dam = new DamageData((int)damValue, (int)damValue, element, DamageTypes.Magic);
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
            var dam = new DamageData((int)damValue, (int)damValue, element, DamageTypes.Magic);
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

        public virtual void AddAttrModify(int tp, int itemId, int attr, int val)
        {
            if (val == 0)
                return;

            var findItem = ModifyList.Find(data => data.Type == (AttrModifyInfo.AttrModifyTypes) tp && data.ItemId == itemId &&
                        data.Attr == (AttrModifyInfo.AttrTypes) attr);
            if (findItem != null)
                findItem.Val += val;
            else
                ModifyList.Add(new AttrModifyInfo {Type = (AttrModifyInfo.AttrModifyTypes)tp, ItemId = itemId, Attr = (AttrModifyInfo.AttrTypes)attr, Val = val});

            RefreshAttrs();
        }

        public void RemoveAttrModify(int tp, int itemId)
        {
            ModifyList.RemoveAll(item => (int) item.Type == tp && item.ItemId == itemId);

            RefreshAttrs();
        }

        public void RemoveAllAttrModify()
        {//看下是否需要合并
            ModifyList.RemoveAll(item => item.Type != AttrModifyInfo.AttrModifyTypes.Weapon);

            RefreshAttrs();
        }

        public void RefreshAttrs()
        {
            RealAtk = Atk;
            RealDef = Def;
            RealMag = Mag;
            RealSpd = Spd;
            RealHit = Hit;
            RealDHit = Dhit;
            RealCrt = Crt;
            RealMaxHp = MaxHp;
            foreach (var attrModifyInfo in ModifyList)
            {
                switch (attrModifyInfo.Attr)
                {
                    case AttrModifyInfo.AttrTypes.Atk: RealAtk += attrModifyInfo.Val; break;
                    case AttrModifyInfo.AttrTypes.MaxHp: RealMaxHp += attrModifyInfo.Val; break;
                    case AttrModifyInfo.AttrTypes.Def: RealDef += attrModifyInfo.Val; break;
                    case AttrModifyInfo.AttrTypes.Mag: RealMag += attrModifyInfo.Val; break;
                    case AttrModifyInfo.AttrTypes.Spd: RealSpd += attrModifyInfo.Val; break;
                    case AttrModifyInfo.AttrTypes.Hit: RealHit += attrModifyInfo.Val; break;
                    case AttrModifyInfo.AttrTypes.Dhit: RealDHit += attrModifyInfo.Val; break;
                    case AttrModifyInfo.AttrTypes.Crt: RealCrt += attrModifyInfo.Val; break;
                }
            }
        }

        public bool ResistBuffType(BuffImmuneGroup type)
        {
            var rate = BuffManager.GetBuffImmuneRate((int)type);
            return MathTool.GetRandom(0.0, 1.0) < rate;
        }

        public void SetAIMode(MonsterAi.AiModes mode)
        {
            if (aiController != null)
                aiController.AIMode = mode;
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

                if (IsPrepare)
                {//绘制召唤遮罩
                    var prepareRate = Math.Min(1, 0.2 + 0.3 * actPoint / PrepareAtsNeed);
                    SolidBrush brush = new SolidBrush(Color.FromArgb(Math.Max(0, (int)(255 - 255 * prepareRate)), Color.Black));
                    g.FillRectangle(brush, 0, 0, 100, 100);
                    brush.Dispose();
                }

                if (uponColor != Color.White)
                {
                    SolidBrush brush = new SolidBrush(Color.FromArgb(150, uponColor));
                    g.FillRectangle(brush, 0, 0, 100, 100);
                    brush.Dispose();
                }

                var pen = new Pen(!IsLeft ? Brushes.Blue : Brushes.Red, 3);
                g.DrawRectangle(pen, 1, 1, 98, 98);
                pen.Dispose();

                HpBar.Draw(g);
                if (!IsPrepare)
                {
                    g.FillPie(Brushes.Gray, 65, 65, 30, 30, 0, 360);
                    var skillPercent = SkillManager.GetRoundSkillPercent();
                    var atsRate = (float)actPoint / GameConstants.LimitAts;
                    if (skillPercent > 0)
                    {
                        //画集气槽
                        g.FillPie(Brushes.Purple, 65, 65, 30, 30, 0, skillPercent * 360 / 100);
                        //画行动槽
                        g.FillPie(CanAttack ? Brushes.Yellow : Brushes.LightGray, 70, 70, 20, 20, 0, atsRate * 360);
                    }
                    else
                    {
                        //画行动槽
                        g.FillPie(CanAttack ? Brushes.Yellow : Brushes.LightGray, 65, 65, 30, 30, 0, atsRate * 360);
                    }

                    var starIcon = HSIcons.GetIconsByEName("sysstar");
                    for (int i = 0; i < Avatar.Star; i++)
                        g.DrawImage(starIcon, i * 12, 8, 16, 16);

                    Font fontLevel = new Font("Arial", 20 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                    g.DrawString(Level.ToString(), fontLevel, Brushes.Wheat, Level < 10 ? 71 : 67, 68);
                    g.DrawString(Level.ToString(), fontLevel, Brushes.DarkBlue, Level < 10 ? 70 : 66, 67);
                    fontLevel.Dispose();
                }
                else
                {
                    var prepareRate = Math.Min(1, (float)actPoint / PrepareAtsNeed);
                    g.FillPie(Brushes.DodgerBlue, 30, 30, 40, 40, 0, prepareRate * 360);
                }

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
    }
}
