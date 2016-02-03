using ConfigDatas;
using NarlonLib.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards.Monsters;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    /// <summary>
    /// 只管战斗过程中一些状态的记录
    /// </summary>
    internal class PlayerState
    {
        public AutoDictionary<int, int> Monsterskills = new AutoDictionary<int, int>();
        public AutoDictionary<int, int> Weaponskills = new AutoDictionary<int, int>();
        public AutoDictionary<int, int> Masterskills = new AutoDictionary<int, int>();
        private AutoDictionary<int, int> MonsterTypeCounts = new AutoDictionary<int, int>();//属性类型为key

        public PlayerState()
        {
        }

        public void UpdateSkills(int[] sidArray, int[] svalueArray)
        {
            for (int i = 0; i < sidArray.Length; i ++)
            {
                var sid = sidArray[i];
                switch (ConfigData.GetEquipAddonConfig(sid).Type)
                {
                    case "mon": Monsterskills[sid] = Monsterskills[sid] + svalueArray[i]; break;
                    case "master": Masterskills[sid] = Masterskills[sid] + svalueArray[i]; break;
                    case "weapon": Weaponskills[sid] = Weaponskills[sid] + svalueArray[i]; break;
                }
            }
        }

        public void CheckMonsterEvent(bool isAdd, Monster mon)
        {
            if (isAdd)
            {
                MonsterTypeCounts[(int) MonsterCountTypes.Total]++;
                MonsterTypeCounts[mon.MonsterConfig.Attr + 10]++;
                MonsterTypeCounts[mon.MonsterConfig.Type + 20]++;
            }
            else
            {
                MonsterTypeCounts[(int)MonsterCountTypes.Total]--;
                MonsterTypeCounts[mon.MonsterConfig.Attr + 10]--;
                MonsterTypeCounts[mon.MonsterConfig.Type + 20]--;
            }
        }

        public int GetMonsterCountByType(MonsterCountTypes type)
        {
            return MonsterTypeCounts[(int)type];
        }
    }
}
