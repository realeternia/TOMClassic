namespace TaleofMonsters.Core
{
    internal class HSErrorTypes
    {
        public const int OK = 0;
        public const int CommonError = 1;

        public const int BattleNoUseCard = 101;
        public const int BattleNoUseSpellCard = 102;
        public const int BattleLackMp = 103;
        public const int BattleLackLp = 104;
        public const int BattleLackPp = 105;
        public const int BattleHeroSkillInCd = 106;
        public const int CardOutPunish = 107;
        public const int CardFullPunish = 108;

        public const int DeckCardTypeLimitLegend = 1000;
        public const int DeckCardTypeLimit = 1001;
        public const int DeckIsFull = 1002;
        public const int CardExpNotEnough = 1003;
        public const int CardExpNotEnough2 = 1004;
        public const int CardJobTwice = 1005;

        public const int SceneLevelNeed = 2000;
        public const int SceneAPNotEnough = 2001;
        public const int SceneWarpNeedActive = 2002;

        public const int BagNotEnoughDimond = 3000;
        public const int BagNotEnoughResource = 3001;

        public static string GetDescript(int id)
        {
            return ConfigDatas.ConfigData.GetErrorConfig(id).Des;
        }
    }
}
