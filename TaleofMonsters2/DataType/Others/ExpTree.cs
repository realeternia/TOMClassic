namespace TaleofMonsters.DataType.Others
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
        public static double GetResourceFactor(int level)
        {
            return ConfigDatas.ConfigData.GetLevelExpConfig(level).ResouceFactor;
        }
    }
}
