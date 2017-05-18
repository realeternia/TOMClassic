using System.Collections.Generic;
using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.DataType.Effects;
using TaleofMonsters.DataType.Skills;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data
{
    internal class MemBaseSkill
    {
        private readonly Dictionary<int, BurstStage> burst;
        private bool onAddTriggered;

        private float castRoundAddon; //累积special技能的时间

        public SkillSourceTypes Type { get; set; } //技能来源，天生/武器
        public int SkillId { get; private set; }
        public int Percent { get; private set; }
        public int Level { private get; set; }
        public LiveMonster Self { get; set; }

        public MemBaseSkill(Skill skill, int per)
        {
            SkillInfo = skill;
            SkillId = skill.Id;
            Percent = per;
            burst= new Dictionary<int, BurstStage>();
        }
        
        public static int GetBurstKey(int srcId, int destId)
        {
            return srcId * -1 + destId;
        }

        public bool IsBurst(int key)
        {
            if (!burst.ContainsKey(key))
            {
                return false;
            }
            return burst[key] != BurstStage.Fail;
        }

        public int GetPercent()
        {
            if (SkillConfig.SpecialCd > 0)
            {
                return (int)(castRoundAddon*100/SkillConfig.SpecialCd);
            }
            return 0;
        }

        public Skill SkillInfo { get; private set; }

        public SkillConfig SkillConfig
        {
            get { return SkillInfo.SkillConfig; }
        }

        private bool CheckRate()
        {
            return MathTool.GetRandom(10000 + Self.RealLuk * GameConstants.LukToRoll) > (100 - Percent) * 100;
        }

        public void CheckBurst(LiveMonster src, LiveMonster dest, bool isMelee)
        {
            var isActive = src == Self;
            if (isActive && SkillInfo.SkillConfig.Active == SkillActiveType.Passive)
            {
                return;
            }
            if (!isActive && SkillInfo.SkillConfig.Active == SkillActiveType.Active)
            {
                return;
            }

            BurstStage isBurst = CheckRate() ? BurstStage.Pass : BurstStage.Fail;
            if (SkillInfo.SkillConfig.CanBurst != null && !SkillInfo.SkillConfig.CanBurst(src, dest, isMelee))
            {
                isBurst = BurstStage.Fail;
            }

            int key = GetBurstKey(src.Id, dest.Id);
            burst[key] = isBurst;
        }

        public void CheckInitialEffect()
        {
            if (SkillInfo.SkillConfig.OnAdd != null)
            {
                if (CheckRate())
                {
                    onAddTriggered = true;
                    SkillInfo.SkillConfig.OnAdd(SkillInfo, Self, Level);
                    SendSkillIcon(0);
                    if (SkillInfo.SkillConfig.Effect != "")
                    {
                        BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(SkillInfo.SkillConfig.Effect), Self, false));
                    }
                    if (SkillInfo.SkillConfig.EffectArea != "")
                    {
                        SendAreaEffect(Self.Position);
                    }
                }
            }
        }

        public void CheckRemoveEffect()
        {
            if (onAddTriggered && SkillInfo.SkillConfig.OnRemove != null)
            {
                SkillInfo.SkillConfig.OnRemove(SkillInfo, Self, Level);
            }
        }

        public void CheckHit(LiveMonster src, LiveMonster dest, ref int hit, int key)
        {
            if (SkillInfo.SkillConfig.CheckHit!=null)
            {
                SkillInfo.SkillConfig.CheckHit(src, dest, ref hit, Level);
                SendSkillIcon(key);
            }
        }

        public void CheckDamage(LiveMonster src, LiveMonster dest, bool isActive, HitDamage damage, ref bool nodef, int key)
        {
            if (SkillInfo.SkillConfig.CheckDamage != null)
            {
                SkillInfo.SkillConfig.CheckDamage(src, dest, isActive, damage, ref nodef, Level);
                SendSkillIcon(key);
            }
        }

        public void CheckHitEffectAfter(LiveMonster src, LiveMonster dest, HitDamage damage, int key)
        {
            if (SkillInfo.SkillConfig.AfterHit != null)
            {
                SkillInfo.SkillConfig.AfterHit(SkillInfo, src, dest, damage, Level);
                SendSkillIcon(key);
                if (SkillInfo.SkillConfig.EffectArea != "")
                {
                    SendAreaEffect(SkillInfo.SkillConfig.PointSelf ? Self.Position : dest.Position);
                }
            }

            if (SkillInfo.SkillConfig.DeathHit != null && !dest.IsAlive)
            {
                SkillInfo.SkillConfig.DeathHit(SkillInfo, src, dest, damage, Level);
                SendSkillIcon(key);
                if (SkillInfo.SkillConfig.EffectArea != "")
                {
                    SendAreaEffect(SkillInfo.SkillConfig.PointSelf ? Self.Position : dest.Position);
                }
            }

        }

        public bool CheckSpecial(float pastRound)
        {
            if (SkillInfo.SkillConfig.CheckSpecial != null)
            {
                castRoundAddon += pastRound;
                if (!CheckRate())
                {
                    return false;
                }

                if (castRoundAddon < SkillInfo.SkillConfig.SpecialCd)
                {//in cd 
                    return false;
                }

                castRoundAddon = (float)(castRoundAddon- SkillInfo.SkillConfig.SpecialCd);

                SkillInfo.SkillConfig.CheckSpecial(SkillInfo, Self, Level);
                SendSkillIcon(0);
                if (SkillInfo.SkillConfig.Effect!="")
                {
                    BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(SkillInfo.SkillConfig.Effect), Self, false));
                }
                if (SkillInfo.SkillConfig.EffectArea != "")
                {
                    SendAreaEffect(Self.Position);
                }
                return true;
            }
            return false;
        }

        public void OnUseCard(IPlayer caster, int cardType, int lv)
        {
            if (SkillInfo.SkillConfig.OnUseCard != null)
            {
                if (!CheckRate())
                {
                    return;
                }

                bool success = false;
                SkillInfo.SkillConfig.OnUseCard(Self, caster, cardType, lv, ref success);
                if (success)
                {
                    SendSkillIcon(0);
                    if (SkillInfo.SkillConfig.Effect != "")
                    {
                        BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(SkillInfo.SkillConfig.Effect), Self, false));
                    }
                }
            }
        }

        private void SendSkillIcon(int key)
        {
            if (key != 0)
            {
                if (burst[key] == BurstStage.Fin)
                {
                    return;//保证一次触发只播放一次
                }
                burst[key] = BurstStage.Fin;
            }

            if (Self != null)
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowSkillInfo(SkillId, Self.Position, 0, "lime", 20, 50, ""), true);
            }
        }

        private void SendAreaEffect(Point pos)
        {
            //播放特效
            RegionTypes rt = BattleTargetManager.GetRegionType(SkillInfo.SkillConfig.Target[2]);
            var cardSize = BattleManager.Instance.MemMap.CardSize;
            foreach (var memMapPoint in BattleManager.Instance.MemMap.Cells)
            {
                var pointData = memMapPoint.ToPoint();
                if (BattleLocationManager.IsPointInRegionType(rt, pos.X, pos.Y, pointData, SkillInfo.SkillConfig.Range, Self.IsLeft))
                {
                    var effectData = new ActiveEffect(EffectBook.GetEffect(SkillInfo.SkillConfig.EffectArea), pointData + new Size(cardSize / 2, cardSize / 2), false);
                    BattleManager.Instance.EffectQueue.Add(effectData);
                }
            }
        }


    }

    internal enum BurstStage
    {
        Fail,
        Pass,
        Fin
    }
}
