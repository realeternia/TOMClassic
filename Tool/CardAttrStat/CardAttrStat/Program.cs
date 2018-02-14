using System.Collections.Generic;
using System.IO;
using ConfigDatas;

namespace CardAttrStat
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigDatas.ConfigData.LoadData();

            StreamWriter sw = new StreamWriter("./race.txt");
            for (int i = (int) CardTypeSub.Devil; i <= (int) CardTypeSub.Totem; i++)
                CheckRace(i, sw);
            sw.Close();

            sw = new StreamWriter("./monster.txt");
            CheckMonster(sw);
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
            int ruleCount = 0;
            int tankCount = 0;
            int rageCount = 0;
            int elementCount = 0;
            int tileCount = 0;
            int killCount = 0;

            foreach (var monsterConfig in ConfigData.MonsterDict.Values)
            {
                if (monsterConfig.IsSpecial == 1)
                    continue;

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
                if (HasSkillType(monsterConfig, "规则"))
                    ruleCount++;
                if (HasSkillType(monsterConfig, "攻城"))
                    tankCount++;
                if (HasSkillType(monsterConfig, "怒火"))
                    rageCount++;
                if (HasSkillType(monsterConfig, "元素"))
                    elementCount++;
                if (HasSkillType(monsterConfig, "地形"))
                    tileCount++;
                if (HasSkillType(monsterConfig, "必杀"))
                    killCount++;
            }
            
            sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}", 
                tauntCount, rushCount, hideCount, auroCount, aoeCount, buffCount, overcomeCount, rangeUnit,
                defendUnit, summonCount, magCount, cardCount, deathSayCount, healCount, aidCount, ruleCount,
                tankCount, rageCount, elementCount, tileCount, killCount);
        }

        static void CheckMonster(StreamWriter sw)
        {
            Dictionary<string, bool> attrs = new Dictionary<string, bool>();
            foreach (var monsterConfig in ConfigData.MonsterDict.Values)
            {
                if (monsterConfig.IsSpecial == 1)
                    continue;
                attrs.Clear();
                if (monsterConfig.Quality == 0)
                    attrs["基本"] = true;

                if (HasSkill(monsterConfig, 55100008))
                    attrs["嘲讽"] = true;
                if (HasSkill(monsterConfig, 55100005))
                    attrs["冲锋"] = true;
                if (HasSkill(monsterConfig, 55100009))
                    attrs["隐藏"] = true;
                if (HasSkillType(monsterConfig, "光环"))
                    attrs["光环"] = true;
                if (HasSkillType(monsterConfig, "范围"))
                    attrs["AOE"] = true;
                if (HasSkillType(monsterConfig, "状态"))
                    attrs["状态"] = true;
                if (HasSkillType(monsterConfig, "克制"))
                    attrs["克制"] = true;
                if (monsterConfig.Range >= 20)
                    attrs["范围"] = true;
                if (monsterConfig.Mov == 0)
                    attrs["防御"] = true;
                if (HasSkillType(monsterConfig, "召唤"))
                    attrs["召唤"] = true;
                if (HasSkillType(monsterConfig, "魔法"))
                    attrs["魔法"] = true;
                if (HasSkillType(monsterConfig, "过牌"))
                    attrs["过牌"] = true;
                if (HasSkillType(monsterConfig, "亡语"))
                    attrs["亡语"] = true;
                if (HasSkillType(monsterConfig, "回复"))
                    attrs["回复"] = true;
                if (HasSkillType(monsterConfig, "支援"))
                    attrs["支援"] = true;
                if (HasSkillType(monsterConfig, "规则"))
                    attrs["规则"] = true;
                if (HasSkillType(monsterConfig, "攻城"))
                    attrs["攻城"] = true;
                if (HasSkillType(monsterConfig, "怒火"))
                    attrs["怒火"] = true;
                if (HasSkillType(monsterConfig, "连击"))
                    attrs["连击"] = true;
                if (HasSkillType(monsterConfig, "元素"))
                    attrs["元素"] = true;
                if (HasSkillType(monsterConfig, "地形"))
                    attrs["地形"] = true;
                if (HasSkillType(monsterConfig, "必杀"))
                    attrs["必杀"] = true;

                if (attrs.Count > 0)
                {
                    List<string> dts = new List<string>();
                    foreach (var key in attrs.Keys)
                        dts.Add(key);
                    sw.WriteLine(string.Join(",", dts.ToArray()));
                }
                else
                {
                    sw.WriteLine();
                }
            }
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
