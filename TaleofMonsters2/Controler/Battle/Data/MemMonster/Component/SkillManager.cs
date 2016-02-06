using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Core;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Effects;
using TaleofMonsters.DataType.Skills;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    internal class SkillManager
    {
        public List<MemBaseSkill> Skills { get; private set; }
        private SimpleSet<SkillMarks> specialMarks;
        private LiveMonster self;

        public SkillManager(LiveMonster lm)
        {
            Skills = new List<MemBaseSkill>();
            specialMarks = new SimpleSet<SkillMarks>();
            self = lm;
        }

        public bool HasSkill(int sid)
        {
            foreach (var sk in Skills)
            {
                if (sk.SkillId == sid)
                    return true;
            }
            return false;
        }

        public void Reload()
        {
            Skills = GetMemSkillDataForMonster();
            specialMarks = new SimpleSet<SkillMarks>();
        }

        private List<MemBaseSkill> GetMemSkillDataForMonster()
        {
            List<MemBaseSkill> skills = new List<MemBaseSkill>();
            for (int i = 0; i < self.Avatar.MonsterConfig.Skills.Count; i++)
            {
                RLVector3 skillData = self.Avatar.MonsterConfig.Skills[i];
                int lukLevel = skillData.Z != 0 ? skillData.Z : self.Level;
                Skill luk = new Skill(skillData.X);
                luk.UpgradeToLevel(lukLevel);
                MemBaseSkill baseSkill = new MemBaseSkill(luk, skillData.Y) { Self = self, Level = lukLevel };
                skills.Add(baseSkill);
            }

            return skills;
        }

        public void AddSkill(int sid, int slevel, int rate, SkillSourceTypes type)
        {
            foreach (MemBaseSkill memSkill in Skills)
            {
                if (memSkill.SkillId == sid)
                {
                    Skills.Remove(memSkill);
                    break;
                }
            }

            Skill skill = new Skill(sid);
            skill.UpgradeToLevel(slevel);
            MemBaseSkill skillbase = new MemBaseSkill(skill, rate);
            skillbase.Type = type;
            skillbase.Level = slevel;
            skillbase.Self = self;
            skillbase.CheckInitialEffect();
            Skills.Add(skillbase);
        }

        public void RemoveSkill(int sid)
        {
            foreach (MemBaseSkill memSkill in Skills)
            {
                if (memSkill.SkillId == sid)
                {
                    Skills.Remove(memSkill);
                    break;
                }
            }
        }
        
        public void AddSpecialMark(SkillMarks mark)
        {
            if (!HasSpecialMark(mark))
                specialMarks.Add(mark);
        }

        public bool HasSpecialMark(SkillMarks mark)
        {
            return specialMarks.Has(mark);
        }

        public void Forget()
        {
            Skills.RemoveAll(skill => skill.Type != SkillSourceTypes.Weapon);
        }

        public void CheckCover(List<ActiveEffect> coverEffectList)
        {
            foreach (MemBaseSkill memBaseSkill in Skills)//技能造成的特效
            {
                if (memBaseSkill.SkillConfig.Cover != "")
                {
                    ActiveEffect ef = new ActiveEffect(EffectBook.GetEffect(memBaseSkill.SkillConfig.Cover), self, true);
                    ef.Repeat = true;
                    BattleManager.Instance.EffectQueue.Add(ef);
                    coverEffectList.Add(ef);
                }
            }
        }

        public void CheckInitialEffect()
        {
            foreach (MemBaseSkill skill in Skills.ToArray())
            {
                skill.CheckInitialEffect();
            }
        }

        public void CheckRemoveEffect()
        {
            foreach (MemBaseSkill skill in Skills.ToArray())
            {
                skill.CheckRemoveEffect();
            }
        }

        public bool CheckSpecial()
        {
            if (self.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
            {
                return false;
            }

            foreach (MemBaseSkill skill in Skills.ToArray())
            {
                if (skill.CheckSpecial())
                    return true;
            }
            return false;
        }

        public void CheckBurst(LiveMonster target, bool isActive)
        {
            foreach (MemBaseSkill skill in Skills.ToArray())
            {
                skill.CheckBurst(self, target, isActive);
            }
        }

        public void CheckHit(LiveMonster src, LiveMonster dest, ref int rhit)
        {
            foreach (MemBaseSkill skill in Skills.ToArray())
            {
                if (skill.IsBurst(src.Id + dest.Id))
                {
                    skill.CheckHit(src, dest, ref rhit);
                }
            }
        }

        public void CheckDamage(LiveMonster src, LiveMonster dest, bool isActive, HitDamage damage, ref int minDamage, ref bool deathHit)
        {
            foreach (var skill in Skills.ToArray())
            {
                if (skill.IsBurst(src.Id + dest.Id))
                {
                    skill.CheckDamage(src, dest, true, damage, ref minDamage, ref deathHit);
                }
            }
        }

        public void CheckHitEffectAfter(LiveMonster src, LiveMonster dest, HitDamage damage)
        {
            foreach (MemBaseSkill skill in Skills.ToArray())
            {
                if (skill.IsBurst(src.Id + dest.Id))
                {
                    skill.CheckHitEffectAfter(src, dest, damage);
                }
            }
        }

    }
}
