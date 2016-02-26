using System;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Buffs;
using TaleofMonsters.DataType.Formulas;
using ConfigDatas;

namespace TaleofMonsters.DataType.Skills
{
    static class SkillAssistant
    {
        public static void CheckBurst(LiveMonster src, LiveMonster dest)
        {
            src.SkillManager.CheckBurst(dest, true);
            dest.SkillManager.CheckBurst(src, false);
        }

        public static int GetHit(LiveMonster src, LiveMonster dest)
        {
            int rhit = GameConstants.DefaultHitRate + src.RealHit - dest.RealDHit;
            if (!src.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
            {
                src.SkillManager.CheckHit(src, dest, ref rhit);
            }
            if (!dest.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
            {
                dest.SkillManager.CheckHit(src, dest, ref rhit);
            }
            return Math.Max(rhit, 0);
        }

        public static HitDamage GetDamage(LiveMonster src, LiveMonster dest)
        {
            HitDamage damage;
            double attrRateOn = 1; //属性相克的伤害修正
            if (src.Avatar.MonsterConfig.AttrAtk != null)//目前英雄可能为null
            {
                attrRateOn += src.Avatar.MonsterConfig.AttrAtk[dest.Avatar.MonsterConfig.Attr];
            }
            if (dest.Avatar.MonsterConfig.AttrDef != null)
            {
                attrRateOn -= dest.Avatar.MonsterConfig.AttrDef[src.AttackType];
            }

            if (!src.IsMagicAtk)
            {
                var damvalue = (int)(FormulaBook.GetPhysicalDamage(src.RealAtk, dest.RealDef)* attrRateOn);
                damage = new HitDamage(damvalue, 0, DamageTypes.Physical);
            }
            else
            {
                var damvalue = (int)(FormulaBook.GetMagicDamage(src.RealMag, dest.RealMag) * attrRateOn);
                damage = new HitDamage(damvalue, src.AttackType, DamageTypes.Magic);
                dest.CheckMagicDamage(damage);
            }

            bool deathHit = false; //一击必杀
            int minDamage = 1; //最小伤害
            if (!src.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
            {
                src.SkillManager.CheckDamage(src, dest, true, damage, ref minDamage, ref deathHit);
            }
            if (!dest.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
            {
                dest.SkillManager.CheckDamage(src, dest, false, damage, ref minDamage, ref deathHit);
            }

            damage.SetDamage(DamageTypes.All, deathHit ? dest.Life : Math.Max(damage.Value, minDamage));
            return damage;
        }

        public static void CheckHitEffectAfter(LiveMonster src, LiveMonster dest, HitDamage dam)
        {
            if (!src.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
            {
                src.SkillManager.CheckHitEffectAfter(src, dest, dam);
            }

            if (!dest.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
            {
                dest.SkillManager.CheckHitEffectAfter(src, dest, dam);
            }
        }

        public static void CheckAuroState(LiveMonster src, TileMatchResult tileMatching)
        {
            if (!src.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
            {
                src.CheckAuroEffect();
            }

            if (tileMatching == TileMatchResult.Enhance && !src.BuffManager.HasBuff(BuffEffectTypes.NoTile))//地形
            {
                src.AddBuff((int)BuffIds.Tile, 1, 0.05);
            }
        }

        public static int GetMagicDamage(LiveMonster dest, HitDamage damage)
        {
            if (damage.Dtype == DamageTypes.Magic)
            {
                dest.CheckMagicDamage(damage);
            }
            return damage.Value;
        }

    }
}

