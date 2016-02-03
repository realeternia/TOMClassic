using ConfigDatas;

namespace TaleofMonsters.DataType.Buffs
{
    internal class Buff
    {
        public BuffConfig BuffConfig;

        private int lv = 1;

        public int Id
        {
            get { return BuffConfig.Id; }
        }

        public Buff(int id)
        {
            BuffConfig = ConfigData.BuffDict[id];
        }

        public void UpgradeToLevel(int newLevel)
        {
            lv = newLevel;
        }

        public string Descript
        {
            get
            {
                return BuffConfig.GetDescript(lv);
            }
        }

    }
}
