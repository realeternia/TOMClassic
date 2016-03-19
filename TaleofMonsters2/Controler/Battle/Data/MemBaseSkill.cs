﻿using System;
using System.Collections.Generic;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.DataType.Effects;
using TaleofMonsters.DataType.Skills;
using ConfigDatas;
using NarlonLib.Core;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data
{
    internal class MemBaseSkill
    {
        private readonly Dictionary<int, bool> burst;
        private bool onAddTriggered;

        private int lastCastSpecialTime; //上次施放special技能的时间

        public SkillSourceTypes Type { get; set; } //技能来源，天生/武器
        public int SkillId { get; private set; }
        public int Percent { get; private set; }
        public int Level { private get; set; }

        public MemBaseSkill(Skill skill, int per)
        {
            SkillInfo = skill;
            SkillId = skill.Id;
            Percent = per;
            burst= new Dictionary<int, bool>();
            lastCastSpecialTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
        }

        public LiveMonster Self { get; set; }

        public bool IsBurst(int keyid)
        {
            if (!burst.ContainsKey(keyid))
            {
                return false;
            }
            return burst[keyid];
        }

        public Skill SkillInfo { get; private set; }

        public SkillConfig SkillConfig
        {
            get { return SkillInfo.SkillConfig; }
        }

        private bool CheckRate()
        {
            return Percent > MathTool.GetRandom(100);
        }

        public void CheckBurst(LiveMonster src, LiveMonster dest,bool isActive)
        {
            bool isBurst = CheckRate();

            if (Self == src && SkillInfo.SkillConfig.Active == SkillActiveType.Passive)
            {
                isBurst = false;
            }
            else if (Self == dest && SkillInfo.SkillConfig.Active == SkillActiveType.Active)
            {
                isBurst = false;
            }

            if (SkillInfo.SkillConfig.CanBurst != null && !SkillInfo.SkillConfig.CanBurst(src, dest, isActive))
            {
                isBurst = false;
            }

            int key = src.Id + dest.Id;
            burst[key] = isBurst;
        }

        public void CheckInitialEffect()
        {
            if (SkillInfo.SkillConfig.OnAdd != null)
            {
                if (CheckRate())
                {
                    onAddTriggered = true;
                    SkillInfo.SkillConfig.OnAdd(Self, Level);
                    SendEffect();
                    if (SkillInfo.SkillConfig.Effect != "")
                    {
                        BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(SkillInfo.SkillConfig.Effect), Self, false));
                    }
                }
            }
        }

        public void CheckRemoveEffect()
        {
            if (onAddTriggered && SkillInfo.SkillConfig.OnRemove != null)
            {
                SkillInfo.SkillConfig.OnRemove(Self, Level);
            }
        }

        public void CheckHit(LiveMonster src, LiveMonster dest, ref int hit)
        {
            if (SkillInfo.SkillConfig.CheckHit!=null)
            {
                SkillInfo.SkillConfig.CheckHit(src, dest, ref hit, Level);
                SendEffect();
            }
        }

        public void CheckDamage(LiveMonster src, LiveMonster dest, bool isActive, HitDamage damage, ref int minDamage, ref bool deathHit)
        {
            if (SkillInfo.SkillConfig.CheckDamage != null)
            {
                SkillInfo.SkillConfig.CheckDamage(src, dest, isActive, damage, ref minDamage, ref deathHit, Level);
                SendEffect();
            }
        }

        public void CheckHitEffectAfter(LiveMonster src, LiveMonster dest, HitDamage damage)
        {
            if (SkillInfo.SkillConfig.AfterHit != null)
            {
                SkillInfo.SkillConfig.AfterHit(src, dest, damage, !dest.IsAlive, Level);
                SendEffect();
            }
        }

        public bool CheckSpecial()
        {
            if (SkillInfo.SkillConfig.CheckSpecial != null && CheckRate())
            {
                if (SkillInfo.SkillConfig.CanBurst!=null&& !SkillInfo.SkillConfig.CanBurst(Self, null, true))
                {
                    return false;
                }

                int nowTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
                if (nowTime - lastCastSpecialTime < SkillInfo.SkillConfig.SpecialCd* GameConstants.RoundTime/1000)
                {//in cd 
                    return false;
                }

                lastCastSpecialTime = TimeTool.DateTimeToUnixTime(DateTime.Now);

                SkillInfo.SkillConfig.CheckSpecial(Self, Level);
                SendEffect();
                if (SkillInfo.SkillConfig.Effect!="")
                {
                    BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(SkillInfo.SkillConfig.Effect), Self, false));
                }
                return true;
            }
            return false;
        }

        private void SendEffect(int key, string exp)
        {
            if (Self != null && SkillId < 10000 && key != 0)
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowSkillInfo(SkillId, Self.Position, 0, "lime", 20, 50, exp.Replace("+-", "-")), true);
            }
        }

        private void SendEffect()
        {
            SendEffect(-1, "");
        }        

    }
}
