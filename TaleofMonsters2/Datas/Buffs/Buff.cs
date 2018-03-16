using ConfigDatas;

namespace TaleofMonsters.Datas.Buffs
{
    internal class Buff : IBuff
    {
        public BuffConfig BuffConfig { get; private set; }

        public int Level { get; set; }

        public int Id
        {
            get { return BuffConfig.Id; }
        }

        public Buff(int id)
        {
            BuffConfig = ConfigData.BuffDict[id];
            Level = 1;
        }

        public void UpgradeToLevel(int newLevel)
        {
            Level = newLevel;
        }

        public string Descript
        {
            get { return BuffConfig.GetDescript(Level); }
        }

    }
}
