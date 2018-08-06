using System;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Formulas;

namespace TaleofMonsters.Datas.Skills
{
    internal static class SkillAssistant
    {
        public static void CheckBurst(LiveMonster src, LiveMonster dest, bool isMelee)
        {
            src.SkillManager.CheckBurst(src, dest, isMelee);
            dest.SkillManager.CheckBurst(src, dest, isMelee);
        }

        public static int GetHit(LiveMonster src, LiveMonster dest)
        {
            HitData hit = new HitData(GameConstants.DefaultHitRate + (src.RealHit - dest.RealDHit) * GameConstants.HitToRate);
            if (!src.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
                src.SkillManager.CheckHit(src, dest, hit);
            if (!dest.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
                dest.SkillManager.CheckHit(src, dest, hit);
            return Math.Max(hit.Value, 0);
        }

        public static DamageData GetDamage(LiveMonster src, LiveMonster dest)
        {
            DamageData damage;
            double attrRateOn = 1; //属性相克的伤害修正
            bool isCrt = false;
            if (dest.Avatar.MonsterConfig.AttrDef != null)
                attrRateOn -= dest.Avatar.MonsterConfig.AttrDef[src.AttackType];

            if (src.RealCrt > 0)//存在暴击率
            {
                if (MathTool.GetRandom(100) < src.RealCrt * GameConstants.CrtToRate)
                {
                    attrRateOn *= (GameConstants.DefaultCrtDamage + src.CrtDamAddRate);
                    isCrt = true;
                }
            }

            var realAttackType = MonsterBook.HasTag(src.CardId, "mattack") ? src.Attr : src.AttackType;
            if (realAttackType == 0)//物理攻击
            {
                var damValue = Math.Max(1, (int)(src.RealAtk*(1 - FormulaBook.GetPhyDefRate(dest.RealDef)) * attrRateOn));//至少有1点伤害
                var noDefDamValue = (int)(src.RealAtk * attrRateOn);
                damage = new DamageData(damValue, noDefDamValue, 0, DamageTypes.Physical);
            }
            else
            {
                var damValue = (int)(src.RealAtk * (1 + src.RealMag * GameConstants.MagToRate) * (1 - FormulaBook.GetMagDefRate(dest.RealMag)) * attrRateOn);
                damage = new DamageData(damValue, damValue, realAttackType, DamageTypes.Magic);
                dest.CheckMagicDamage(damage);
            }
            damage.IsCrt = isCrt;

            if (!src.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
                src.SkillManager.CheckDamage(src, dest, true, damage);
            if (!dest.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
                dest.SkillManager.CheckDamage(src, dest, false, damage);

            damage.FinalCheck();
            return damage;
        }

        public static void CheckHitEffectAfter(LiveMonster src, LiveMonster dest, DamageData dam)
        {
            if (!src.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
                src.SkillManager.CheckHitEffectAfter(src, dest, dam);

            if (!dest.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
                dest.SkillManager.CheckHitEffectAfter(src, dest, dam);
        }

        /// <summary>
        /// 看下光环的影响和地形影响
        /// </summary>
        public static void CheckAuroState(LiveMonster src, bool tileMatching)
        {
            if (!src.SkillManager.IsSilent && !src.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
                src.AuroManager.CheckAuroEffect();

            if (tileMatching || src.HasSkill(SkillConfig.Indexer.TileChange))//地形
                src.BuffManager.AddBuff(BuffConfig.Indexer.Tile, 1, 0.05);
        }

    }
}

