using System.Collections.Generic;
using System.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;
using TaleofMonsters.Datas.Skills;

namespace TaleofMonsters.Controler.Battle.Data
{
    internal class MemBaseSkill
    {
        private readonly Dictionary<int, BurstStage> burst;
        private bool onAddTriggered;

        private float castRoundAddon; //累积special技能的时间

        public SkillSourceTypes Type { get; private set; } //技能来源，天生/武器
        public int SkillId { get; private set; }
        public int Percent { get; private set; }
        private int level;
        public LiveMonster Self { get; private set; }
        public Skill SkillInfo { get; private set; }

        public SkillConfig SkillConfig
        {
            get { return SkillInfo.SkillConfig; }
        }

        public MemBaseSkill(LiveMonster lm, Skill skill, int per, int lv, SkillSourceTypes type)
        {
            Self = lm;
            SkillInfo = skill;
            SkillId = skill.Id;
            Percent = per;
            level = lv;
            Type = type;
            burst = new Dictionary<int, BurstStage>();
        }
        
        public static int GetBurstKey(int srcId, int destId)
        {
            return srcId * -1 + destId;
        }

        public bool IsBurst(int key)
        {
            if (!burst.ContainsKey(key))
                return false;
            return burst[key] != BurstStage.Fail;
        }

        public int GetPercent()
        {
            if (SkillConfig.SpecialCd > 0)
                return (int) (castRoundAddon*100/SkillConfig.SpecialCd);
            return 0;
        }
        
        private bool CheckRate()
        {
            return MathTool.GetRandom(10000) > (100 - Percent) * 100;
        }

        public bool CheckBurst(LiveMonster src, LiveMonster dest, bool isMelee, bool needSave)
        {
            var isActive = src == Self;
            if (isActive && SkillInfo.SkillConfig.Active == SkillActiveType.Passive)
                return false;
            if (!isActive && SkillInfo.SkillConfig.Active == SkillActiveType.Active)
                return false;

            BurstStage isBurst = CheckRate() ? BurstStage.Pass : BurstStage.Fail;
            if (SkillInfo.SkillConfig.CanBurst != null && !SkillInfo.SkillConfig.CanBurst(src, dest, isMelee))
                isBurst = BurstStage.Fail;

            if (needSave)
            {
                int key = GetBurstKey(src.Id, dest.Id);
                burst[key] = isBurst;
            }

            return isBurst == BurstStage.Pass;
        }

        public void CheckInitialEffect()
        {
            if (SkillInfo.SkillConfig.OnAdd == null)
                return;
            if (CheckBurst(Self, null, true, false))
            {
                onAddTriggered = true;
                bool result = false;
                SkillInfo.SkillConfig.OnAdd(SkillInfo, Self.Owner, Self, null, null, 0, 0, ref result);
                SendSkillIcon(0);
                if (SkillInfo.SkillConfig.Effect != "")
                    BattleManager.Instance.EffectQueue.Add(new MonsterBindEffect(EffectBook.GetEffect(SkillInfo.SkillConfig.Effect), Self, false));
                if (SkillInfo.SkillConfig.EffectArea != "")
                    SendAreaEffect(Self.Position);
            }
        }

        public void CheckRemoveEffect()
        {
            Self.RemoveAttrModify((int)LiveMonster.AttrModifyInfo.AttrModifyTypes.Skill, SkillId);
            if (!onAddTriggered || SkillInfo.SkillConfig.OnRemove == null)
                return;
            if (CheckBurst(Self, null, true, false))
            {
                bool result = false;
                SkillInfo.SkillConfig.OnRemove(SkillInfo, Self.Owner, Self, null, null, 0, 0, ref result);
            }
        }

        public void CheckHit(LiveMonster src, LiveMonster dest, int key)
        {
            if (SkillInfo.SkillConfig.CheckHit == null)
                return;
            bool result = false;
            SkillInfo.SkillConfig.CheckHit(SkillInfo, src.Owner, src, dest, null, 0, 0, ref result);
            SendSkillIcon(key);
        }

        public void CheckDamage(LiveMonster src, LiveMonster dest, bool isActive, HitDamage damage, int key)
        {
            if (SkillInfo.SkillConfig.CheckDamage == null)
                return;
            bool result = false;
            SkillInfo.SkillConfig.CheckDamage(SkillInfo, src.Owner, src, dest, damage, 0,0, ref result);
            SendSkillIcon(key);
        }

        public void CheckHitEffectAfter(LiveMonster src, LiveMonster dest, HitDamage damage, int key)
        {
            if (SkillInfo.SkillConfig.AfterHit != null)
            {
                bool result = true;
                SkillInfo.SkillConfig.AfterHit(SkillInfo, src.Owner, src, dest, damage, 0, 0, ref result);
                if (result)
                {
                    SendSkillIcon(key);
                    if (SkillInfo.SkillConfig.EffectArea != "")
                        SendAreaEffect(SkillInfo.SkillConfig.PointSelf ? Self.Position : dest.Position);
                }
            }
        }

        public bool CheckSpecial(float pastRound)
        {
            if (SkillInfo.SkillConfig.CheckSpecial == null)
                return false;

            if (!CheckBurst(Self, null, true, false))
                return false;

            castRoundAddon += pastRound;
            if (castRoundAddon < SkillInfo.SkillConfig.SpecialCd)
                return false;//in cd 

            castRoundAddon = (float)(castRoundAddon- SkillInfo.SkillConfig.SpecialCd);

            bool result = false;
            SkillInfo.SkillConfig.CheckSpecial(SkillInfo, Self.Owner, Self, null, null, 0, 0, ref result);
            SendSkillIcon(0);
            if (SkillInfo.SkillConfig.Effect!="")
                BattleManager.Instance.EffectQueue.Add(new MonsterBindEffect(EffectBook.GetEffect(SkillInfo.SkillConfig.Effect), Self, false));
            if (SkillInfo.SkillConfig.EffectArea != "")
                SendAreaEffect(Self.Position);
            return true;
        }

        public void CheckEvent(IPlayer p, IMonster src, IMonster dest, HitDamage damage, int cardId, int cardType, int cardLevel)
        {
            if (!CheckBurst(Self, null, true, false))
                return;

            bool result = false;
            SkillInfo.SkillConfig.EventChecker(SkillInfo, p, src, dest, damage, cardType, cardLevel, ref result);
            if (result)
            {
                SendSkillIcon(0);
                if (SkillInfo.SkillConfig.Effect != "")
                    BattleManager.Instance.EffectQueue.Add(new MonsterBindEffect(EffectBook.GetEffect(SkillInfo.SkillConfig.Effect), Self, false));
            }
        }

        private void SendSkillIcon(int key)
        {
            if (key != 0)
            {
                if (burst[key] == BurstStage.Fin)
                    return;//保证一次触发只播放一次
                burst[key] = BurstStage.Fin;
            }

            if (Self != null)
                BattleManager.Instance.FlowWordQueue.Add(new FlowSkillInfo(SkillId, Self.Position, 0, "lime", 20, 50, ""));
        }

        private void SendAreaEffect(Point pos)
        {
            //播放特效
            RegionTypes rt = BattleTargetManager.GetRegionType(SkillInfo.SkillConfig.Target[2]);
            var cardSize = BattleManager.Instance.MemMap.CardSize;
            foreach (var pickCell in BattleManager.Instance.MemMap.Cells)
            {
                var pointData = pickCell.ToPoint();
                if (BattleLocationManager.IsPointInRegionType(rt, pos.X, pos.Y, pointData, SkillInfo.SkillConfig.Range, Self.IsLeft))
                {
                    var effectData = new MonsterBindEffect(EffectBook.GetEffect(SkillInfo.SkillConfig.EffectArea), pointData + new Size(cardSize / 2, cardSize / 2), false);
                    BattleManager.Instance.EffectQueue.Add(effectData);
                }
            }
        }
    }
}
