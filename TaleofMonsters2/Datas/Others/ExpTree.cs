namespace TaleofMonsters.Datas.Others
{
    internal class ExpTree
    {
        public const int MaxLevel = 99;

        public static int GetNextRequired(int level)
        {
            return ConfigDatas.ConfigData.GetLevelExpConfig(level).Exp;
        }

        public static int GetNextRequiredCard(int level)
        {
            return ConfigDatas.ConfigData.GetLevelExpConfig(level).CardExp;
        }
        public static int GetNextRequiredEquip(int level)
        {
            return ConfigDatas.ConfigData.GetLevelExpConfig(level).EquipExp;
        }
        public static double GetGoldFactor(int level)
        {
            return ConfigDatas.ConfigData.GetLevelExpConfig(level).GoldFactor;
        }
        public static double GetResFactor(int level)
        {
            return ConfigDatas.ConfigData.GetLevelExpConfig(level).ResFactor;
        }
    }
}
