using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.DataTent;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;
using TaleofMonsters.Datas.Skills;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    internal class SkillManager : ISubscribeUser
    {
        public List<MemBaseSkill> SkillList { get; private set; }
        private LiveMonster self;
        private Skill skill1Data;

        public bool IsSilent { get; set; }//处于遗忘状态，遗忘除了武器外的技能

        public SkillManager(LiveMonster lm)
        {
            SkillList = new List<MemBaseSkill>();
            self = lm;

            BattleManager.Instance.EventMsgQueue.Subscribe(this);
        }

        public bool HasSkill(int sid)
        {
            foreach (var skill in SkillList)
            {
                if (skill.SkillId == sid)
                    return true;
            }
            return false;
        }

        public void Reload()
        {
            SkillList = GetMemSkillDataForMonster();
        }

        private List<MemBaseSkill> GetMemSkillDataForMonster()
        {
            List<MemBaseSkill> skills = new List<MemBaseSkill>();
            foreach (var skill in MonsterBook.GetSkillList(self.Avatar.MonsterConfig.Id))
            {
                int monLevel = self.Level;
                Skill skillData = new Skill(self, skill.Id);
                skillData.UpgradeToLevel(monLevel);
                MemBaseSkill baseSkill = new MemBaseSkill(self, skillData, skill.Value, monLevel, SkillSourceTypes.Monster);
                skills.Add(baseSkill);
            }

            return skills;
        }

        public void AddSkillBeforeInit(List<RLIdValue> skills, SkillSourceTypes type)
        {
            foreach (var skillInfo in skills)
            {
                Skill skill = new Skill(self, skillInfo.Id);
                skill.UpgradeToLevel(self.Level);
                MemBaseSkill skillbase = new MemBaseSkill(self, skill, skillInfo.Value, self.Level, type);
                SkillList.Add(skillbase);
            }

        }

        public void AddSkill(int sid, int slevel, int rate, SkillSourceTypes type)
        {
            foreach (var skill in SkillList)
            {
                if (skill.SkillId == sid)
                {
                    SkillList.Remove(skill);
                    break;
                }
            }

            skill1Data = new Skill(self, sid);
            skill1Data.UpgradeToLevel(slevel);
            MemBaseSkill skillbase = new MemBaseSkill(self, skill1Data, rate, slevel, type);
            skillbase.CheckInitialEffect();
            SkillList.Add(skillbase);
        }

        public void RemoveSkill(int sid)
        {
            foreach (var skill in SkillList)
            {
                if (skill.SkillId == sid)
                {
                    skill.CheckRemoveEffect();
                    SkillList.Remove(skill);
                    break;
                }
            }
        }

        public void Silent()
        {
            IsSilent = true;
            CheckRemoveEffect();
        }

        public void CheckCover(List<MonsterBindEffect> coverEffectList)
        {
            foreach (var skill in SkillList)//技能造成的特效
            {
                if (skill.SkillConfig.Cover != "")
                {
                    MonsterBindEffect ef = new MonsterBindEffect(EffectBook.GetEffect(skill.SkillConfig.Cover), self, true);
                    ef.Repeat = true;
                    BattleManager.Instance.EffectQueue.Add(ef);
                    coverEffectList.Add(ef);
                }
            }
        }

        public void CheckInitialEffect()
        {
            foreach (var skill in SkillList.ToArray())
                skill.CheckInitialEffect();
        }

        public void CheckRemoveEffect()
        {
            if(IsSilent)
                return;
            foreach (var skill in SkillList.ToArray())
                skill.CheckRemoveEffect();
        }

        public bool CheckSpecial(float pastRound)
        {
            if (self.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
                return false;

            foreach (var skill in SkillList.ToArray())
            {
                if (IsSilent && skill.Type != SkillSourceTypes.Weapon)
                    continue;

                if (skill.CheckSpecial(pastRound))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 判定技能释放状态
        /// </summary>
        /// <param name="src">攻击者</param>
        /// <param name="dest">受击者</param>
        /// <param name="isMelee">是否是近战攻击（否则远程）</param>
        public void CheckBurst(LiveMonster src, LiveMonster dest, bool isMelee)
        {
            foreach (var skill in SkillList.ToArray())
            {
                if (IsSilent && skill.Type != SkillSourceTypes.Weapon)
                    continue;

                skill.CheckBurst(src, dest, isMelee, true);
            }
        }

        /// <summary>
        /// 判定hit技能影响
        /// </summary>
        /// <param name="src">攻击者</param>
        /// <param name="dest">受击者</param>
        public void CheckHit(LiveMonster src, LiveMonster dest)
        {
            foreach (var skill in SkillList.ToArray())
            {
                var key = MemBaseSkill.GetBurstKey(src.Id, dest.Id);
                if (skill.IsBurst(key))
                    skill.CheckHit(src, dest, key);
            }
        }

        /// <summary>
        /// 判定伤害技能影响
        /// </summary>
        /// <param name="src">攻击者</param>
        /// <param name="dest">受击者</param>
        /// <param name="isActive">是否主动</param>
        /// <param name="damage">伤害值</param>
        public void CheckDamage(LiveMonster src, LiveMonster dest, bool isActive, HitDamage damage)
        {
            foreach (var skill in SkillList.ToArray())
            {
                var key = MemBaseSkill.GetBurstKey(src.Id, dest.Id);
                if (skill.IsBurst(key))
                    skill.CheckDamage(src, dest, isActive, damage, key);
            }
        }

        /// <summary>
        /// 判定伤害后技能影响
        /// </summary>
        /// <param name="src">攻击者</param>
        /// <param name="dest">受击者</param>
        /// <param name="damage">伤害值</param>
        public void CheckHitEffectAfter(LiveMonster src, LiveMonster dest, HitDamage damage)
        {
            foreach (var skill in SkillList.ToArray())
            {
                var key = MemBaseSkill.GetBurstKey(src.Id, dest.Id);
                if (skill.IsBurst(key))
                    skill.CheckHitEffectAfter(src, dest, damage, key);
            }
        }

        public int GetRoundSkillPercent()
        {
            foreach (var skill in SkillList.ToArray())
            {
                var round = skill.GetPercent();
                if (round > 0)
                    return round;
            }
            return 0;
        }

        public void OnMessage(EventMsgQueue.EventMsgTypes type, IPlayer p, IMonster src, IMonster dest, HitDamage damage, Point l, int cardId, int cardType, int cardLevel)
        {
            if (self.BuffManager.HasBuff(BuffEffectTypes.NoSkill) || IsSilent)
                return;

            foreach (var skill in SkillList)
            {
                var skillConfig = ConfigData.GetSkillConfig(skill.SkillId);
                if(string.IsNullOrEmpty(skillConfig.EventType))
                    continue;
                var typeV = (EventMsgQueue.EventMsgTypes)Enum.Parse(typeof(EventMsgQueue.EventMsgTypes), skillConfig.EventType);
                if (typeV == type)
                {
                    skill.CheckEvent(p, src, dest, damage, cardId, cardType, cardLevel);
                }
            }
        }
    }
}
