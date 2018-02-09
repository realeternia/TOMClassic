using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Effects;
using TaleofMonsters.DataType.Skills;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    internal class SkillManager
    {
        public List<MemBaseSkill> Skills { get; private set; }
        private LiveMonster self;
        private Skill skill1Data;
        public bool IsSilent { get; set; }//处于遗忘状态，遗忘除了武器外的技能

        public SkillManager(LiveMonster lm)
        {
            Skills = new List<MemBaseSkill>();
            self = lm;
        }

        public bool HasSkill(int sid)
        {
            foreach (var skill in Skills)
            {
                if (skill.SkillId == sid)
                    return true;
            }
            return false;
        }

        public void Reload()
        {
            Skills = GetMemSkillDataForMonster();
        }

        private List<MemBaseSkill> GetMemSkillDataForMonster()
        {
            List<MemBaseSkill> skills = new List<MemBaseSkill>();
            foreach (var skill in MonsterBook.GetSkillList(self.Avatar.MonsterConfig.Id))
            {
                int lukLevel = self.Level;
                Skill luk = new Skill(skill.Id);
                luk.UpgradeToLevel(lukLevel);
                MemBaseSkill baseSkill = new MemBaseSkill(luk, skill.Value) { Self = self, Level = lukLevel };
                skills.Add(baseSkill);
            }

            return skills;
        }

        public void AddSkillBeforeInit(List<RLIdValue> skills, SkillSourceTypes type)
        {
            foreach (var skillInfo in skills)
            {
                Skill skill = new Skill(skillInfo.Id);
                skill.UpgradeToLevel(self.Level);
                MemBaseSkill skillbase = new MemBaseSkill(skill, skillInfo.Value);
                skillbase.Type = type;
                skillbase.Level = self.Level;
                skillbase.Self = self;

                Skills.Add(skillbase);
            }

        }

        public void AddSkill(int sid, int slevel, int rate, SkillSourceTypes type)
        {
            foreach (var skill in Skills)
            {
                if (skill.SkillId == sid)
                {
                    Skills.Remove(skill);
                    break;
                }
            }

            skill1Data = new Skill(sid);
            skill1Data.UpgradeToLevel(slevel);
            MemBaseSkill skillbase = new MemBaseSkill(skill1Data, rate);
            skillbase.Type = type;
            skillbase.Level = slevel;
            skillbase.Self = self;
            skillbase.CheckInitialEffect();
            Skills.Add(skillbase);
        }

        public void RemoveSkill(int sid)
        {
            foreach (var skill in Skills)
            {
                if (skill.SkillId == sid)
                {
                    Skills.Remove(skill);
                    break;
                }
            }
        }

        public void Silent()
        {
            IsSilent = true;
            CheckSilentEffect();
        }

        public void CheckCover(List<ActiveEffect> coverEffectList)
        {
            foreach (var skill in Skills)//技能造成的特效
            {
                if (skill.SkillConfig.Cover != "")
                {
                    ActiveEffect ef = new ActiveEffect(EffectBook.GetEffect(skill.SkillConfig.Cover), self, true);
                    ef.Repeat = true;
                    BattleManager.Instance.EffectQueue.Add(ef);
                    coverEffectList.Add(ef);
                }
            }
        }

        public void CheckInitialEffect()
        {
            foreach (var skill in Skills.ToArray())
                skill.CheckInitialEffect();
        }

        public void CheckRemoveEffect()
        {
            if(IsSilent)
                return;
            foreach (var skill in Skills.ToArray())
                skill.CheckRemoveEffect();
        }

        public void CheckSilentEffect()
        {
            foreach (var skill in Skills.ToArray())
                skill.CheckSilentEffect();
        }

        public bool CheckSpecial(float pastRound)
        {
            if (self.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
                return false;

            foreach (var skill in Skills.ToArray())
            {
                if (IsSilent && skill.Type != SkillSourceTypes.Weapon)
                    continue;

                if (skill.CheckSpecial(pastRound))
                    return true;
            }
            return false;
        }

        public void CheckUseCard(IPlayer caster, int cardType, int lv)
        {
            if (self.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
                return;

            foreach (var skill in Skills.ToArray())
            {
                if (IsSilent && skill.Type != SkillSourceTypes.Weapon)
                    continue;

                skill.OnUseCard(caster, cardType, lv);
            }
        }

        public void DeathSkill()
        {
            if (self.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
                return;

            foreach (var skill in Skills.ToArray())
            {
                if (IsSilent && skill.Type != SkillSourceTypes.Weapon)
                    continue;

                skill.DeathSkill();
            }
        }

        /// <summary>
        /// 判定技能释放状态
        /// </summary>
        /// <param name="src">攻击者</param>
        /// <param name="dest">受击者</param>
        /// <param name="isMelee">是否是近战攻击（否则远程）</param>
        public void CheckBurst(LiveMonster src, LiveMonster dest, bool isMelee)
        {
            foreach (var skill in Skills.ToArray())
            {
                if (IsSilent && skill.Type != SkillSourceTypes.Weapon)
                    continue;

                skill.CheckBurst(src, dest, isMelee);
            }
        }

        /// <summary>
        /// 判定hit技能影响
        /// </summary>
        /// <param name="src">攻击者</param>
        /// <param name="dest">受击者</param>
        /// <param name="rhit">命中</param>
        public void CheckHit(LiveMonster src, LiveMonster dest, ref int rhit)
        {
            foreach (var skill in Skills.ToArray())
            {
                var key = MemBaseSkill.GetBurstKey(src.Id, dest.Id);
                if (skill.IsBurst(key))
                    skill.CheckHit(src, dest, ref rhit, key);
            }
        }

        /// <summary>
        /// 判定伤害技能影响
        /// </summary>
        /// <param name="src">攻击者</param>
        /// <param name="dest">受击者</param>
        /// <param name="isActive">是否主动</param>
        /// <param name="damage">伤害值</param>
        /// <param name="nodef">无视防御</param>
        public void CheckDamage(LiveMonster src, LiveMonster dest, bool isActive, HitDamage damage, ref bool nodef)
        {
            foreach (var skill in Skills.ToArray())
            {
                var key = MemBaseSkill.GetBurstKey(src.Id, dest.Id);
                if (skill.IsBurst(key))
                    skill.CheckDamage(src, dest, isActive, damage, ref nodef, key);
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
            foreach (var skill in Skills.ToArray())
            {
                var key = MemBaseSkill.GetBurstKey(src.Id, dest.Id);
                if (skill.IsBurst(key))
                    skill.CheckHitEffectAfter(src, dest, damage, key);
            }
        }

        public int GetRoundSkillPercent()
        {
            foreach (var skill in Skills.ToArray())
            {
                var round = skill.GetPercent();
                if (round > 0)
                    return round;
            }
            return 0;
        }
    }
}
