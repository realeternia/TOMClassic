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

        public const int DeckCardAlreadyIn = 1000;
        public const int DeckCardTypeLimit = 1001;
        public const int DeckIsFull = 1002;
        public const int CardExpNotEnough = 1003;
        public const int CardJobTwice = 1004;

        public static string GetDescript(int id)
        {
            return ConfigDatas.ConfigData.GetErrorConfig(id).Des;
        }
    }
}
