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
            int rushCount = 0;

            foreach (var monsterConfig in ConfigData.MonsterDict.Values)
            {
                if (monsterConfig.Type != rid)
                    continue;
                if (monsterConfig.Skill1 == 55100008 || monsterConfig.Skill2 == 55100008)
                    tauntCount ++;
                if (monsterConfig.Skill1 == 55100005 || monsterConfig.Skill2 == 55100005)
                    rushCount++;
            }
            
            sw.WriteLine("{0}\t{1}", tauntCount, rushCount);
        }
    }
}
