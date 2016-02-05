﻿using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
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
        private int action;

        private int life;
        private int oldLife;
        private int lastDamagerId;

        private List<ActiveEffect> coverEffectList = new List<ActiveEffect>();//变身时需要重算
        private List<MonsterAuro> auroList;//光环
        private int[] antiMagic;//魔法抗性

        private readonly HpBar hpBar;
        private bool canAttack;//雕像会设成false

        #region 属性

        public int Id { get; private set; }
        public int Level { get; private set; }
        public Monster Avatar { get; private set; }
        public int HpReg { get; set; }
        public int GhostTime { get; set; }
        public Point Position { get; set; }
        public bool IsHero { get; set; }
        public IMap Map { get { return BattleManager.Instance.MemMap; } }
        public bool IsLeft { get; set; }

        public TrueWeapon TWeapon { get; set; }
        public SkillManager SkillManager { get; private set; }
        public BuffManager BuffManager { get; private set; }
        public int RoundMark { get; private set; }

        public bool IsMagicAtk { get; set; }//只有武器可以改变，技能不行
        public int AttackType { get; set; }//只有武器可以改变，技能不行

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
            get { return new Point(Position.X + 40, Position.Y + 40); }
        }

        public int Action
        {
            get { return action; }
            set { action = Math.Max(value, 0); }
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
                double diff = (Def.Source + Def.Adder) * (1 + Def.Multiter) - Def.Source;
                if (SkillManager.HasSpecialMark(SkillMarks.AtkDefBonus))
                {
                    diff = diff * 3 / 2;
                }
                return Math.Max((int)(Def.Source + diff), 0);
            }
        }

        public int RealMag
        {
            get
            {
                return (int)Math.Max((Mag.Source + Mag.Adder) * (1 + Mag.Multiter), 0);
            }
        }

        public int RealHit
        {
            get
            {
                int hit = (int) Math.Max((Hit.Source + Hit.Adder)*(1 + Hit.Multiter), 0);
                int dhit = RealDhit;
                if (SkillManager.HasSpecialMark(SkillMarks.HitHigh) && dhit > hit)
                {
                    return dhit;
                }
                if (SkillManager.HasSpecialMark(SkillMarks.HitLow) && dhit < hit)
                {
                    return dhit;
                }
                return hit;
            }
        }

        public int RealDhit
        {
            get
            {
                return (int)Math.Max((DHit.Source + DHit.Adder) * (1 + DHit.Multiter), 0);
            }
        }

        public int RealLuk
        {
            get { return (int)((Luk.Source + Luk.Adder) * (1 + Luk.Multiter)); }
        }

        public int RealSpd
        {
            get { return (int)((Spd.Source + Spd.Adder) * (1 + Spd.Multiter)); }
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

        public int Range
        {
            get { return 2; }
        }

        public int Mov
        {
            get { return 1; }
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
            Ats = GameConstants.RoundAts;
            action = 0;
            RoundMark = 0;
            SkillManager = new SkillManager(this);
            TWeapon = new TrueWeapon();
            AttackType = (int)CardElements.None;
            CanAttack = true;
            
            BuffManager = new BuffManager(this);

            SetBasicData();
            CheckCover();
        }

        private void CheckCover()
        {
            string cover = Avatar.MonsterConfig.Cover;
            if (!string.IsNullOrEmpty(cover))
            {
                ActiveEffect ef = new ActiveEffect(EffectBook.GetEffect(cover), this, true);
                ef.Repeat = true;
                BattleManager.Instance.EffectQueue.Add(ef);
                coverEffectList.Add(ef);
            }

            SkillManager.CheckCover(coverEffectList);
        }

        private void RemoveAllCover()
        {
            foreach (var activeEffect in coverEffectList)
            {
                activeEffect.IsFinished = RunState.Finished;
            }
            coverEffectList.Clear();
        }

        private void SetBasicData()
        {
            BuffManager.Reload();
            auroList = new List<MonsterAuro>();
            SkillManager.Reload();
            
            antiMagic = new int[9];//8个属性，+1圣属性

            Atk = new AttrModifyData(Avatar.Atk);
            Def = new AttrModifyData(Avatar.Def);
            Mag = new AttrModifyData(Avatar.Mag);
            Hit = new AttrModifyData(Avatar.Hit);
            DHit = new AttrModifyData(Avatar.Dhit);
            Spd = new AttrModifyData(Avatar.Spd);
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
            GhostTime = 1;
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
                BattleManager.Instance.BattleInfo.GetPlayer(!IsLeft).Kill++;
            }

            SkillManager.CheckRemoveEffect();
            if (lastDamagerId != 0)
            {
                var rival = Rival as Player;
                rival.OnKillMonster(lastDamagerId, Level, Avatar.MonsterConfig.Star, Position);
            }
        }

        public void Next(TileMatchResult tileMatching)//附带判断地形因素
        {
            RoundMark++;
            hpBar.Update();
            if ((RoundMark % 4) == 3)
            {
                SkillAssistant.CheckAuroState(this, tileMatching);
            }
            if ((RoundMark % 100) == 0)//一回合
            {
                Life += HpReg;
            }
            BuffManager.BuffCount();

            bool isLeft = IsLeft;
            if (BuffManager.HasBuff(BuffEffectTypes.Rebel))//控制
            {
                isLeft = !isLeft;
            }
            if (AddAts())
            {
                if (SkillManager.CheckSpecial())
                {
                    return;//特殊技能触发
                }

                if (!CanAttack)
                {
                    return;
                }

                int eid = BattleManager.Instance.MemMap.GetEnemyId(Id, isLeft, Position.Y, IsShooter);
                if (eid != 0)
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
            }
        }

        public bool AddAts()
        {
            if (BuffManager.HasBuff(BuffEffectTypes.NoAttack))
                return false;
            action += Ats;//200ms + 30
            if (action >= GameConstants.LimitAts)
            {
                action = action - GameConstants.LimitAts + MathTool.GetRandom(Avatar.Spd);
                return true;
            }
            return false;
        }

        public void CheckMagicDamage(HitDamage damage)
        {
            if (damage.Element>0&&antiMagic[damage.Element-1]>0)
            {
                damage.SetDamage(DamageTypes.Magic,Math.Max(damage.Value*(100 - antiMagic[damage.Element - 1])/100,0));
            }
        }
        
        public void AddWeapon(TrueWeapon tw)
        {
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
		    Def += (double) basedata/10;
			MaxHp=Avatar.Hp + 3 * basedata / 10;
		}

        public void Draw(Graphics g2, Color uponColor)
        {
            Bitmap image = new Bitmap(100, 100);
            Graphics g = Graphics.FromImage(image);

            if (!IsGhost)
            {
                var monImg = Avatar.MonsterConfig.Type == (int)CardTypeSub.Hero ? OwnerPlayer.HeroImage : MonsterBook.GetMonsterImage(Avatar.Id, 100, 100);
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
                Pen pen;
                pen = new Pen(!IsLeft ? Brushes.Blue : Brushes.Red, 3);
                Font font = new Font("Arial", 7*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                Font font2 = new Font("Arial", 6*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                Font fontLevel = new Font("Arial", 10*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                g.DrawRectangle(pen, 1, 1, 98, 98);
                pen.Dispose();

                hpBar.Draw(g);

                g.FillPie(Brushes.Gray, 75, 15, 20, 20, 0, 360);
                g.FillPie(CanAttack? Brushes.Yellow:Brushes.LightGray, 75, 15, 20, 20, 0, action*360 / GameConstants.LimitAts);

                const string stars = "★★★★★★★★★★";
                g.DrawString(stars.Substring(10 - Avatar.MonsterConfig.Star), font2, Brushes.Yellow, 0, 0);
#if DEBUG
                g.DrawString(Id.ToString(), font, Brushes.White, 0, 20);
#endif
                g.DrawString("AT", font, Brushes.Red, 0, 81);
                g.FillRectangle(Brushes.Red, 15, 84, Avatar.Atk / 8, 5);
                g.DrawString("DF", font, Brushes.Blue, 0, 88);
                g.FillRectangle(Brushes.Blue, 15, 91, Avatar.Def / 8, 5);
                g.DrawString(Level.ToString(), fontLevel, Brushes.DarkBlue, Level < 10 ? 81 : 77, 18);
                g.DrawString(Level.ToString(), fontLevel, Brushes.LightPink, Level<10? 80:76,17);
                font.Dispose();
                font2.Dispose();
                fontLevel.Dispose();

                if (TWeapon.CardId>0)
                    g.DrawImage(TWeapon.GetImage(16, 16), 5, 30, 16, 16);
                BuffManager.DrawBuff(g);
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

        /// <summary>
        /// 模拟tooltip实现卡片的点击说明
        /// </summary>
        public void DrawCardToolTips(Graphics g)
        {
            var img = GetMonsterImage();
            int size = BattleManager.Instance.MemMap.CardSize;
            int stagewid = BattleManager.Instance.MemMap.StageWidth;
            int stageheg = BattleManager.Instance.MemMap.StageHeight;
            int x = Position.X + size;
            int y = Position.Y;
            if (x + img.Width > stagewid)
                x = Position.X - img.Width;
            if (y + img.Height > stageheg)
                y = stageheg - img.Height - 1;
            g.DrawImage(img, x, y, img.Width, img.Height);
            img.Dispose();
        }

        private Image GetMonsterImage()
        {
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            var cardQual = Config.CardConfigManager.GetCardConfig(CardId).Quality;
            var name = string.Format("{0}(Lv{1})", Avatar.Name, Level);
            tipData.AddTextNewLine(name, HSTypes.I2QualityColor(cardQual), 20);
            tipData.AddImage(HSIcons.GetIconsByEName("atr" + Avatar.MonsterConfig.Attr), 16, 16);
            tipData.AddImage(HSIcons.GetIconsByEName("rac" + Avatar.MonsterConfig.Type), 16, 16);
            tipData.AddLine();
            AddText(tipData,"物攻", (int)Atk.Source,RealAtk,!IsMagicAtk && CanAttack ? "White" : "DarkGray", true);
            AddText(tipData, "物防", (int)Def.Source, RealDef, "White", false);
            AddText(tipData, "魔力", (int)Mag.Source, RealMag, !IsMagicAtk || !CanAttack ? "DarkGray" : "White", true);
            AddText(tipData, "命中", (int)Hit.Source, RealHit, "White", true);
            AddText(tipData, "回避", (int)DHit.Source, RealDhit, "White", false);
            AddText(tipData, "速度", (int)Spd.Source, RealSpd, "White", true);
            AddText(tipData, "幸运", (int)Luk.Source, RealLuk, "White", false);
            tipData.AddTextNewLine(string.Format("生命值 {0} / {1}", Life, Avatar.Hp), "Lime");

            foreach (var memBaseSkill in SkillManager.Skills)
            {
                tipData.AddImageNewLine(SkillBook.GetSkillImage(memBaseSkill.SkillId));

                string tp = string.Format("{0}:{1}{2}", memBaseSkill.SkillInfo.Name, memBaseSkill.SkillInfo.Descript, memBaseSkill.Percent == 100 ? "" : string.Format("({0}%)", memBaseSkill.Percent));
                if (tp.Length > 20)
                {
                    tipData.AddText(tp.Substring(0, 19), "White");
                    tipData.AddTextNewLine(tp.Substring(19), "White");
                }
                else
                {
                    tipData.AddText(tp, "White");
                }
            }
            if (TWeapon.CardId > 0)
            {
                tipData.AddImageNewLine(TWeapon.GetImage(16, 16));

                string tp = string.Format("{0}({1}/{2}):{3}", TWeapon.Avatar.WeaponConfig.Name, TWeapon.Life, TWeapon.Avatar.Dura, TWeapon.Avatar);
                tipData.AddText(tp, "White");
            }

            if (!IsGhost)//鬼不显示buff
            {
                BuffManager.DrawBuffToolTip(tipData);
            }

            return tipData.Image;
        }

        private void AddText(ControlPlus.TipImage tipData, string title, int source, int real, string color, bool isLeft)
        {
            if (isLeft)
            {
                tipData.AddTextNewLine(string.Format("{0} {1,3:D}", title, source), color);
            }
            else
            {
                tipData.AddTextOff(string.Format("{0} {1,3:D}", title, source), color, 90);
            }

            int temp = real - source;
            if (temp > 0)
            {
                tipData.AddText(string.Format("+{0,2:D}", temp), "Lime");
            }
            else if (temp < 0)
            {
                tipData.AddText(string.Format("{0,2:D}", temp), "Red");
            }
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
            RemoveAllCover();
            SkillManager.CheckRemoveEffect();
            OwnerPlayer.State.CheckMonsterEvent(false, Avatar);
            Avatar = new Monster(monId);
            Avatar.UpgradeToLevel(Level);
            OwnerPlayer.State.CheckMonsterEvent(true, Avatar);
            SetBasicData();
            CheckCover();
            SkillManager.CheckInitialEffect();
            if (cardId > 0)
            {
                AddWeapon(savedWeapon);
            }
            Life = Avatar.Hp * lifp / 100;
        }

        public void AddCardRate(int monId, int rate)
        {
            if (OwnerPlayer is HumanPlayer)
            {
                BattleManager.Instance.BattleInfo.AddCardRate(monId, rate);
            }
        }

        public void AddResource(int type, int count)
        {
            OwnerPlayer.AddResource((GameResourceType)type, count);
            BattleManager.Instance.FlowWordQueue.Add(new FlowResourceInfo(type + 1, count, Position, 20, 50), false);
        }

        public void AddActionRate(double value)
        {
            Action += (int)(GameConstants.BattleActionLimit*value);
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

        public void AddImmune(int buffId)
        {
            BuffManager.AddImmuneRate(buffId, 1);
        }

        public void AddImmuneRate(int buffId, double rate)
        {
            BuffManager.AddImmuneRate(buffId, rate);
        }

        public int GetMonsterCountByRace(int rid)
        {
            return OwnerPlayer.State.GetMonsterCountByType((MonsterCountTypes)(rid + 20));
        }

        public int GetMonsterCountByType(int type)
        {
            return OwnerPlayer.State.GetMonsterCountByType((MonsterCountTypes)(type+10));
        }

        public AttrModifyData Atk { get; set; }

        public AttrModifyData DHit { get; set; }

        public AttrModifyData Def { get; set; }

        public AttrModifyData Hit { get; set; }

        public AttrModifyData Spd { get; set; }

        public int Ats { get; set; }

        public AttrModifyData Mag { get; set; }

        public AttrModifyData Luk { get; set; }

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

        public bool CanAttack
        {
            get { 
                if (!IsHero) 
                return canAttack;
                return WeaponId != 0 && TWeapon.IsAttackWeapon;//英雄只有拿武器才能攻击
            }
            set { canAttack = value; }
        }

        public bool IsShooter
        {
            get { return HasSkill(SkillConfig.Indexer.ShootSkill); }
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

            foreach (Point pos in posLis)
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
