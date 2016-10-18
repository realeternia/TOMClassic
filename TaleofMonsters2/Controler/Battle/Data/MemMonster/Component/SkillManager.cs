using System.Collections.Generic;
using ConfigDatas;
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
        private LiveMonster self;

        public SkillManager(LiveMonster lm)
        {
            Skills = new List<MemBaseSkill>();
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

        public bool CheckSpecial(float pastRound)
        {
            if (self.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
            {
                return false;
            }

            foreach (MemBaseSkill skill in Skills.ToArray())
            {
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
            foreach (MemBaseSkill skill in Skills.ToArray())
            {
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
            foreach (MemBaseSkill skill in Skills.ToArray())
            {
                var key = MemBaseSkill.GetBurstKey(src.Id, dest.Id);
                if (skill.IsBurst(key))
                {
                    skill.CheckHit(src, dest, ref rhit, key);
                }
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
                {
                    skill.CheckDamage(src, dest, isActive, damage, ref nodef, key);
                }
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
            foreach (MemBaseSkill skill in Skills.ToArray())
            {
                var key = MemBaseSkill.GetBurstKey(src.Id, dest.Id);
                if (skill.IsBurst(key))
                {
                    skill.CheckHitEffectAfter(src, dest, damage, key);
                }
            }
        }

        public int GetRoundSkillPercent()
        {
            foreach (MemBaseSkill skill in Skills.ToArray())
            {
                var round = skill.GetPercent();
                if (round > 0)
                {
                    return round;
                }
            }
            return 0;
        }
    }
}
