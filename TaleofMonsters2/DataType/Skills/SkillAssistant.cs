using System;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Buffs;
using ConfigDatas;

namespace TaleofMonsters.DataType.Skills
{
    static class SkillAssistant
    {
        public static void CheckBurst(LiveMonster src, LiveMonster dest)
        {
            src.SkillManager.CheckBurst(src, dest);
            dest.SkillManager.CheckBurst(src, dest);
        }

        public static int GetHit(LiveMonster src, LiveMonster dest)
        {
            int rhit = GameConstants.DefaultHitRate + (src.RealHit - dest.RealDHit)*GameConstants.HitToRate;
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
            if (dest.Avatar.MonsterConfig.AttrDef != null)
            {
                attrRateOn -= dest.Avatar.MonsterConfig.AttrDef[src.AttackType];
            }

            if (src.RealCrt > 0)//存在暴击率
            {
                if (MathTool.GetRandom(100) < src.RealCrt * GameConstants.CrtToRate)
                {
                    attrRateOn *= GameConstants.DefaultCrtDamage;
                }
            }

            if (!src.IsMagicAtk)
            {
                var damValue = (int)(src.RealAtk*(100-dest.RealDef*GameConstants.DefToRate)/100f * attrRateOn);
                var noDefDamValue = (int)(src.RealAtk * attrRateOn);
                damage = new HitDamage(damValue, noDefDamValue, 0, DamageTypes.Physical);
            }
            else
            {
                var damValue = (int)(src.RealAtk * (100 - src.RealMag * GameConstants.MagToRate) / 100f * attrRateOn);
                var noDefDamValue = (int)(src.RealAtk * attrRateOn);
                damage = new HitDamage(damValue, noDefDamValue, src.AttackType, DamageTypes.Magic);
                dest.CheckMagicDamage(damage);
            }

            bool nodef = false; //无视防御
            if (!src.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
            {
                src.SkillManager.CheckDamage(src, dest, true, damage, ref nodef);
            }
            if (!dest.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
            {
                dest.SkillManager.CheckDamage(src, dest, false, damage, ref nodef);
            }

            damage.SetDamage(DamageTypes.All, damage.Value);
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

        /// <summary>
        /// 看下光环的影响和地形影响
        /// </summary>
        public static void CheckAuroState(LiveMonster src, bool tileMatching)
        {
            if (!src.BuffManager.HasBuff(BuffEffectTypes.NoSkill))
            {
                src.CheckAuroEffect();
            }

            if (tileMatching || src.HasSkill(GameConstants.SkillTileId))//地形
            {
                src.AddBuff(BuffConfig.Indexer.Tile, 1, 0.05);
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

