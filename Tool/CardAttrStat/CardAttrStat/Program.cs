using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ConfigDatas;

namespace CardAttrStat
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigDatas.ConfigData.LoadData();

            StreamWriter sw = new StreamWriter("./a.txt");
            for (int i = (int) CardTypeSub.Devil; i <= (int) CardTypeSub.Totem; i++)
                CheckRace(i, sw);
            sw.Close();
        }

        static void CheckRace(int rid, StreamWriter sw)
        {
            int tauntCount = 0;
            int hideCount = 0;
            int rushCount = 0;
            int auroCount = 0;
            int aoeCount = 0;
            int buffCount = 0;
            int overcomeCount = 0;
            int rangeUnit = 0;
            int defendUnit = 0;
            int summonCount = 0;
            int magCount = 0;
            int cardCount = 0;
            int deathSayCount = 0;
            int healCount = 0;
            int aidCount = 0;

            foreach (var monsterConfig in ConfigData.MonsterDict.Values)
            {
                if (monsterConfig.Type != rid)
                    continue;
                if (HasSkill(monsterConfig, 55100008))
                    tauntCount ++;
                if (HasSkill(monsterConfig, 55100005))
                    rushCount++;
                if (HasSkill(monsterConfig, 55100009))
                    hideCount++;
                if (HasSkillType(monsterConfig, "光环"))
                    auroCount++;
                if (HasSkillType(monsterConfig, "范围"))
                    aoeCount++;
                if (HasSkillType(monsterConfig, "状态"))
                    buffCount++;
                if (HasSkillType(monsterConfig, "克制"))
                    overcomeCount++;
                if (monsterConfig.Range >= 20)
                    rangeUnit++;
                if (monsterConfig.Mov == 0)
                    defendUnit++;
                if (HasSkillType(monsterConfig, "召唤"))
                    summonCount++;
                if (HasSkillType(monsterConfig, "魔法"))
                    magCount++;
                if (HasSkillType(monsterConfig, "过牌"))
                    cardCount++;
                if (HasSkillType(monsterConfig, "亡语"))
                    deathSayCount++;
                if (HasSkillType(monsterConfig, "回复"))
                    healCount++;
                if (HasSkillType(monsterConfig, "支援"))
                    aidCount++;
            }
            
            sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}", 
                tauntCount, rushCount, hideCount, auroCount, aoeCount, buffCount, overcomeCount, rangeUnit,
                defendUnit, summonCount, magCount, cardCount, deathSayCount, healCount, aidCount);
        }

        private static bool HasSkill(MonsterConfig monsterConfig, int sid)
        {
            if (monsterConfig.Skill1 == sid || monsterConfig.Skill2 == sid)
                return true;
            return false;
        }
        private static bool HasSkillType(MonsterConfig monsterConfig, string type)
        {
            if (monsterConfig.Skill1 > 0)
            {
                var skillConfig = ConfigData.GetSkillConfig(monsterConfig.Skill1);
                if (skillConfig.Type == type || skillConfig.Remark.Contains(type))
                    return true;
            }
            if (monsterConfig.Skill2 > 0)
            {
                var skillConfig = ConfigData.GetSkillConfig(monsterConfig.Skill2);
                if (skillConfig.Type == type || skillConfig.Remark.Contains(type))
                    return true;
            }
            return false;
        }
    }
}
