using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMissile;
using TaleofMonsters.Controler.Battle.Data.MemWeapon;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Cards.Weapons;
using TaleofMonsters.Datas.Decks;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;
using TaleofMonsters.Datas.Skills;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    internal class MonsterAction : IMonsterAction
    {
        private LiveMonster self;
        public MonsterAction(LiveMonster mon)
        {
            self = mon;
        }

        public bool IsNight
        {
            get { return BattleManager.Instance.IsNight; }
        }

        public bool IsTileMatching
        {
            get { return self.BuffManager.IsTileMatching; }
        }

        public bool IsElement(string ele)
        {
            return (int)Enum.Parse(typeof(CardElements), ele) == self.Avatar.MonsterConfig.Type;
        }

        public bool IsRace(string rac)
        {
            return (int)Enum.Parse(typeof(CardTypeSub), rac) == self.Avatar.MonsterConfig.Type;
        }

        public void AddItem(int itemId)
        {
            if (self.OwnerPlayer is HumanPlayer)
            {
                BattleManager.Instance.StatisticData.AddItemGet(itemId);
                UserProfile.InfoBag.AddItem(itemId, 1);
                BattleManager.Instance.FlowWordQueue.Add(new FlowItemInfo(itemId, self.Position, 20, 50));
            }
        }

        public void Transform(int monId)
        {
            if (self.ResistBuffType(BuffImmuneGroup.Mental))//建筑100%抵抗
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("抵抗", self.Position, 0, "Gold", 26, 0, 0, 1, 15));
                return;
            }

            int cardId = self.Weapon == null ? 0 : self.Weapon.CardId;
            var savedWeapon = self.Weapon == null ? null : self.Weapon.GetCopy();
            self.DeleteWeapon(false);
            int lifp = self.Hp * 100 / self.Avatar.Hp;
            self.CoverBox.RemoveAllCover();
            self.SkillManager.CheckRemoveEffect();
            self.OwnerPlayer.Modifier.CheckMonsterEvent(false, self);
            self.Avatar = new Monster(monId);
            self.Avatar.UpgradeToLevel(self.Level);
            self.OwnerPlayer.Modifier.CheckMonsterEvent(true, self);
            self.SetBasicData();
            self.CoverBox.CheckCover();
            self.SkillManager.CheckInitialEffect();
            if (cardId > 0)
                self.AddWeapon(savedWeapon);
            self.HpBar.SetHp(self.Avatar.Hp * lifp / 100);
        }

        public void ChangeAI(string type)
        {
            self.SetAIMode((MonsterAi.AiModes)Enum.Parse(typeof(MonsterAi.AiModes), type));
        }

        public void SetPrepareTime(float rate)
        {
            self.PrepareAtsNeed = (int)(GameConstants.PrepareAts*rate);
        }

        public void Return(int costChange)
        {
            if (self.Avatar.MonsterConfig.IsBuilding)
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("抵抗", self.Position, 0, "Gold", 26, 0, 0, 1, 15));
                return;
            }

            BattleManager.Instance.MemMap.UpdateCellOwner(self.Position, 0);
            self.SkillManager.CheckRemoveEffect();
            self.Owner.Action.AddCard(self, self.CardId, self.Level, costChange);

            self.CoverBox.RemoveAllCover();
            BattleManager.Instance.MonsterQueue.RemoveDirect(self.Id);
        }

        public void AddWeapon(int weaponId, int lv)
        {
            if (!self.CanAddWeapon())
                return;

            Weapon wpn = new Weapon(weaponId);
            wpn.UpgradeToLevel(lv);
            var tWeapon = new TrueWeapon(self, lv, wpn);
            self.AddWeapon(tWeapon);
        }

        public void StealWeapon(IMonster target)
        {
            var monster = target as LiveMonster;
            if (monster != null)
            {
                var weapon = monster.Weapon;
                self.AddWeapon(weapon);
                target.Action.BreakWeapon();
            }
        }

        public void BreakWeapon()
        {
            if (self.Weapon != null)
                self.DeleteWeapon(true);
        }

        public void WeaponReturn()
        {
            if (self.Weapon is TrueWeapon)
            {
                ActiveCard card = new ActiveCard(self.Weapon.CardId, (byte)self.Weapon.Level);
                self.OwnerPlayer.HandCards.AddCard(card);
                self.DeleteWeapon(false);
            }
        }

        public void LevelUpWeapon(int lv)
        {
            if (self.Weapon != null)
            {
                var weaponId = self.Weapon.CardId;
                var weaponLevel = self.Weapon.Level;
                BreakWeapon();
                AddWeapon(weaponId, weaponLevel + lv);
            }
        }

        public void AddRandSkill()
        {
            self.SkillManager.AddSkill(SkillBook.GetRandSkillId(), self.Level, 100, SkillSourceTypes.Skill);
        }

        public void AddSkill(int skillId, int rate)
        {
            self.SkillManager.AddSkill(skillId, self.Level, rate, SkillSourceTypes.Skill);
        }

        public int GetMonsterCountByRace(int rid)
        {
            int count = 0;
            foreach (var checkMon in BattleManager.Instance.MonsterQueue.Enumerator)
            {
                if (checkMon.Type == rid)
                    count++;
            }
            return count;
        }

        public int GetMonsterCountByType(int type)
        {
            int count = 0;
            foreach (var checkMon in BattleManager.Instance.MonsterQueue.Enumerator)
            {
                if (checkMon.Attr == type)
                    count++;
            }
            return count;
        }

        public void AddMissile(IMonster target, int attr, double damage, string arrow)
        {
            BasicMissileControler controler = new TraceMissileControler(self, target as LiveMonster);
            Missile mi = new Missile(arrow, self.Position.X, self.Position.Y, controler, attr, (float)damage);
            BattleManager.Instance.MissileQueue.Add(mi);
        }

        public void Disappear()
        {
            if (self.IsGhost)
                self.GhostTime = 1;//让坟场消失
        }

        public void AddBuff(int buffId, int blevel, double dura)
        {
            self.BuffManager.AddBuff(buffId, blevel, dura);
        }

        public void ClearDebuff()
        {
            self.BuffManager.ClearBuff(true);
        }

        public void ExtendDebuff(double count)
        {
            self.BuffManager.ExtendDebuff(count);
        }

        public bool HasBuff(int buffid)
        {
            return self.BuffManager.HasBuff(buffid);
        }

        public void SetToPosition(string type, int step)
        {
            if (self.ResistBuffType(BuffImmuneGroup.Physical)) //建筑100%抵抗
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("抵抗", self.Position, 0, "Gold", 26, 0, 0, 1, 15));
                return;
            }

            Point dest = MonsterPositionHelper.GetAvailPoint(self.Position, type, self.IsLeft, step);
            if (dest.X != self.Position.X || dest.Y != self.Position.Y)
                BattleLocationManager.SetToPosition(self, dest);
        }


        public void SuddenDeath()
        {
            if (self.ResistBuffType(BuffImmuneGroup.Life))//建筑100%抵抗
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("抵抗", self.Position, 0, "Gold", 26, 0, 0, 1, 15));
                return;
            }

            self.HpBar.SetHp(0);
        }

        public void Rebel()
        {
            if (self.ResistBuffType(BuffImmuneGroup.Mental))//建筑100%抵抗
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("抵抗", self.Position, 0, "Gold", 26, 0, 0, 1, 15));
                return;
            }
            self.IsLeft = !self.IsLeft;
        }

        public void Summon(string type, int id, int count)
        {
            if (self.IsSummoned)
            {//召唤生物不能继续召唤，防止无限循环
                return;
            }

            List<Point> posLis = MonsterPositionHelper.GetAvailPointList(self.Position, type, self.IsLeft, count);

            foreach (var pos in posLis)
            {
                var mon = new Monster(id);
                mon.UpgradeToLevel(self.Level);
                LiveMonster newMon = new LiveMonster(self.Level, mon, pos, self.IsLeft);
                newMon.IsSummoned = true;
                BattleManager.Instance.MonsterQueue.AddDelay(newMon);
            }
        }

        public void SummonRandomAttr(string type, int attr, int star)
        {
            int cardId;
            while (true)
            {
                cardId = CardConfigManager.GetRandomAttrStarCard(attr, star);
                if (CardConfigManager.GetCardConfig(cardId).Type == CardTypes.Monster)
                    break;
            }
            Summon(type, cardId, 1);
        }

        public void SummonRandomRace(string type, int race, int star)
        {
            int cardId;
            while (true)
            {
                cardId = CardConfigManager.GetRandomRaceStarCard(race, star);
                if (CardConfigManager.GetCardConfig(cardId).Type == CardTypes.Monster)
                    break;
            }
            Summon(type, cardId, 1);
        }

        public void MadDrug()
        {
            if (!self.Avatar.MonsterConfig.IsBuilding && self.RealRange > 0)
                self.Atk = self.MaxHp / 5;
        }

        public void CureRandomAlien(double rate)
        {
            IMonster target = null;
            foreach (var checkMon in self.Map.GetAllMonster(self.Position))
            {
                if (checkMon.IsLeft == self.IsLeft && checkMon.HpRate < 100 && checkMon.Id != self.Id)
                {
                    if (target == null || target.HpRate > checkMon.HpRate)
                        target = checkMon;
                }
            }
            if (target != null)
            {
                target.AddHpRate(rate);
                BattleManager.Instance.EffectQueue.Add(new MonsterBindEffect(EffectBook.GetEffect("yellowstar"), (LiveMonster)target, false));
            }
        }

        public void EatTomb(IMonster tomb)
        {
            (tomb as LiveMonster).Action.Disappear();
        }

        //遗忘所有武器技能除外
        public void Silent()
        {
            self.SkillManager.Silent();
            self.BuffManager.ClearBuff(false);//清除所有buff
            self.RemoveAllAttrModify();
        }

        public IMonsterAuro AddAuro(int buff, int lv, string tar)
        {
            return self.AuroManager.AddAuro(buff, lv, tar);
        }

        public void AddPArmor(double val)
        {
            self.HpBar.AddPArmor((int)val);
        }

        public void AddMArmor(double val)
        {
            self.HpBar.AddMArmor((int)val);
        }

        public int GetPArmor()
        {
            return self.HpBar.PArmor;
        }

        public void AddAttrModify(string tp, int itemId, string attr, double val)
        {
            int tpId = (int)Enum.Parse(typeof (LiveMonster.AttrModifyInfo.AttrModifyTypes), tp);
            int attrId = (int)Enum.Parse(typeof(LiveMonster.AttrModifyInfo.AttrTypes), attr);
            self.AddAttrModify(tpId, itemId, attrId, (int)val);
        }

        public void AddMaxHp(string tp, int itemId, double value)
        {
            if (self.Avatar.MonsterConfig.IsBuilding)
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("抵抗", self.Position, 0, "Gold", 26, 0, 0, 1, 15));
                return;
            }

            var lifeRate = self.Hp / self.RealMaxHp;
            int tpId = (int)Enum.Parse(typeof(LiveMonster.AttrModifyInfo.AttrModifyTypes), tp);
            self.AddAttrModify(tpId, itemId, (int)LiveMonster.AttrModifyInfo.AttrTypes.MaxHp, (int)value);
            if (value > 0)
                self.AddHp(value);//顺便把hp也加上
            else
                self.AddHp(lifeRate * value);
        }
    }
}